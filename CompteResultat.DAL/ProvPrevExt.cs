using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using CompteResultat.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;

namespace CompteResultat.DAL
{
    [MetadataType(typeof(DecomptePrev.MetaData))]
    public partial class ProvPrev
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<ProvPrev> GetProvPrevForContracts(List<string> assurList, List<string> parentCompanyList, List<string> companyList,
            List<string> contrIds, string college, DateTime debutPeriod, DateTime finPeriod, DateTime dateArret, C.eTypeComptes typeComptes)
        {
            try
            {
                List<ProvPrev> provPrev;

                using (var context = new CompteResultatEntities())
                {
                    if (typeComptes == C.eTypeComptes.Comptable)
                    {
                        //Comptable => no test on IsComptable
                        provPrev = context.ProvPrevs.Where(pp => assurList.Contains(pp.AssureurName) && parentCompanyList.Contains(pp.Company)
                        && companyList.Contains(pp.Subsid) && contrIds.Contains(pp.ContractId)
                        && pp.DateProvision >= debutPeriod && pp.DateProvision <= finPeriod).ToList();
                    }
                    else
                    {
                        //Survenance => pp.IsComptable == false : 0
                        provPrev = context.ProvPrevs.Where(pp => assurList.Contains(pp.AssureurName) && parentCompanyList.Contains(pp.Company)
                        && companyList.Contains(pp.Subsid) && contrIds.Contains(pp.ContractId)
                        && pp.DateSinistre >= debutPeriod && pp.DateSinistre <= finPeriod
                        && pp.DateProvision == dateArret && pp.IsComptable == "0").ToList();
                    }
                }

                return provPrev;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        // OLD désactivé le 0208 2021 AM
        public static List<ProvPrev> GetProvPrevGlobalEntData(List<string> parentCompanyList, List<string> companyList,
            DateTime debutPeriod, DateTime finPeriod, DateTime dateArret, C.eTypeComptes TypeComptes)
        {
            try
            {
                List<ProvPrev> provPrev;

                using (var context = new CompteResultatEntities())
                {
                    if (TypeComptes == C.eTypeComptes.Comptable)
                    {
                        //Comptable => no test on IsComptable
                        provPrev = context.ProvPrevs.Where(pp => parentCompanyList.Contains(pp.Company)
                        && companyList.Contains(pp.Subsid)
                        && pp.DateProvision >= debutPeriod && pp.DateProvision <= finPeriod).ToList();
                    }
                    else
                    {
                        //Survenance => pp.IsComptable == false : 0
                        provPrev = context.ProvPrevs.Where(pp => parentCompanyList.Contains(pp.Company)
                        && companyList.Contains(pp.Subsid)
                        && pp.DateSinistre >= debutPeriod && pp.DateSinistre <= finPeriod
                        && pp.DateProvision == dateArret && pp.IsComptable == "0"  ).ToList();
                    }
                }

                return provPrev;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        // NEW ACTIVE le 0208 2021 AM
        public static List<ProvPrev> GetProvPrevGlobalEntData_NEW(List<int> years, List<string> parentCompanyList, DateTime dateArret)
        {
            try
            {
                List<ProvPrev> provPrev;

                using (var context = new CompteResultatEntities())
                {
                    //selection with DateProvision
                    provPrev = context.ProvPrevs
                        .Where(pp => years.Contains(pp.DateSinistre.Value.Year) && parentCompanyList.Contains(pp.Company) && pp.DateProvision == dateArret)
                        .GroupBy(p => new { p.AssureurName, p.Company, AnnSurv = p.DateSinistre.Value.Year })
                        .Select (g=> new ProvPrev
                         {
                            Pm = g.Sum(i => i.Pm),
                            PmPassage = g.Sum(i => i.PmPassage),
                            Psap = g.Sum(i => i.Psap),
                            PmMgdc = g.Sum(i => i.PmMgdc),
                            Psi = g.Sum(i => i.Psi),
                            PmPortabilite = g.Sum(i => i.PmPortabilite),
                        })
                        .ToList();
                }

                return provPrev;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetSinistreLabels()
        {
            try
            {
                List<string> sinistreLabels;

                using (var context = new CompteResultatEntities())
                {
                    sinistreLabels = context.Database
                            .SqlQuery<string>("SELECT DISTINCT NatureSinistre FROM dbo.ProvPrev ORDER BY NatureSinistre")
                            .ToList<string>();
                }

                return sinistreLabels;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


    }

    public class MetaData
    {
        //[Display(Name = "Email address")]
        //[Required(ErrorMessage = "The email address is required")]
        //public string Email { get; set; }

    }
}
