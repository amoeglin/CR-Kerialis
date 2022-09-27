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
    [MetadataType(typeof(PrestSante.MetaData))]
    public partial class PrestSante
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<CumulPresta> CumulPrestaData()
        {
            try
            {
                List<CumulPresta> prestations = new List<CumulPresta>();

                using (var context = new CompteResultatEntities())
                {
                    string sql = @"select AssureurName, year(DateSoins) as AnneeSoins, (year(DatePayment)-year(DateSoins))*12 + month(DatePayment) as MoisReglement, 
                        sum(RembNous) as SommePresta
                        from [CompteResultat].[dbo].[PrestSante]
                        group by AssureurName, year(DateSoins), (year(DatePayment)-year(DateSoins))*12 + month(DatePayment) 
                        order by year(DateSoins), (year(DatePayment)-year(DateSoins))*12 + month(DatePayment)";
                    
                    prestations = context.Database.SqlQuery<CumulPresta>(sql).ToList<CumulPresta>();                   
                }

                return prestations;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        //This method is in Demography
        public static List<NumberBenefs> GetNumberBenefs_TEST(string assur, string comp, int anneeSurv)
        {
            try
            {
                List<NumberBenefs> cotisat = new List<NumberBenefs>();

                using (var context = new CompteResultatEntities())
                {
                    cotisat = context.PrestSantes
                    .Where(d => d.DateSoins.Value.Year == anneeSurv && d.AssureurName == assur && d.Company == comp)
                    .GroupBy(p => new { p.Beneficiaire })
                    .Select(g => new NumberBenefs
                    {
                        Benef = g.Key.Beneficiaire,
                        Number = g.Count()
                    })
                    .OrderBy(gr => gr.Benef)
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

        public static string GetTopGroupNameForCompany(string comp, int anneeSurv)
        {
            try
            {
                string topGroup = "";

                using (var context = new CompteResultatEntities())
                {
                    var res = context.PrestSantes
                    .Where(d => d.DateSoins.Value.Year == anneeSurv && d.Company == comp)
                    .GroupBy(p => new { p.GroupName })
                    .Select(g => new TopGroupNames
                    {
                        GroupName = g.Key.GroupName,
                        TotalRembNous = g.Sum(i => i.RembNous)
                    })
                    .OrderByDescending(gr => gr.TotalRembNous)
                    .ToList();

                    if(res.Any())
                    {
                        topGroup = res[0].GroupName + " : " + Math.Round(res[0].TotalRembNous.Value,0) + " €";
                    }
                }

                return topGroup;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        //This method nees to be in CotsatSante
        public static string GetAllContractsForCompany_TEST(ref int numbProd, string assur, string company, int anneeSurv)
        {
            try
            {
                string allContr = "";
                numbProd = 1;

                using (var context = new CompteResultatEntities())
                {
                    var res = context.PrestSantes
                        .Where(d => d.AssureurName == assur && d.Company == company && d.DateSoins.Value.Year == anneeSurv)
                        .Select(e => e.ContractId)
                        .Distinct()
                        .ToList();

                    if(res.Any())
                    {
                        allContr = String.Join("_", res);
                        numbProd = res.Count();
                    }
                }

                return allContr;
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalPrestaData> GetPrestaGlobalEntData(List<int> years, List<string> companyList, DateTime dateArret)
        {
            try
            {
                List<ExcelGlobalPrestaData> prestations = new List<ExcelGlobalPrestaData>();

                using (var context = new CompteResultatEntities())
                {
                    prestations = context.PrestSantes
                    .Where(d => years.Contains(d.DateSoins.Value.Year) && companyList.Contains(d.Company) && d.DatePayment <= dateArret)
                    .GroupBy(p => new { p.AssureurName, p.Company, AnnSurv = p.DateSoins.Value.Year, p.ContractId })
                    .Select(g => new ExcelGlobalPrestaData
                    {
                        Assureur = g.Key.AssureurName,
                        Contract = g.Key.ContractId,
                        Company = g.Key.Company,
                        Subsid = "",
                        YearSurv = g.Key.AnnSurv,
                        FR = g.Sum(i => i.FraisReel),
                        RSS = g.Sum(i => i.RembSS),
                        RAnnexe = g.Sum(i => i.RembAnnexe),
                        RNous = g.Sum(i => i.RembNous),
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

                return prestations;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalPrestaData> GetPrestaGlobalEntDataCompta(List<int> years, List<string> companyList, DateTime dateArret)
        {
            try
            {
                List<ExcelGlobalPrestaData> prestations = new List<ExcelGlobalPrestaData>();

                using (var context = new CompteResultatEntities())
                {
                    prestations = context.PrestSantes
                    .Where(d => years.Contains(d.DatePayment.Value.Year) && companyList.Contains(d.Company) && d.DatePayment <= dateArret)
                    .GroupBy(p => new { p.AssureurName, p.Company, AnnPaiement = p.DatePayment.Value.Year, p.ContractId })
                    .Select(g => new ExcelGlobalPrestaData
                    {
                        Assureur = g.Key.AssureurName,
                        Contract = g.Key.ContractId,
                        Company = g.Key.Company,
                        Subsid = "",
                        YearSurv = g.Key.AnnPaiement,
                        FR = g.Sum(i => i.FraisReel),
                        RSS = g.Sum(i => i.RembSS),
                        RAnnexe = g.Sum(i => i.RembAnnexe),
                        RNous = g.Sum(i => i.RembNous),
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

                return prestations;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalPrestaData> GetPrestaGlobalSubsidData(List<int> years, List<string> subsidList, DateTime dateArret)
        {
            try
            {
                List<ExcelGlobalPrestaData> prestations = new List<ExcelGlobalPrestaData>();

                using (var context = new CompteResultatEntities())
                {
                    prestations = context.PrestSantes
                    .Where(d => years.Contains(d.DateSoins.Value.Year) && subsidList.Contains(d.Company) && d.DatePayment <= dateArret)
                    .GroupBy(p => new { p.AssureurName, p.Company, p.Subsid, AnnSurv = p.DateSoins.Value.Year, p.ContractId })
                    .Select(g => new ExcelGlobalPrestaData
                    {
                        Assureur = g.Key.AssureurName,
                        Contract = g.Key.ContractId,
                        Company = g.Key.Company,
                        Subsid = g.Key.Subsid,
                        YearSurv = g.Key.AnnSurv,
                        FR = g.Sum(i => i.FraisReel),
                        RSS = g.Sum(i => i.RembSS),
                        RAnnexe = g.Sum(i => i.RembAnnexe),
                        RNous = g.Sum(i => i.RembNous),
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

                return prestations;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelGlobalPrestaData> GetPrestaGlobalSubsidDataCompta(List<int> years, List<string> subsidList, DateTime dateArret)
        {
            try
            {
                List<ExcelGlobalPrestaData> prestations = new List<ExcelGlobalPrestaData>();

                using (var context = new CompteResultatEntities())
                {
                    prestations = context.PrestSantes
                    .Where(d => years.Contains(d.DatePayment.Value.Year) && subsidList.Contains(d.Company) && d.DatePayment <= dateArret)
                    .GroupBy(p => new { p.AssureurName, p.Company, p.Subsid, AnneePaiement = p.DatePayment.Value.Year, p.ContractId })
                    .Select(g => new ExcelGlobalPrestaData
                    {
                        Assureur = g.Key.AssureurName,
                        Contract = g.Key.ContractId,
                        Company = g.Key.Company,
                        Subsid = g.Key.Subsid,
                        YearSurv = g.Key.AnneePaiement,
                        FR = g.Sum(i => i.FraisReel),
                        RSS = g.Sum(i => i.RembSS),
                        RAnnexe = g.Sum(i => i.RembAnnexe),
                        RNous = g.Sum(i => i.RembNous),
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

                return prestations;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static long TotalPrestaCount()
        {
            long prestaCount = 0;
            using (var context = new CompteResultatEntities())
            {
                prestaCount = context.PrestSantes.Count();
            }

            return prestaCount;
        }

        public static List<PrestSante> GetPrestationsPaged(long offset, int numberRows)
        {
            try
            {
                List<PrestSante> prestations;
       
                using (var context = new CompteResultatEntities())
                {
                    string sql = "SELECT * FROM PrestSante ORDER BY Id OFFSET " + offset + " ROWS FETCH NEXT " + numberRows + " ROWS ONLY;";
                    prestations = context.PrestSantes.SqlQuery(sql).ToList<PrestSante>();
                    //prestations = context.PrestSantes.OrderBy(p => p.Id).Skip(offset).Take(numberRows).ToList();                    
                }

                return prestations;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<PrestSante> GetPrestationsForContracts(List<string> assurList, List<string> parentCompanyList, List<string> companyList, 
            List<string> contrIds, string college, DateTime debutPeriod, DateTime finPeriod, DateTime dateArret)
        {
            try
            {
                List<PrestSante> prestations;

                using (var context = new CompteResultatEntities())
                {                    
                    prestations = context.PrestSantes.Where(prest => assurList.Contains(prest.AssureurName) && parentCompanyList.Contains(prest.Company)
                        && companyList.Contains(prest.Subsid) && contrIds.Contains(prest.ContractId)
                        && prest.DateSoins >= debutPeriod && prest.DateSoins <= finPeriod && prest.DatePayment <= dateArret).ToList();

                    //prestations = context.PrestSantes.Where(prest => contrIds.Contains(prest.ContractId) 
                    //   && prest.DateSoins >= debutPeriod && prest.DateSoins <= finPeriod && prest.DatePayment <= dateArret).ToList();

                    //var x = prestations
                    //       .Where(prest => contrIds.Contains(prest.ContractId) && prest.DateSoins >= debutPeriod && prest.DateSoins <= finPeriod)
                    //       .GroupBy(p => new { p.DateVision, p.ContractId, p.CodeCol, DateSoinsYear = p.DateSoins.Value.Year, p.CodeActe, p.CAS, p.Reseau })
                    //       .Select(p => new
                    //       {
                    //           DateVision = p.Key.DateVision,
                    //           ContractId = p.Key.ContractId,
                    //           CodeCol = p.Key.CodeCol,
                    //           DateSoins = new DateTime(p.Key.DateSoinsYear, 1, 1),
                    //           CodeActe = p.Key.CodeActe,
                    //           CAS = p.Key.CAS,
                    //           NombreActe = p.Sum(pr => pr.NombreActe),
                    //           FraisReel = p.Sum(pr => pr.FraisReel),
                    //           RembSS = p.Sum(pr => pr.RembSS),
                    //           RembAnnexe = p.Sum(pr => pr.RembAnnexe),
                    //           RembNous = p.Sum(pr => pr.RembNous),
                    //           Reseau = p.Key.Reseau
                    //       })
                    //       .ToList();


                    //taking into account College:
                    //prestations = context.PrestSantes.Where(prest => contrIds.Contains(prest.ContractId) && prest.CodeCol == college
                    //    && prest.DateSoins >= debutPeriod && prest.DateSoins <= finPeriod).ToList();

                    //cotisat2 = context.CotisatSantes.Where(cot => contrIds.Contains(cot.ContractId)).
                    //    Select(cot => new { cot.DebPrime, cot.FinPrime, cot.ContractId, cot.CodeCol, cot.Year, cot.Cotisation }).ToList();
                }

                return prestations;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<PrestSante> GetPrestationsForContractsCompta(List<string> assurList, List<string> parentCompanyList, List<string> companyList,
            List<string> contrIds, string college, DateTime debutPeriod, DateTime finPeriod, DateTime dateArret)
        {
            try
            {
                List<PrestSante> prestations;

                using (var context = new CompteResultatEntities())
                {
                    prestations = context.PrestSantes.Where(prest => assurList.Contains(prest.AssureurName) && parentCompanyList.Contains(prest.Company)
                        && companyList.Contains(prest.Subsid) && contrIds.Contains(prest.ContractId)
                        && prest.DatePayment >= debutPeriod && prest.DatePayment <= finPeriod && prest.DatePayment <= dateArret).ToList();
                }

                return prestations;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<PrestSante> GetPrestations()
        {
            try
            {
                List<PrestSante> prestations;

                using (var context = new CompteResultatEntities())
                {
                    prestations = context.PrestSantes.ToList();
                }

                return prestations;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<PrestSanteContrIdCount> GetContractIdCount()
        {
            try
            {
                List<PrestSanteContrIdCount> contrCount;

                using (var context = new CompteResultatEntities())
                {
                    contrCount = context.PrestSantes
                        .GroupBy(p => new { p.ContractId })
                        .Select(p => new PrestSanteContrIdCount
                        {
                            ContractId = p.Key.ContractId,
                            Count = p.Count()
                        })
                        .OrderBy(gr => gr.Count)
                        .ToList();
                }

                return contrCount;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelPrestaSheet> GetDataSmallGroup()
        {
            try
            {
                List<ExcelPrestaSheet> excelPrest = new List<ExcelPrestaSheet>();

                string sql = @"SELECT 
	                        AssureurName,
	                        ContractId = 'XXXXX', 
	                        CodeCol = 'XXXXX',  
	                        YEAR(DateSoins) AS AnneeDateSoins,                             
	                        GroupName, 
	                        GarantyName, 
	                        CAS,	 	
	                        SUM(NombreActe) AS NombreActe,
	                        SUM(FraisReel) AS FraisReel,
	                        SUM(RembSS) AS RembSS,
	                        SUM(RembAnnexe) AS RembAnnexe,
	                        SUM(RembNous) AS RembNous,
	                        Reseau,
                            MIN(FraisReel / NombreActe) AS MinFR,
                            MAX(FraisReel / NombreActe) AS MaxFR,
                            MIN(RembNous / NombreActe) AS MinNous,
                            MAX(RembNous / NombreActe) AS MaxNous
                            FROM PrestSante
                            GROUP BY AssureurName, YEAR(DateSoins), GroupName, GarantyName, CAS, Reseau
                            ORDER BY GroupName, GarantyName";

                using (var context = new CompteResultatEntities())
                {
                    excelPrest = context.Database.SqlQuery<ExcelPrestaSheet>(sql).ToList();
                }

                return excelPrest;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " --- " + ex.InnerException);
                throw ex;
            }
        }

        public static List<ExcelPrestaSheet> GetDataLargeGroup()
        {
            try
            {
                List<ExcelPrestaSheet> excelPrest = new List<ExcelPrestaSheet>();

                string sql = @"SELECT 
	                        AssureurName,
                            DateVision,
	                        ContractId, 
	                        CodeCol,  
	                        YEAR(DateSoins) AS AnneeDateSoins,                             
	                        GroupName, 
	                        GarantyName, 
	                        CAS,	 	
	                        SUM(NombreActe) AS NombreActe,
	                        SUM(FraisReel) AS FraisReel,
	                        SUM(RembSS) AS RembSS,
	                        SUM(RembAnnexe) AS RembAnnexe,
	                        SUM(RembNous) AS RembNous,
	                        Reseau,
                            MIN(FraisReel / NombreActe) AS MinFR,
                            MAX(FraisReel / NombreActe) AS MaxFR,
                            MIN(RembNous / NombreActe) AS MinNous,
                            MAX(RembNous / NombreActe) AS MaxNous
                            FROM PrestSante
                            GROUP BY AssureurName, DateVision, ContractId, CodeCol, YEAR(DateSoins), GroupName, GarantyName, CAS, Reseau
                            ORDER BY GroupName, GarantyName";                

                using (var context = new CompteResultatEntities())
                {
                    excelPrest = context.Database.SqlQuery<ExcelPrestaSheet>(sql).ToList();
                }

                return excelPrest;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message + " --- " + ex.InnerException);
                throw ex;
            }
        }        

        public static List<GroupesGarantiesSante> GetGroupGarantyList()
        {
            try
            {
                List<GroupesGarantiesSante> GroupGarantyTable;

                using (var context = new CompteResultatEntities())
                {
                    GroupGarantyTable = context.Database
                            .SqlQuery<GroupesGarantiesSante>("SELECT DISTINCT AssureurName,GroupName,GarantyName,CodeActe,OrderNumber=1 FROM dbo.PrestSante ORDER BY AssureurName,GroupName,GarantyName,CodeActe")
                            .ToList<GroupesGarantiesSante>();
                }

                return GroupGarantyTable;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<GroupesGarantiesSante> GetExperienceData()
        {
            try
            {
                List<GroupesGarantiesSante> GroupGarantyTable;

                using (var context = new CompteResultatEntities())
                {
                    GroupGarantyTable = context.Database
                            .SqlQuery<GroupesGarantiesSante>("SELECT DISTINCT AssureurName,GroupName,GarantyName,CodeActe,OrderNumber=1 FROM dbo.PrestSante ORDER BY AssureurName,GroupName,GarantyName,CodeActe")
                            .ToList<GroupesGarantiesSante>();
                }

                return GroupGarantyTable;
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
                    //context.PrestSantes.RemoveRange(context.PrestSantes.Where(c => c.ImportId == importId));
                    //context.SaveChanges();

                    context.Database.ExecuteSqlCommand("DELETE FROM PrestSante WHERE ImportId = {0}", importId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void UpdateGroupGaranty(int id, string groupName, string garantyName)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    var sql = "UPDATE PrestSante SET GroupName = {0}, GarantyName = {1} WHERE Id = {2}";
                    context.Database.ExecuteSqlCommand(sql, groupName, garantyName, id);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void UpdateOptionField(int importId, string contractId)
        {
            try
            {
                
                using (var context = new CompteResultatEntities())
                {

                    context.Database.ExecuteSqlCommand("update PrestSante set WithOption = 'True' where ContractId = '" + contractId + "' and ImportId = " + importId);

                    //var elements = context.PrestSantes.Where(p => p.ContractId == contractId && p.ImportId == importId);

                    //if (elements.Any())
                    //{
                    //    PrestSante prest = elements.First();
                    //    prest.WithOption = "True";
                    //    context.SaveChanges();
                    //}
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void UpdateWithOptionField(string sql)
        {
            try
            {

                using (var context = new CompteResultatEntities())
                {
                    context.Database.ExecuteSqlCommand(sql);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }



        #region OLD_METHODS

        public static List<ExcelPrestaSheet> GetDataSmallGroup_OLD()
        {
            try
            {
                List<ExcelPrestaSheet> excelPrest;

                using (var context = new CompteResultatEntities())
                {

                    excelPrest = context.PrestSantes
                        .GroupBy(p => new {
                            p.DateSoins.Value.Year,
                            p.GroupName,
                            p.GarantyName,
                            p.CAS,
                            p.Reseau
                        })
                        .Select(p => new ExcelPrestaSheet
                        {
                            AssureurName = p.Select(pr => pr.AssureurName).FirstOrDefault(),
                            DateVision = new DateTime(1900, 01, 01),
                            ContractId = "XXXXX",
                            CodeCol = "XXXXX",
                            AnneeDateSoins = p.Key.Year,
                            DateSoins = new DateTime(1900, 1, 1),
                            //DateSoins = new DateTime(p.Key.Year, 1, 1),
                            GroupName = p.Key.GroupName,
                            GarantyName = p.Key.GarantyName,
                            CAS = p.Key.CAS.ToLower() == "true" ? "VRAI" : "FAUX",
                            NombreActe = p.Sum(pr => pr.NombreActe),
                            FraisReel = p.Sum(pr => pr.FraisReel),
                            RembSS = p.Sum(pr => pr.RembSS),
                            RembAnnexe = p.Sum(pr => pr.RembAnnexe),
                            RembNous = p.Sum(pr => pr.RembNous),
                            Reseau = String.IsNullOrEmpty(p.Key.Reseau) ? "FAUX" : "VRAI",
                            MinFR = p.Where(pr => pr.FraisReel >= 0).Min(pr => pr.FraisReel / pr.NombreActe),
                            MaxFR = p.Where(pr => pr.FraisReel >= 0).Max(pr => pr.FraisReel / pr.NombreActe),
                            MinNous = p.Where(pr => pr.RembNous >= 0).Min(pr => pr.RembNous / pr.NombreActe),
                            MaxNous = p.Where(pr => pr.RembNous >= 0).Max(pr => pr.RembNous / pr.NombreActe)
                        })
                           .OrderBy(gr => gr.GroupName).ThenBy(ga => ga.GarantyName)
                           .ToList();
                }

                return excelPrest;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ExcelPrestaSheet> GetDataLargeGroup_OLD()
        {
            try
            {
                List<ExcelPrestaSheet> excelPrest;

                using (var context = new CompteResultatEntities())
                {
                    excelPrest = context.PrestSantes
                        .GroupBy(p => new
                        {
                            p.DateVision,
                            p.ContractId,
                            p.CodeCol,
                            p.DateSoins.Value.Year,
                            p.GroupName,
                            p.GarantyName,
                            p.CAS,
                            p.Reseau
                        })
                        .Select(p => new ExcelPrestaSheet
                        {
                            AssureurName = p.Select(pr => pr.AssureurName).FirstOrDefault(),
                            DateVision = p.Key.DateVision,
                            ContractId = p.Key.ContractId,
                            CodeCol = p.Key.CodeCol,
                            AnneeDateSoins = p.Key.Year,
                            DateSoins = new DateTime(1900, 1, 1),
                            GroupName = p.Key.GroupName,
                            GarantyName = p.Key.GarantyName,
                            CAS = p.Key.CAS.ToLower() == "true" ? "VRAI" : "FAUX",
                            NombreActe = p.Sum(pr => pr.NombreActe),
                            FraisReel = p.Sum(pr => pr.FraisReel),
                            RembSS = p.Sum(pr => pr.RembSS),
                            RembAnnexe = p.Sum(pr => pr.RembAnnexe),
                            RembNous = p.Sum(pr => pr.RembNous),
                            Reseau = String.IsNullOrEmpty(p.Key.Reseau) ? "FAUX" : "VRAI",
                            MinFR = p.Where(pr => pr.FraisReel >= 0).Min(pr => pr.FraisReel / pr.NombreActe),
                            MaxFR = p.Where(pr => pr.FraisReel >= 0).Max(pr => pr.FraisReel / pr.NombreActe),
                            MinNous = p.Where(pr => pr.RembNous >= 0).Min(pr => pr.RembNous / pr.NombreActe),
                            MaxNous = p.Where(pr => pr.RembNous >= 0).Max(pr => pr.RembNous / pr.NombreActe)
                        })
                        .OrderBy(gr => gr.GroupName).ThenBy(ga => ga.GarantyName)
                        .ToList();
                }

                return excelPrest;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        #endregion


        //MetaData definition for basic validation
        public class MetaData
        {
            //[Display(Name = "Email address")]
            //[Required(ErrorMessage = "The email address is required")]
            //public string Email { get; set; }

        }
    }
}
