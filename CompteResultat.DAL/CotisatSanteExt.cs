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
    [MetadataType(typeof(CotisatSante.MetaData))]
    public partial class CotisatSante
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<ExcelGlobalCotisatData> GetCotisatGlobalEntData(List<int> years, List<string> companyList)
        {
            try
            {
                List<ExcelGlobalCotisatData> cotisat = new List<ExcelGlobalCotisatData>();

                using (var context = new CompteResultatEntities())
                {
                    cotisat = context.CotisatSantes
                    .Where(d => years.Contains((d.Year.HasValue ? d.Year.Value : 0)) && companyList.Contains(d.Company))
                    .GroupBy(p => new { p.AssureurName, p.Company, AnnSurv = p.Year, Contract =p.ContractId })
                    .Select(g => new ExcelGlobalCotisatData
                    {
                        Assureur = g.Key.AssureurName,
                        ContractId = g.Key.Contract,
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

        public static List<ExcelGlobalCotisatData> GetCotisatGlobalSubsidData(List<int> years, List<string> subsidList)
        {
            try
            {
                List<ExcelGlobalCotisatData> cotisat = new List<ExcelGlobalCotisatData>();

                using (var context = new CompteResultatEntities())
                {
                    cotisat = context.CotisatSantes
                    .Where(d => years.Contains((d.Year.HasValue ? d.Year.Value : 0)) && subsidList.Contains(d.Subsid))
                    .GroupBy(p => new { p.AssureurName, p.Company, p.Subsid, AnnSurv = p.Year, Contract = p.ContractId })
                    .Select(g => new ExcelGlobalCotisatData
                    {
                        Assureur = g.Key.AssureurName,
                        ContractId = g.Key.Contract,
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

        public static List<CotisatSante> GetCotisationsForContracts(List<string> assurList, List<string> parentCompanyList, List<string> companyList, 
            List<string> contrIds, string college, DateTime debutPeriod, DateTime finPeriod)
        {
            try
            {
                List<CotisatSante> cotisat;

                using (var context = new CompteResultatEntities())
                {
                    cotisat = context.CotisatSantes.Where(cot => assurList.Contains(cot.AssureurName) && parentCompanyList.Contains(cot.Company) 
                        && companyList.Contains(cot.Subsid) && contrIds.Contains(cot.ContractId)
                        && cot.DebPrime >= debutPeriod && cot.FinPrime <= finPeriod).ToList();

                    //cotisat = context.CotisatSantes.Where(cot => contrIds.Contains(cot.ContractId) 
                    //    && cot.DebPrime >= debutPeriod && cot.FinPrime <= finPeriod).ToList();

                    //old: here we also search for college                   
                    //cotisat = context.CotisatSantes.Where(cot => contrIds.Contains(cot.ContractId) && cot.CodeCol == college 
                    //    && cot.DebPrime >= debutPeriod && cot.FinPrime <= finPeriod).ToList();

                    //cotisat2 = context.CotisatSantes.Where(cot => contrIds.Contains(cot.ContractId)).
                    //    Select(cot => new { cot.DebPrime, cot.FinPrime, cot.ContractId, cot.CodeCol, cot.Year, cot.Cotisation }).ToList();
                }

                return cotisat;

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
                    context.Database.ExecuteSqlCommand("update CotisatSante set WithOption = 'True' where ContractId = '" + contractId + "' and ImportId = " + importId);

                    //var elements = context.CotisatSantes.Where(p => p.ContractId == contractId && p.ImportId == importId);

                    //if (elements.Any())
                    //{
                    //    CotisatSante cotis = elements.First();
                    //    cotis.WithOption = "True";
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

        public static void DeleteRowsWithImportId(int importId)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    //context.CotisatSantes.RemoveRange(context.CotisatSantes.Where(c => c.ImportId == importId));
                    //context.SaveChanges();

                    context.Database.ExecuteSqlCommand("DELETE FROM CotisatSante WHERE ImportId = {0}", importId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static string GetAllContractsForCompany(ref int numbProd, string assur, string company, int anneeSurv)
        {
            try
            {
                string allContr = "";
                numbProd = 1;

                using (var context = new CompteResultatEntities())
                {
                    var res = context.CotisatSantes
                        .Where(d => d.AssureurName == assur && d.Company == company && d.Year == anneeSurv)
                        .Select(e => e.ContractId)
                        .Distinct()
                        .ToList();

                    if (res.Any())
                    {
                        allContr = String.Join("_", res);
                        numbProd = res.Count();
                    }
                }

                return allContr;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int GetNumberEntreprise (string assur, string contract, int anneeSurv)
        {
            try
            {
                int numbEntreprise = 1;

                using (var context = new CompteResultatEntities())
                {
                    var res = context.CotisatSantes
                        .Where(d => d.AssureurName == assur && d.ContractId == contract && d.Year == anneeSurv)
                        .Select(e => e.Company)
                        .Distinct()
                        .ToList();

                    if (res.Any())
                    {
                        numbEntreprise = res.Count();
                    }
                }

                return numbEntreprise;
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
