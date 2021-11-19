using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace CompteResultat.DAL
{
    [MetadataType(typeof(FraisSante.MetaData))]
    public partial class FraisSante
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<FraisSante> GetFraisSante()
        {
            try
            {
                List<FraisSante> fraisSante;

                using (var context = new CompteResultatEntities())
                {
                    fraisSante = context.FraisSantes.OrderBy(p => p.AnneeSurvenance).ToList();
                }

                return fraisSante;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void DeleteFraisSante()
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.FraisSantes.RemoveRange(context.FraisSantes);
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
                    context.Database.ExecuteSqlCommand("Delete From FraisSante");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int InsertFraisSante(FraisSante fs)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.FraisSantes.Add(fs);
                    context.SaveChanges();
                    return fs.Id;
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

