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
    [MetadataType(typeof(Demography.MetaData))]
    public partial class Demography
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Demography GetDemoById(int id)
        {
            try
            {
                Demography demo = null;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Demographies.Where(c => c.Id == id);

                    if (elements.Any())
                    {
                        demo = elements.First();
                    }
                }
                return demo;
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
                    //context.Demographies.RemoveRange(context.Demographies.Where(c => c.ImportId == importId));
                    //context.SaveChanges();

                    context.Database.ExecuteSqlCommand("DELETE FROM Demography WHERE ImportId = {0}", importId);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<CDemoData> GetDemoDataFromSP(string assurList, string parentCompanyList, string companyList, 
            string contractList, DateTime debutPeriode, DateTime finPeriode)
        {
            try
            {
                List<CDemoData> demoData;

                using (var context = new CompteResultatEntities())
                {
                    //### do we also need to select demo data by codecol
                    //companyList: "ACTI ALT2,ACTI ALT" => no space can be before or after the ,
                    demoData = context.SPGetDemoData(assurList, parentCompanyList, companyList, contractList, debutPeriode, finPeriode).ToList();
                }

                return demoData;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<CDemoDataWithoutOption> GetDemoDataWithoutOptionFromSP(string assurList, string parentCompanyList, string companyList, 
            string contractList, DateTime debutPeriode, DateTime finPeriode)
        {
            try
            {
                List<CDemoDataWithoutOption> demoData;

                using (var context = new CompteResultatEntities())
                {
                    demoData = context.SPGetDemoDataWithoutOption(assurList, parentCompanyList, companyList, contractList, debutPeriode, finPeriode).ToList();
                }

                return demoData;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<DemoSanteWithOptionInfo> GetDemoDataWithOptionInfoTrue(int importId)
        {
            try
            {
                List<DemoSanteWithOptionInfo> demoDatawithOption;

                using (var context = new CompteResultatEntities())
                {
                    demoDatawithOption = context.Demographies
                        .Where(d => d.ImportId == importId && d.WithOption.Trim().ToUpper() == "TRUE")
                       .Select(d => new DemoSanteWithOptionInfo
                       {
                           ContractId = d.ContractId,
                           //WithOption = d.WithOption.HasValue ? d.WithOption.Value : false 
                           WithOption = "TRUE"
                       }).ToList();
                    
                }

                return demoDatawithOption;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<NumberBenefs> GetNumberBenefs(string assur, string comp, int anneeSurv)
        {
            try
            {
                List<NumberBenefs> numberBenef = new List<NumberBenefs>();

                using (var context = new CompteResultatEntities())
                {
                    numberBenef = context.Demographies
                    .Where(d => d.DateDemo.Value.Year == anneeSurv && d.AssureurName == assur && d.Company == comp)
                    .GroupBy(p => new { p.Lien })
                    .Select(g => new NumberBenefs
                    {
                        Benef = g.Key.Lien,
                        Number = g.Count()
                    })
                    .OrderBy(gr => gr.Benef)
                    .ToList();
                }

                return numberBenef;

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
