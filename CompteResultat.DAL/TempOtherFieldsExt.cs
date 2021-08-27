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
    [MetadataType(typeof(C_TempOtherFields.MetaData))]
    public partial class C_TempOtherFields
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public static List<string> GetUniqueContractIds()
        {
            try
            {
                List<string> contractIds;

                using (var context = new CompteResultatEntities())
                {
                    contractIds = context.C_TempOtherFields.Where(c => c.ContractId != null && c.ContractId != "").Select(c => c.ContractId).Distinct().ToList();
                }

                return contractIds;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetUniqueParentCompanies()
        {
            try
            {
                List<string> comps;

                using (var context = new CompteResultatEntities())
                {
                    comps = context.C_TempOtherFields.Where(c => c.Company != null && c.Company != "").Select(c => c.Company).Distinct().ToList();
                }

                return comps;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetUniqueParentCompanyContractIdPairs()
        {
            try
            {
                List<string> comps;

                using (var context = new CompteResultatEntities())
                {
                    comps = context.C_TempOtherFields.Select(c => c.ContractId + C.cSTRINGSEP + c.Company).Distinct().ToList();
                }

                return comps;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetUniqueSubsids()
        {
            try
            {
                List<string> subs;

                using (var context = new CompteResultatEntities())
                {
                    subs = context.C_TempOtherFields.Where(c => c.Subsid != null && c.Subsid != "").Select(c => c.Subsid).Distinct().ToList();
                }

                return subs;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetAllSubsids()
        {
            try
            {
                List<string> subs;

                using (var context = new CompteResultatEntities())
                {
                    subs = context.C_TempOtherFields.Where(c => c.Subsid != null && c.Subsid != "").Select(c => c.Subsid).ToList();
                }

                return subs;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetUniqueSubsidContractIdPairs()
        {
            try
            {
                List<string> comps;

                using (var context = new CompteResultatEntities())
                {
                    comps = context.C_TempOtherFields.Select(c => c.ContractId + C.cSTRINGSEP + c.Subsid).Distinct().ToList();
                }

                return comps;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetUniqueAssNames()
        {
            try
            {
                List<string> assur;

                using (var context = new CompteResultatEntities())
                {
                    assur = context.C_TempOtherFields.Where(c=>c.Assureur != null && c.Assureur !="").Select(c => c.Assureur).Distinct().ToList();
                }

                return assur;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static string GetAssNamesForContract(string contractName)
        {
            try
            {
                string assName;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.C_TempOtherFields.Where(t => t.ContractId == contractName).Select(t => t.Assureur);

                    if (elements.Any())
                        assName = elements.Distinct().First();
                    else
                        assName = C.cINVALIDSTRING;
                }

                return assName;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetAllAssNamesForContract(string contrName)
        {
            try
            {
                List<string> assNames;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.C_TempOtherFields.Where(t => t.ContractId == contrName).Select(t => t.Assureur);

                    if (elements.Any())
                        assNames = elements.Distinct().ToList();
                    else
                        assNames = null;
                }

                return assNames;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetAllContractNamesForAssureur(string ass)
        {
            try
            {
                List<string> contrNames;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.C_TempOtherFields.Where(t => t.Assureur == ass).Select(t => t.ContractId);

                    if (elements.Any())
                        contrNames = elements.Distinct().ToList();
                    else
                        contrNames = null;
                }

                return contrNames;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetAllContractNamesForCompany(string companyName)
        {
            try
            {
                List<string> contrNames;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.C_TempOtherFields.Where(t => t.Company == companyName).Select(t => t.ContractId);

                    if (elements.Any())
                        contrNames = elements.Distinct().ToList();
                    else
                        contrNames = null;
                }

                return contrNames;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetAllContractNamesForSubsid(string subsidName)
        {
            try
            {
                List<string> contrNames;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.C_TempOtherFields.Where(t => t.Subsid == subsidName).Select(t => t.ContractId);

                    if (elements.Any())
                        contrNames = elements.Distinct().ToList();
                    else
                        contrNames = null;
                }

                return contrNames;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }
        
        public static List<string> GetAllParentCompForContract(string contrName)
        {
            try
            {
                List<string> compNames;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.C_TempOtherFields.Where(t => t.ContractId == contrName).Select(t => t.Company);

                    if (elements.Any())
                        compNames = elements.Distinct().ToList();
                    else
                        compNames = null;
                }

                return compNames;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<string> GetAllSubsidForContract(string contrName)
        {
            try
            {
                List<string> compNames;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.C_TempOtherFields.Where(t => t.ContractId == contrName).Select(t => t.Subsid);

                    if (elements.Any())
                        compNames = elements.Distinct().ToList();
                    else
                        compNames = null;
                }

                return compNames;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }
                
        public static List<string> GetParentCompanyNamesForSubsid(string subsid)
        {
            try
            {
                List<string> compNames;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.C_TempOtherFields.Where(t => t.Subsid == subsid).Select(t => t.Company);

                    if (elements.Any())
                        compNames = elements.ToList();
                    else
                        compNames = null;
                }

                return compNames;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<OtherTableAssurContrPair> GetAssurContrList()
        {
            try
            {
                List<OtherTableAssurContrPair> ACPairTempTable;

                using (var context = new CompteResultatEntities())
                {
                    //contractIds = context.C_TempOtherFields.Where(c => c.ContractId != null && c.ContractId != "").Select(c => c.ContractId).Distinct().ToList();
                    ACPairTempTable = context.Database
                            .SqlQuery<OtherTableAssurContrPair>("select Assureur, ContractId from dbo._TempOtherFields where Assureur Is Not Null And ContractId Is Not Null")
                            .ToList<OtherTableAssurContrPair>();
                }

                return ACPairTempTable;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<OtherTableContrCompPair> GetContrCompList()
        {
            try
            {
                List<OtherTableContrCompPair> CCPairTempTable;

                using (var context = new CompteResultatEntities())
                {
                    CCPairTempTable = context.Database
                            .SqlQuery<OtherTableContrCompPair>("select ContractId, Company from dbo._TempOtherFields where ContractId Is Not Null And Company Is Not Null")
                            .ToList<OtherTableContrCompPair>();
                }

                return CCPairTempTable;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<OtherTableContrSubsidPair> GetContrSubsidList()
        {
            try
            {
                List<OtherTableContrSubsidPair> CSPairTempTable;

                using (var context = new CompteResultatEntities())
                {
                    CSPairTempTable = context.Database
                            .SqlQuery<OtherTableContrSubsidPair>("select ContractId, Subsid from dbo._TempOtherFields where ContractId Is Not Null And Subsid Is Not Null")
                            .ToList<OtherTableContrSubsidPair>();
                }

                return CSPairTempTable;
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
                    //context.Database.ExecuteSqlCommand("DELETE FROM " + C.eDBTempTables._TempOtherFields.ToString() + " WHERE ImportId = " + importId);

                    context.Database.ExecuteSqlCommand("TRUNCATE TABLE " + C.eDBTempTables._TempOtherFields.ToString() );

                    //the following code works only if the table has a primary key
                    //context.C_TempOtherFields.RemoveRange(context.C_TempOtherFields.Where(a => a.ImportId == importId));
                    //context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }




        #region OLD FUNCTIONS

        //public static string GetContractNameForCompany(string companyName)
        //{
        //    try
        //    {
        //        string contrName;

        //        using (var context = new CompteResultatEntities())
        //        {
        //            var elements = context.C_TempOtherFields.Where(t => t.Company == companyName).Select(t => t.ContractId);

        //            if (elements.Any())
        //                contrName = elements.First();
        //            else
        //                contrName = C.cINVALIDSTRING;
        //        }

        //        return contrName;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        throw ex;
        //    }
        //}

        //public static string GetContractNameForSubsid(string subsidName)
        //{
        //    try
        //    {
        //        string contrName;

        //        using (var context = new CompteResultatEntities())
        //        {
        //            var elements = context.C_TempOtherFields.Where(t => t.Subsid == subsidName).Select(t => t.ContractId);

        //            if (elements.Any())
        //                contrName = elements.First();
        //            else
        //                contrName = C.cINVALIDSTRING;
        //        }

        //        return contrName;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        throw ex;
        //    }
        //}

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
