using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

using CompteResultat.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;

namespace CompteResultat.DAL
{
    [MetadataType(typeof(C_TempExpData.MetaData))]
    public partial class C_TempExpData
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<C_TempExpData> GetExpData(int yearStart, int yearEnd)
        {
            try
            {
                List<C_TempExpData> expData;

                using (var context = new CompteResultatEntities())
                {
                    expData = context.C_TempExpData.Where(e => e.AnneeExp >= yearStart && e.AnneeExp <= yearEnd).ToList();
                }

                return expData;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<C_TempExpData> GetExpDataForAssureur(string assurName)
        {
            try
            {
                List<C_TempExpData> expData;

                using (var context = new CompteResultatEntities())
                {
                    expData = context.C_TempExpData.Where(e => e.AssureurName == assurName).ToList();
                }

                return expData;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }
        
        public static void DeleteExpDataForYear(int year)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {                 
                    context.C_TempExpData.RemoveRange(context.C_TempExpData.Where(e => e.AnneeExp == year));
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void DeleteExperienceWithSpecificAssureurName(string assurName)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.C_TempExpData.RemoveRange(context.C_TempExpData.Where(c => c.AssureurName == assurName));
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void TruncateTable()
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.Database.ExecuteSqlCommand("TRUNCATE TABLE _TempExpData;");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int InsertExp(C_TempExpData exp)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.C_TempExpData.Add(exp);
                    context.SaveChanges();

                    return exp.Id;
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
