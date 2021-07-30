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
    public partial class DecomptePrev
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<ExcelGlobalDecompteData> GetDecompteGlobalEntData(List<int> years, List<string> companyList)
        {
            try
            {
                List<ExcelGlobalDecompteData> decomptes = new List<ExcelGlobalDecompteData>();

                using (var context = new CompteResultatEntities())
                {
                    decomptes = context.DecomptePrevs
                    .Where(d => years.Contains(d.DateSin.Value.Year) && companyList.Contains(d.Company))
                    .GroupBy(p => new { p.AssureurName, p.Company, AnnSurv = p.DateSin.Value.Year })
                    .Select(g => new ExcelGlobalDecompteData
                    {
                        Assureur = g.Key.AssureurName,
                        Company = g.Key.Company,
                        Subsid = "",
                        YearSurv = g.Key.AnnSurv,
                        FR = 0,
                        RSS = 0,
                        RAnnexe = 0,
                        RNous = g.Sum(i => i.Total),
                        Provisions = 0,
                        CotBrute = 0,
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

                return decomptes;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalDecompteData> GetDecompteGlobalSubsidData(List<int> years, List<string> subsidList)
        {
            try
            {
                List<ExcelGlobalDecompteData> decomptes = new List<ExcelGlobalDecompteData>();

                using (var context = new CompteResultatEntities())
                {
                    decomptes = context.DecomptePrevs
                    //.Where(d => years.Contains(d.DateSin.Value.Year) && subsidList.Contains(d.Company)) //RS désactivé 12/11/2020
                    .Where(d => years.Contains(d.DateSin.Value.Year) )
                    .GroupBy(p => new { p.AssureurName, p.Company, p.Subsid, AnnSurv = p.DateSin.Value.Year })
                    .Select(g => new ExcelGlobalDecompteData
                    {
                        Assureur = g.Key.AssureurName,
                        Company = g.Key.Company,
                        Subsid = g.Key.Subsid,
                        YearSurv = g.Key.AnnSurv,
                        FR = 0,
                        RSS = 0,
                        RAnnexe = 0,
                        RNous = g.Sum(i => i.Total),
                        Provisions = 0,
                        CotBrute = 0,
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

                return decomptes;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<DecomptePrev> GetDecomptesForDossier(string dossier, DateTime dateArret)
        {
            try
            {
                List<DecomptePrev> decomptePrev;

                using (var context = new CompteResultatEntities())
                {
                    decomptePrev = context.DecomptePrevs.Where(d => d.Dossier == dossier && d.DatePayement <= dateArret).ToList();                       
                }

                return decomptePrev;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<DecomptePrevReduced> GetDecomptesForDossierList(List<string> assurList, List<string> dossiers, DateTime dateArret)
        {
            try
            {
                List<DecomptePrevReduced> decomptes;

                using (var context = new CompteResultatEntities())
                {
                    decomptes = context.DecomptePrevs
                        .Where(dec => assurList.Contains(dec.AssureurName) &&  dossiers.Contains(dec.Dossier) && dec.DatePayement <= dateArret)
                       .Select(dec => new DecomptePrevReduced {
                            Dossier = dec.Dossier,
                            Total = dec.Total,
                            DatePayement = dec.DatePayement,
                            DebSin = dec.DebSin,
                            FinSin = dec.FinSin}).ToList();

                }

                return decomptes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static decimal GetSumPrestaForDossier(string dossier, DateTime dateArret)
        {
            try
            {
                decimal sumPresta = 0;

                using (var context = new CompteResultatEntities())
                {                    
                    var res = context.DecomptePrevs
                        .Where(d => d.Dossier == dossier && d.DatePayement <= dateArret)
                        .Sum(d => d.Total);

                    if (res.HasValue)
                        sumPresta = (decimal)res.Value;
                }

                return sumPresta;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static double GetSumPrestaForDossierFromSP(string dossier, DateTime dateArret)
        {
            try
            {
                double sumPresta = 0;

                using (var context = new CompteResultatEntities())
                {                    
                    var res = context.SPGetSumPrestaForDossier(dossier, dateArret).FirstOrDefault();

                    if (res.HasValue)
                        sumPresta = res.Value;
                }

                return sumPresta;
            }
            catch (Exception ex)
            {
                log.Error("Error :: GetSumPrestaForDossierFromSP : " + ex.Message);
                throw ex;
            }
        }


        public static double GetSumPrestaAnnualized(string dossier, DateTime dateArret)
        {
            try
            {
                double sumPresta = 0;

                using (var context = new CompteResultatEntities())
                {
                    var res = context.DecomptePrevs
                        .Where(d => d.Dossier == dossier && d.DatePayement <= dateArret && d.FinSin.HasValue && d.DebSin.HasValue && d.Total.HasValue)
                        .Sum(d => d.Total / (int)(DbFunctions.DiffDays(d.DebSin.Value, d.FinSin.Value) + 1) * 365 );                    

                    if (res.HasValue)
                        sumPresta = res.Value;
                }

                return sumPresta;
            }
            catch (Exception ex)
            {
                log.Error("Error :: GetSumPrestaAnnualized : " + ex.Message);
                throw ex;
            }
        }

        public static double GetSumPrestaAnnualizedFromSP(string dossier, DateTime dateArret)
        {
            try
            {
                double sumPresta = 0;

                using (var context = new CompteResultatEntities())
                {
                    var res = context.SPGetSumPrestaAnnualized(dossier, dateArret).FirstOrDefault();

                    if (res.HasValue)
                        sumPresta = res.Value;
                }

                return sumPresta;
            }
            catch (Exception ex)
            {
                log.Error("Error :: GetSumPrestaAnnualizedFromSP : " + ex.Message);
                throw ex;
            }
        }

        public static DateTime? GetDateMaxForDossier(string dossier, DateTime dateArret)
        {
            try
            {                                
                using (var context = new CompteResultatEntities())
                {
                    var res = context.DecomptePrevs
                        .Where(d => d.Dossier == dossier && d.DatePayement <= dateArret)
                        .Max(d => d.FinSin);

                    if (res.HasValue)
                        return res.Value;
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error :: GetDateMaxForDossier : " + ex.Message);
                throw ex;
            }
        }

        public static DateTime? GetDateMaxForDossierFromSP(string dossier, DateTime dateArret)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {

                    var res = context.SPGetDateMaxForDossier(dossier, dateArret).FirstOrDefault();

                    if (res.HasValue)
                        return res.Value;
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error :: GetDateMaxForDossierFromSP : " + ex.Message);
                throw ex;
            }
        }





        public static void DeleteRowsWithImportId(int importId)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.Database.ExecuteSqlCommand("DELETE FROM DecomptePrev WHERE ImportId = {0}", importId);
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
