using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompteResultat.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;

namespace CompteResultat.DAL
{
    [MetadataType(typeof(CotisatPrev.MetaData))]
    public partial class CotisatPrev
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<ExcelGlobalCotisatData> GetCotisatGlobalEntDataPrev(List<int> years, List<string> companyList)
        {
            try
            {
                List<ExcelGlobalCotisatData> cotisat = new List<ExcelGlobalCotisatData>();

                using (var context = new CompteResultatEntities())
                {
                    cotisat = context.CotisatPrevs
                    .Where(d => years.Contains((d.Year.HasValue ? d.Year.Value : 0)) && companyList.Contains(d.Company))
                    .GroupBy(p => new { p.AssureurName, p.Company, AnnSurv = p.Year })
                    .Select(g => new ExcelGlobalCotisatData
                    {
                        Assureur = g.Key.AssureurName,
                        Company = g.Key.Company,
                        Subsid = "",
                        YearSurv = g.Key.AnnSurv.HasValue ? g.Key.AnnSurv.Value : 0,
                        Cotisat = g.Sum(i => i.Cotisation),
                        CotisatBrute = g.Sum(i => i.CotisationBrute)
                    })
                    .OrderBy(gr => gr.YearSurv).ThenBy(ga => ga.Company)
                    .ToList();
                }

                return cotisat;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }
        
        public static List<ExcelGlobalCotisatData> GetCotisatGlobalSubsidDataPrev(List<int> years, List<string> subsidList)
        {
            try
            {
                List<ExcelGlobalCotisatData> cotisat = new List<ExcelGlobalCotisatData>();

                using (var context = new CompteResultatEntities())
                {
                    cotisat = context.CotisatPrevs
                    .Where(d => years.Contains((d.Year.HasValue ? d.Year.Value : 0)) && subsidList.Contains(d.Subsid))
                    .GroupBy(p => new { p.AssureurName, p.Company, p.Subsid, AnnSurv = p.Year })
                    .Select(g => new ExcelGlobalCotisatData
                    {
                        Assureur = g.Key.AssureurName,
                        Company = g.Key.Company,
                        Subsid = g.Key.Subsid,
                        YearSurv = g.Key.AnnSurv.HasValue ? g.Key.AnnSurv.Value : 0,
                        Cotisat = g.Sum(i => i.Cotisation),
                        CotisatBrute = g.Sum(i => i.CotisationBrute)
                    })
                    .OrderBy(ga => ga.YearSurv).ThenBy(gb => gb.Company).ThenBy(gc => gc.Subsid)
                    .ToList();
                }

                return cotisat;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<CotisatPrev> GetCotisationsForContracts(List<string> assurList, List<string> parentCompanyList, List<string> companyList, 
            List<string> contrIds, string college, DateTime debutPeriod, DateTime finPeriod)
        {
            try
            {
                List<CotisatPrev> cotisat;

                using (var context = new CompteResultatEntities())
                {
                    cotisat = context.CotisatPrevs.Where(cot => assurList.Contains(cot.AssureurName) && parentCompanyList.Contains(cot.Company)
                        && companyList.Contains(cot.Subsid) && contrIds.Contains(cot.ContractId)
                        && cot.DebPrime >= debutPeriod && cot.FinPrime <= finPeriod).ToList();

                    //cotisat = context.CotisatPrevs.Where(cot => contrIds.Contains(cot.ContractId)
                    //    && cot.DebPrime >= debutPeriod && cot.FinPrime <= finPeriod).ToList();                    
                }

                return cotisat;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void DeleteRowsWithImportId(int importId)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.Database.ExecuteSqlCommand("DELETE FROM CotisatPrev WHERE ImportId = {0}", importId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        //MetaData definition for basic validation
        public class MetaData
        {
            //[Display(Name = "Email address")]
            //[Required(ErrorMessage = "The email address is required")]
            //public string Email { get; set; }

        }
    }
}
