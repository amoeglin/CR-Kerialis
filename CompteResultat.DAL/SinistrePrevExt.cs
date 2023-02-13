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
    [MetadataType(typeof(SinistrePrev.MetaData))]
    public partial class SinistrePrev
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static SinistrePrev GetSinistrePrevtById(int id)
        {
            try
            {
                SinistrePrev sin = null;
                using (var context = new CompteResultatEntities())
                {
                    var elements = context.SinistrePrevs.Where(c => c.Id == id);

                    if (elements.Any())
                    {
                        sin = elements.First();
                    }
                }
                return sin;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalSinistreData> GetSinistreGlobalEntData(List<int> years, List<string> companyList)
        {
            try
            {
                List<ExcelGlobalSinistreData> sinistres = new List<ExcelGlobalSinistreData>();

                using (var context = new CompteResultatEntities())
                {
                    sinistres = context.SinistrePrevs
                    .Where(d => years.Contains(d.DateSinistre.Value.Year) && companyList.Contains(d.Company))
                    .GroupBy(p => new { p.AssureurName, p.Company, AnnSurv = p.DateSinistre.Value.Year })
                    .Select(g => new ExcelGlobalSinistreData
                    {
                        Assureur = g.Key.AssureurName,
                        Company = g.Key.Company,
                        Subsid = "",
                        YearSurv = g.Key.AnnSurv,
                        FR = 0,
                        RSS = 0,
                        RAnnexe = 0,
                        RNous = 0,
                        Provisions = 0,
                        CotBrut = 0,
                        TaxTotal = "",
                        TaxDefault = "",
                        TaxActive = "",
                        CotNet = 0,
                        Ratio = 0,
                        GainLoss = 0,
                        DateArret = DateTime.Now
                    })
                    //.OrderBy(ga => ga.YearSurv).ThenBy(gb => gb.Company)
                    .OrderBy(ga => ga.Company).ThenBy(gb => gb.Subsid).ThenBy(gc => gc.YearSurv)
                    .ToList();
                }

                return sinistres;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalSinistreData> GetSinistreGlobalSubsidData(List<int> years, List<string> subsidList)
        {
            try
            {
                List<ExcelGlobalSinistreData> sinistres = new List<ExcelGlobalSinistreData>();

                using (var context = new CompteResultatEntities())
                {
                    sinistres = context.SinistrePrevs
                    .Where(d => years.Contains(d.DateSinistre.Value.Year) && subsidList.Contains(d.Company))
                    .GroupBy(p => new { p.AssureurName, p.Company, p.Subsid, AnnSurv = p.DateSinistre.Value.Year })
                    .Select(g => new ExcelGlobalSinistreData
                    {
                        Assureur = g.Key.AssureurName,
                        Company = g.Key.Company,
                        Subsid = g.Key.Subsid,
                        YearSurv = g.Key.AnnSurv,
                        FR = 0,
                        RSS = 0,
                        RAnnexe = 0,
                        RNous = 0,
                        Provisions = 0,
                        CotBrut = 0,
                        TaxTotal = "",
                        TaxDefault = "",
                        TaxActive = "",
                        CotNet = 0,
                        Ratio = 0,
                        GainLoss = 0,
                        DateArret = DateTime.Now
                    })
                    //.OrderBy(ga => ga.YearSurv).ThenBy(gb => gb.Company).ThenBy(gc => gc.Subsid)
                    .OrderBy(ga => ga.Company).ThenBy(gb => gb.Subsid).ThenBy(gc => gc.YearSurv)
                    .ToList();
                }

                return sinistres;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<SinistrePrev> GetSinistresForContracts(List<string> assurList, List<string> parentCompanyList, List<string> companyList, 
            List<string> contrIds, string college, DateTime debutPeriod, DateTime finPeriod, DateTime dateArret, C.eTypeComptes typeCompte)
        {
            try
            {
                List<SinistrePrev> sinistres;

                using (var context = new CompteResultatEntities())
                {
                    if (typeCompte == C.eTypeComptes.Survenance)
                    {
                        sinistres = context.SinistrePrevs.Where(sin => assurList.Contains(sin.AssureurName) && parentCompanyList.Contains(sin.Company)
                            && companyList.Contains(sin.Subsid) && contrIds.Contains(sin.ContractId)
                            && sin.DateSinistre >= debutPeriod && sin.DateSinistre <= finPeriod).ToList();
                    }
                    else
                    {
                        sinistres = context.SinistrePrevs.Where(sin => assurList.Contains(sin.AssureurName) && parentCompanyList.Contains(sin.Company)
                            && companyList.Contains(sin.Subsid) && contrIds.Contains(sin.ContractId)).ToList();
                    }
                }

                return sinistres; 

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
                    context.Database.ExecuteSqlCommand("DELETE FROM SinistrePrev WHERE ImportId = {0}", importId);
                }
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
                            .SqlQuery<string>("SELECT DISTINCT NatureSinistre FROM dbo.SinistrePrev ORDER BY NatureSinistre")
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

        //MetaData definition for basic validation
        public class MetaData
        {
            //[Display(Name = "Email address")]
            //[Required(ErrorMessage = "The email address is required")]
            //public string Email { get; set; }

        }
    }
}
