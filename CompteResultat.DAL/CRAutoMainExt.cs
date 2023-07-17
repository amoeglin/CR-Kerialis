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
    public partial class CRAutoMain
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<CRAutoMain> GetCRAutos()
        {
            return GetCRAutos("CreationDateTime", "ASC", "All");
        }

        public static List<CRAutoMain> GetCRAutos(string sortExpression, string sortDirection, string sortExpressionType)
        {
            try
            {
                List<CRAutoMain> crs;

                using (var context = new CompteResultatEntities())
                {
                    if (sortExpressionType == "SANTE")
                        crs = context.CRAutoMains.Where(i => i.ReporType == "SANTE").ToList();
                    else if (sortExpressionType == "PREV")
                        crs = context.CRAutoMains.Where(i => i.ReporType == "PREV").ToList();
                    else
                        crs = context.CRAutoMains.ToList();

                    if (sortExpression == "ListName")
                    {
                        if (sortDirection == "ASC")
                            crs = crs.OrderBy(i => i.ListName).ToList();
                        else
                            crs = crs.OrderByDescending(i => i.ListName).ToList();
                    }
                    else if (sortExpression == "ReporType")
                    {
                        if (sortDirection == "ASC")
                            crs = crs.OrderBy(i => i.ReporType).ToList();
                        else
                            crs = crs.OrderByDescending(i => i.ReporType).ToList();
                    }                    
                    else if (sortExpression == "UserName")
                    {
                        if (sortDirection == "ASC")
                            crs = crs.OrderBy(i => i.UserName).ToList();
                        else
                            crs = crs.OrderByDescending(i => i.UserName).ToList();
                    }
                    else if (sortExpression == "MainFolderName")
                    {
                        if (sortDirection == "ASC")
                            crs = crs.OrderBy(i => i.MainFolderName).ToList();
                        else
                            crs = crs.OrderByDescending(i => i.MainFolderName).ToList();
                    }
                    else if (sortExpression == "CreationDateTime")
                    {
                        if (sortDirection == "ASC")
                            crs = crs.OrderBy(i => i.CreationDateTime).ToList();
                        else
                            crs = crs.OrderByDescending(i => i.CreationDateTime).ToList();
                    }
                    else
                        crs = crs.OrderByDescending(i => i.CreationDateTime).ToList();
                }

                return crs;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        public static List<CRAutoMain> GetCRByName(string searchText)
        {
            try
            {
                List<CRAutoMain> crs;

                using (var context = new CompteResultatEntities())
                {
                    //.Include("CompteResult").Include("CRPlannings")
                    crs = context.CRAutoMains.Where(i => i.MainFolderName.ToLower().Contains(searchText.ToLower())).OrderByDescending(i => i.CreationDateTime).ToList();
                }

                return crs;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static CRAutoMain GetCRById(int id)
        {
            CRAutoMain myCR = null;

            try
            {
                using (var context = new CompteResultatEntities())
                {
                    var elements = context.CRAutoMains.Include("CompteResult").Include("CRPlannings").Where(c => c.Id == id);

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

        public static int Insert(CRAutoMain cr)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {                    
                    context.CRAutoMains.Add(cr);
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

        public static void Delete(int Id)
        {
            try
            {
                CRAutoMain myCR = null;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.CRAutoMains.Where(c => c.Id == Id);

                    if (elements.Any())
                    {
                        myCR = elements.First();
                        context.CRAutoMains.Attach(myCR);
                        context.CRAutoMains.Remove(myCR);
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
