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

        public static DecomptePrev GetDecomptById(int id)
        {
            try
            {
                DecomptePrev decomp = null;
                using (var context = new CompteResultatEntities())
                {
                    var elements = context.DecomptePrevs.Where(c => c.Id == id);

                    if (elements.Any())
                    {
                        decomp = elements.First();
                    }
                }
                return decomp;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalDecompteData> GetDecompteGlobalEntData(List<int> years, List<string> companyList, DateTime dateArret)
        {
            try
            {
                List<ExcelGlobalDecompteData> decomptes = new List<ExcelGlobalDecompteData>();

                using (var context = new CompteResultatEntities())
                {                   
                    //string strDate = dateArret.ToString("dd/M/yyy");
                    string strDate = dateArret.ToString("yyy-M-dd");
                    string strYears = String.Join(",", years.ToArray());
                    string strCompanies = String.Join(",", companyList.ToArray());
                    strCompanies = strCompanies.Replace("'", "''");
                    string companiesForSql = "'" + strCompanies.Replace(",", "','") + "'";

                    string sql = $@"Select DP.AssureurName AS Assureur, DP.ContractId, DP.Company, YEAR(DP.DateSin) AS YearSurv, SUM(DP.Total) AS RNous
                        FROM DecomptePrev DP 
                        INNER JOIN SinistrePrev SP on DP.Dossier = SP.Dossier AND DP.AssureurName = SP.AssureurName AND DP.DateSin = SP.DateSinistre AND DP.CauseSinistre = SP.NatureSinistre                                                
                        WHERE YEAR(DP.DateSin) in ({strYears}) AND DP.Company in ({companiesForSql}) AND DP.DatePayement <= '{strDate}' 
                        GROUP BY DP.AssureurName, DP.Company, YEAR(DP.DateSin), DP.ContractId
                        ORDER BY DP.Company, YEAR(DP.DateSin)";

                    decomptes = context.Database.SqlQuery<ExcelGlobalDecompteData>(sql)
                        .Select(d => new ExcelGlobalDecompteData
                        {
                            Assureur = d.Assureur,
                            ContractId = d.ContractId,
                            Company = d.Company,
                            DateArret = dateArret,
                            RNous = d.RNous,
                            YearSurv = d.YearSurv

                        })
                        .ToList<ExcelGlobalDecompteData>();
                }

                //using (var context = new CompteResultatEntities())
                //{
                //    decomptes = context.DecomptePrevs
                //    .Where(d => years.Contains(d.DateSin.Value.Year) && companyList.Contains(d.Company) && d.DatePayement <= dateArret )
                //    .GroupBy(p => new { p.AssureurName, p.Company, AnnSurv = p.DateSin.Value.Year })
                //    .Select(g => new ExcelGlobalDecompteData
                //    {
                //        Assureur = g.Key.AssureurName,
                //        Company = g.Key.Company,
                //        Subsid = "",
                //        YearSurv = g.Key.AnnSurv,
                //        FR = 0,
                //        RSS = 0,
                //        RAnnexe = 0,
                //        RNous = g.Sum(i => i.Total),
                //        Provisions = 0,
                //        CotBrute = 0,
                //        TaxTotal = "",
                //        TaxDefault = "",
                //        TaxActive = "",
                //        CotNet = 0,
                //        Ratio = 0,
                //        GainLoss = 0,
                //        DateArret = DateTime.Now
                //    })
                //    //.OrderBy(ga => ga.YearSurv).ThenBy(gb => gb.Company)
                //    .OrderBy(ga => ga.Company).ThenBy(gb => gb.Subsid).ThenBy(gc => gc.YearSurv)
                //    .ToList();
                //}

                return decomptes;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalDecompteData> GetDecompteGlobalEntDataCompta(List<int> years, List<string> companyList, DateTime dateArret)
        {
            try
            {
                List<ExcelGlobalDecompteData> decomptes = new List<ExcelGlobalDecompteData>();

                using (var context = new CompteResultatEntities())
                {
                    //string strDate = dateArret.ToString("dd/M/yyy");
                    string strDate = dateArret.ToString("yyy-M-dd");
                    string strYears = String.Join(",", years.ToArray());
                    string strCompanies = String.Join(",", companyList.ToArray());
                    strCompanies = strCompanies.Replace("'", "''");
                    string companiesForSql = "'" + strCompanies.Replace(",", "','") + "'";

                    string sql = $@"Select DP.AssureurName AS Assureur, DP.ContractId, DP.Company, YEAR(DP.DateSin) AS YearSurv, SUM(DP.Total) AS RNous
                        FROM DecomptePrev DP 
                        INNER JOIN SinistrePrev SP on DP.Dossier = SP.Dossier AND DP.AssureurName = SP.AssureurName AND DP.DateSin = SP.DateSinistre AND DP.CauseSinistre = SP.NatureSinistre                                                
                        WHERE YEAR(DP.DatePayement) in ({strYears}) AND DP.Company in ({companiesForSql}) AND DP.DatePayement <= '{strDate}' 
                        GROUP BY DP.AssureurName, DP.Company, YEAR(DP.DateSin), DP.ContractId, DP.CauseSinistre
                        ORDER BY DP.Company, YEAR(DP.DateSin)";

                    decomptes = context.Database.SqlQuery<ExcelGlobalDecompteData>(sql)
                        .Select(d => new ExcelGlobalDecompteData
                        {
                            Assureur = d.Assureur,
                            ContractId = d.ContractId,
                            Company = d.Company,
                            DateArret = dateArret,
                            RNous = d.RNous,
                            YearSurv = d.YearSurv

                        })
                        .ToList<ExcelGlobalDecompteData>();
                }
              
                return decomptes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalDecompteData> GetDecompteGlobalEntDataWithGarantie(List<int> years, List<string> companyList, DateTime dateArret)
        {
            try
            {
                List<ExcelGlobalDecompteData> decomptes = new List<ExcelGlobalDecompteData>();

                using (var context = new CompteResultatEntities())
                {
                    string strDate = dateArret.ToString("yyy-M-dd");
                    string strYears = String.Join(",", years.ToArray());
                    string strCompanies = String.Join(",", companyList.ToArray());
                    strCompanies = strCompanies.Replace("'", "''");
                    string companiesForSql = "'" + strCompanies.Replace(",", "','") + "'";

                    string sql = $@"Select DP.AssureurName AS Assureur, DP.ContractId, DP.Company, YEAR(DP.DateSin) AS YearSurv, 
                        TP.CodeSinistre AS CodeGarantie, SUM(DP.Total) AS RNous
                        FROM DecomptePrev DP 
                        INNER JOIN SinistrePrev SP on DP.Dossier = SP.Dossier AND DP.AssureurName = SP.AssureurName AND DP.DateSin = SP.DateSinistre AND DP.CauseSinistre = SP.NatureSinistre
                        INNER JOIN TypePrevoyance TP on TP.LabelSinistre = DP.CauseSinistre                         
                        WHERE YEAR(DP.DateSin) in ({strYears}) AND DP.Company in ({companiesForSql}) AND DP.DatePayement <= '{strDate}' 
                        GROUP BY DP.AssureurName, DP.Company, YEAR(DP.DateSin), TP.CodeSinistre, DP.ContractId, DP.CauseSinistre
                        ORDER BY DP.Company, YEAR(DP.DateSin)";

                    decomptes = context.Database.SqlQuery<ExcelGlobalDecompteData>(sql)
                        .Select(d => new ExcelGlobalDecompteData
                        {
                            Assureur = d.Assureur,
                            Company = d.Company,
                            ContractId = d.ContractId,
                            DateArret = dateArret,
                            RNous = d.RNous,
                            YearSurv = d.YearSurv,
                            CodeGarantie = d.CodeGarantie
                            
                        })
                        .ToList<ExcelGlobalDecompteData>();
                }

                return decomptes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalDecompteData> GetDecompteGlobalEntDataWithGarantieCompta(List<int> years, List<string> companyList, DateTime dateArret)
        {
            try
            {
                List<ExcelGlobalDecompteData> decomptes = new List<ExcelGlobalDecompteData>();

                using (var context = new CompteResultatEntities())
                {
                    string strDate = dateArret.ToString("yyy-M-dd");
                    string strYears = String.Join(",", years.ToArray());
                    string strCompanies = String.Join(",", companyList.ToArray());
                    strCompanies = strCompanies.Replace("'", "''");
                    string companiesForSql = "'" + strCompanies.Replace(",", "','") + "'";

                    string sql = $@"Select DP.AssureurName AS Assureur, DP.ContractId, DP.Company, YEAR(DP.DateSin) AS YearSurv, 
                        TP.CodeSinistre AS CodeGarantie, SUM(DP.Total) AS RNous
                        FROM DecomptePrev DP
                        INNER JOIN SinistrePrev SP on DP.Dossier = SP.Dossier AND DP.AssureurName = SP.AssureurName AND DP.DateSin = SP.DateSinistre AND DP.CauseSinistre = SP.NatureSinistre
                        INNER JOIN TypePrevoyance TP on TP.LabelSinistre = DP.CauseSinistre                         
                        WHERE YEAR(DP.DatePayement) in ({strYears}) AND DP.Company in ({companiesForSql}) AND DP.DatePayement <= '{strDate}' 
                        GROUP BY DP.AssureurName, DP.Company, YEAR(DP.DateSin), TP.CodeSinistre, DP.ContractId, DP.CauseSinistre
                        ORDER BY DP.Company, YEAR(DP.DateSin)";

                    decomptes = context.Database.SqlQuery<ExcelGlobalDecompteData>(sql)
                        .Select(d => new ExcelGlobalDecompteData
                        {
                            Assureur = d.Assureur,
                            Company = d.Company,
                            ContractId = d.ContractId,
                            DateArret = dateArret,
                            RNous = d.RNous,
                            YearSurv = d.YearSurv,
                            CodeGarantie = d.CodeGarantie

                        })
                        .ToList<ExcelGlobalDecompteData>();
                }

                return decomptes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalDecompteData> GetDecompteGlobalSubsidData(List<int> years, List<string> subsidList, DateTime dateArret)
        {
            try
            {
                List<ExcelGlobalDecompteData> decomptes = new List<ExcelGlobalDecompteData>();

                using (var context = new CompteResultatEntities())
                {
                    string strDate = dateArret.ToString("yyy-M-dd");
                    string strYears = String.Join(",", years.ToArray());
                    string strCompanies = String.Join(",", subsidList.ToArray());
                    strCompanies = strCompanies.Replace("'", "''");
                    string companiesForSql = "'" + strCompanies.Replace(",", "','") + "'";

                    string sql = $@"Select DP.AssureurName AS Assureur, DP.ContractId, DP.Company, DP.Subsid, YEAR(DP.DateSin) AS YearSurv, SUM(DP.Total) AS RNous
                        FROM DecomptePrev DP 
                        INNER JOIN SinistrePrev SP on DP.Dossier = SP.Dossier AND DP.AssureurName = SP.AssureurName AND DP.DateSin = SP.DateSinistre AND DP.CauseSinistre = SP.NatureSinistre                                               
                        WHERE YEAR(DP.DateSin) in ({strYears}) AND DP.Company in ({companiesForSql}) AND DP.DatePayement <= '{strDate}' 
                        GROUP BY DP.AssureurName, DP.Company, DP.Subsid, YEAR(DP.DateSin), DP.ContractId, DP.CauseSinistre
                        ORDER BY DP.Company, YEAR(DP.DateSin)";

                    decomptes = context.Database.SqlQuery<ExcelGlobalDecompteData>(sql)
                        .Select(d => new ExcelGlobalDecompteData
                        {
                            Assureur = d.Assureur,
                            Company = d.Company,
                            ContractId = d.ContractId,
                            Subsid = d.Subsid,
                            DateArret = dateArret,
                            RNous = d.RNous,
                            YearSurv = d.YearSurv

                        })
                        .ToList<ExcelGlobalDecompteData>();
                }

                return decomptes;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalDecompteData> GetDecompteGlobalSubsidDataCompta(List<int> years, List<string> subsidList, DateTime dateArret)
        {
            try
            {
                List<ExcelGlobalDecompteData> decomptes = new List<ExcelGlobalDecompteData>();

                using (var context = new CompteResultatEntities())
                {
                    string strDate = dateArret.ToString("yyy-M-dd");
                    string strYears = String.Join(",", years.ToArray());
                    string strCompanies = String.Join(",", subsidList.ToArray());
                    strCompanies = strCompanies.Replace("'", "''");
                    string companiesForSql = "'" + strCompanies.Replace(",", "','") + "'";

                    string sql = $@"Select DP.AssureurName AS Assureur, DP.ContractId, DP.Company, DP.Subsid, YEAR(DP.DateSin) AS YearSurv, SUM(DP.Total) AS RNous
                        FROM DecomptePrev DP 
                        INNER JOIN SinistrePrev SP on DP.Dossier = SP.Dossier AND DP.AssureurName = SP.AssureurName AND DP.DateSin = SP.DateSinistre AND DP.CauseSinistre = SP.NatureSinistre                                               
                        WHERE YEAR(DP.DatePayement) in ({strYears}) AND DP.Company in ({companiesForSql}) AND DP.DatePayement <= '{strDate}' 
                        GROUP BY DP.AssureurName, DP.Company, DP.Subsid, YEAR(DP.DateSin), DP.ContractId, DP.CauseSinistre
                        ORDER BY DP.Company, YEAR(DP.DateSin)";

                    decomptes = context.Database.SqlQuery<ExcelGlobalDecompteData>(sql)
                        .Select(d => new ExcelGlobalDecompteData
                        {
                            Assureur = d.Assureur,
                            Company = d.Company,
                            ContractId = d.ContractId,
                            Subsid = d.Subsid,
                            DateArret = dateArret,
                            RNous = d.RNous,
                            YearSurv = d.YearSurv

                        })
                        .ToList<ExcelGlobalDecompteData>();
                }

                return decomptes;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalDecompteData> GetDecompteGlobalSubsidDataWithGarantie(List<int> years, List<string> companyList, DateTime dateArret)
        {
            try
            {
                List<ExcelGlobalDecompteData> decomptes = new List<ExcelGlobalDecompteData>();

                using (var context = new CompteResultatEntities())
                {
                    string strDate = dateArret.ToString("yyy-M-dd");
                    string strYears = String.Join(",", years.ToArray());
                    string strCompanies = String.Join(",", companyList.ToArray());
                    strCompanies = strCompanies.Replace("'", "''");
                    string companiesForSql = "'" + strCompanies.Replace(",", "','") + "'";

                    string sql = $@"Select DP.AssureurName AS Assureur, DP.ContractId, DP.Company, DP.Subsid, YEAR(DP.DateSin) AS YearSurv, 
                        TP.CodeSinistre AS CodeGarantie, SUM(DP.Total) AS RNous
                        FROM DecomptePrev DP 
                        INNER JOIN SinistrePrev SP on DP.Dossier = SP.Dossier AND DP.AssureurName = SP.AssureurName AND DP.DateSin = SP.DateSinistre AND DP.CauseSinistre = SP.NatureSinistre
                        INNER JOIN TypePrevoyance TP on TP.LabelSinistre = DP.CauseSinistre                         
                        WHERE YEAR(DP.DateSin) in ({strYears}) AND DP.DatePayement <= '{strDate}'
                        GROUP BY DP.AssureurName, DP.Company, DP.Subsid, YEAR(DP.DateSin), TP.CodeSinistre, DP.ContractId, DP.CauseSinistre
                        ORDER BY DP.Company, DP.Subsid, YEAR(DP.DateSin)";

                    decomptes = context.Database.SqlQuery<ExcelGlobalDecompteData>(sql)
                        .Select(d => new ExcelGlobalDecompteData
                        {
                            Assureur = d.Assureur,
                            Company = d.Company,
                            ContractId = d.ContractId,
                            Subsid = d.Subsid,
                            DateArret = dateArret,
                            RNous = d.RNous,
                            YearSurv = d.YearSurv,
                            CodeGarantie = d.CodeGarantie

                        })
                        .ToList<ExcelGlobalDecompteData>();
                }

                return decomptes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalDecompteData> GetDecompteGlobalSubsidDataWithGarantieCompta(List<int> years, List<string> companyList, DateTime dateArret)
        {
            try
            {
                List<ExcelGlobalDecompteData> decomptes = new List<ExcelGlobalDecompteData>();

                using (var context = new CompteResultatEntities())
                {
                    string strDate = dateArret.ToString("yyy-M-dd");
                    string strYears = String.Join(",", years.ToArray());
                    string strCompanies = String.Join(",", companyList.ToArray());
                    strCompanies = strCompanies.Replace("'", "''");
                    string companiesForSql = "'" + strCompanies.Replace(",", "','") + "'";

                    string sql = $@"Select DP.AssureurName AS Assureur, DP.ContractId, DP.Company, DP.Subsid, YEAR(DP.DateSin) AS YearSurv, 
                        TP.CodeSinistre AS CodeGarantie, SUM(DP.Total) AS RNous
                        FROM DecomptePrev DP 
                        INNER JOIN SinistrePrev SP on DP.Dossier = SP.Dossier AND DP.AssureurName = SP.AssureurName AND DP.DateSin = SP.DateSinistre AND DP.CauseSinistre = SP.NatureSinistre
                        INNER JOIN TypePrevoyance TP on TP.LabelSinistre = DP.CauseSinistre                         
                        WHERE YEAR(DP.DatePayement) in ({strYears}) AND DP.DatePayement <= '{strDate}'
                        GROUP BY DP.AssureurName, DP.Company, DP.Subsid, YEAR(DP.DateSin), TP.CodeSinistre, DP.ContractId, DP.CauseSinistre
                        ORDER BY DP.Company, DP.Subsid, YEAR(DP.DateSin)";

                    decomptes = context.Database.SqlQuery<ExcelGlobalDecompteData>(sql)
                        .Select(d => new ExcelGlobalDecompteData
                        {
                            Assureur = d.Assureur,
                            Company = d.Company,
                            ContractId = d.ContractId,
                            Subsid = d.Subsid,
                            DateArret = dateArret,
                            RNous = d.RNous,
                            YearSurv = d.YearSurv,
                            CodeGarantie = d.CodeGarantie
                        })
                        .ToList<ExcelGlobalDecompteData>();
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
                            FinSin = dec.FinSin,
                            DateSin = dec.DateSin,
                            CauseSinistre = dec.CauseSinistre}).ToList();

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

        public static List<string> GetSinistreLabels()
        {
            try
            {
                List<string> sinistreLabels;

                using (var context = new CompteResultatEntities())
                {
                    sinistreLabels = context.Database
                            .SqlQuery<string>("SELECT DISTINCT CauseSinistre FROM dbo.DecomptePrev ORDER BY CauseSinistre")
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
