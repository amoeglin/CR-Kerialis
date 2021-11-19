using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace CompteResultat.DAL
{
    [MetadataType(typeof(FraisPrevoyance.MetaData))]
    public partial class FraisPrevoyance
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<FraisPrevoyance> GetFraisPrevoyance()
        {
            try
            {
                List<FraisPrevoyance> fraisPrevoyance;

                using (var context = new CompteResultatEntities())
                {
                    fraisPrevoyance = context.FraisPrevoyances.OrderBy(p => p.AnneeSurvenance).ToList();
                }

                return fraisPrevoyance;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void DeleteFraisPrevoyance()
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.FraisPrevoyances.RemoveRange(context.FraisPrevoyances);
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
                    context.Database.ExecuteSqlCommand("Delete From FraisPrevoyance");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int InsertFraisPrevoyance(FraisPrevoyance fp)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.FraisPrevoyances.Add(fp);
                    context.SaveChanges();
                    return fp.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        public class MetaData
        {
            //[Display(Name = "Email address")]
            //[Required(ErrorMessage = "The email address is required")]
            //public string Email { get; set; }
        }
    }
}

