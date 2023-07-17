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
    [MetadataType(typeof(CompteResult.MetaData))]
    public partial class CompteResult
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string AssurNames { get; set; }
        public string ContractNames { get; set; }
        public string CompanyNames { get; set; }
        public string SubsidNames { get; set; }

        public static List<CompteResult> GetComptesResultatForParentCompany(string companyId)
        {
            try
            {
                List<CompteResult> crs;

                using (var context = new CompteResultatEntities())
                {
                    crs = context.CompteResults.Include("CRPlannings").Where(c => c.CompanyIds == companyId).ToList();                    
                }

                return crs;
                
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<CompteResult> GetComptesResultatForAssur(string assurId)
        {
            try
            {
                List<CompteResult> crs;

                using (var context = new CompteResultatEntities())
                {
                    crs = context.CompteResults.Include("CRPlannings").Where(c => c.AssurIds == assurId).ToList();
                }

                return crs;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static CompteResult GetComptesResultatForId(int id)
        {
            CompteResult myCR = null;

            try
            {
                using (var context = new CompteResultatEntities())
                {
                    var elements = context.CompteResults.Where(c => c.Id == id);

                    if (elements.Any())
                    {
                        myCR = elements.First();
                    }
                }

                return myCR;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<CompteResult> GetComptesResultatByCRAutoId(int crAutoId)
        {
            List<CompteResult> myCRs = null;

            try
            {
                using (var context = new CompteResultatEntities())
                {
                    myCRs = context.CompteResults.Where(c => c.CRAutoId == crAutoId).ToList();
                }

                return myCRs;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int GetIdForCRNameAndParentComp(string crName, string parentCompId)
        {
            int crId = C.cINVALIDID;

            try
            {
                using (var context = new CompteResultatEntities())
                {
                    var elements = context.CompteResults.Where(c => (c.Name == crName && c.CompanyIds == parentCompId)).Select(C => C.Id);

                    if (elements.Any())
                        crId = elements.First();
                    else
                        crId =  C.cINVALIDID;
                }

                return crId;
            }
            catch (Exception ex)
            {
                log.Error("GetIdForCRNameAndParentComp :: " + ex.Message);
                throw ex;
            }
        }

        public static int GetIdForCRNameAndAssur(string crName, string assurId)
        {
            int crId = C.cINVALIDID;

            try
            {
                using (var context = new CompteResultatEntities())
                {
                    var elements = context.CompteResults.Where(c => (c.Name == crName && c.AssurIds == assurId)).Select(C => C.Id);

                    if (elements.Any())
                        crId = elements.First();
                    else
                        crId = C.cINVALIDID;
                }

                return crId;
            }
            catch (Exception ex)
            {
                log.Error("GetIdForCRNameAndAssur :: " + ex.Message);
                throw ex;
            }
        }

        //public static bool ComptesResultatExists(string crName, int parentCompId)
        //{
        //    //int crId = C.cINVALIDID;
        //    bool exists = false;

        //    try
        //    {
        //        using (var context = new CompteResultatEntities())
        //        {
        //            var elements = context.CompteResults.Where(c => (c.Name == crName && c.ParentCompanyId==parentCompId) );

        //            if (elements.Any())
        //            {
        //                //crId = elements.First().Id;
        //                exists = true;
        //            }
        //        }

        //        return exists;
        //        //return crId;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        throw ex;
        //    }
        //}

        public static int Insert(CompteResult cr)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {                    
                    context.CompteResults.Add(cr);
                    context.SaveChanges();

                    return cr.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error("CompteResultatExt - Insert ::" + ex.Message);
                throw ex;
            }
        }

        public static void Update(CompteResult newCR)
        {
            try
            {
                if (newCR == null)
                    throw new Exception("The 'CompteResult' entity does not contain any data!");

                using (var context = new CompteResultatEntities())
                {
                    //CompteResult oldCR = context.CompteResults.Where(c => c.Id == newCR.Id).First();
                    var elements = context.CompteResults.Where(c => c.Id == newCR.Id);

                    if (elements.Any())
                    {
                        CompteResult oldCR = elements.First();

                        oldCR.Name = newCR.Name;                        
                        oldCR.AssurIds = newCR.AssurIds;
                        oldCR.CompanyIds = newCR.CompanyIds;
                        oldCR.SubsidIds = newCR.SubsidIds;
                        oldCR.ContractIds = newCR.ContractIds;
                        oldCR.ReportLevelId = newCR.ReportLevelId;
                        oldCR.CollegeId = newCR.CollegeId;
                        oldCR.UserName = newCR.UserName;
                        oldCR.CreationDate = newCR.CreationDate;
                        oldCR.IsActive = newCR.IsActive;
                        oldCR.IsAutoGenerated = newCR.IsAutoGenerated;
                        oldCR.TaxActif = newCR.TaxActif;
                        oldCR.TaxDefault = newCR.TaxDefault;
                        oldCR.TaxPerif = newCR.TaxPerif;
                        oldCR.ReportType = newCR.ReportType;

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("CompteResultatExt - Update :: " + ex.Message);
                throw ex;
            }
        }

        public static void Delete(CompteResult cr)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.CompteResults.Attach(cr);
                    context.CompteResults.Remove(cr);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void Delete(int Id)
        {
            try
            {
                CompteResult myCR = null;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.CompteResults.Where(c => c.Id == Id);

                    if (elements.Any())
                    {
                        myCR = elements.First();
                        context.CompteResults.Attach(myCR);
                        context.CompteResults.Remove(myCR);
                        context.SaveChanges();
                    }
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
