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
    [MetadataType(typeof(Cadencier.MetaData))]
    public partial class Cadencier
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //public static List<Cadencier> GetCadencierForAssureurId(int assureurId)
        //{
        //    try
        //    {
        //        List<Cadencier> cad = new List<Cadencier>();

        //        using (var context = new CompteResultatEntities())
        //        {
        //            var elements = context.Cadenciers.Where(c => c.AssureurId == assureurId).ToList();

        //            if (elements.Any())
        //                cad = elements.ToList();                   
        //        }

        //        return cad;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);
        //        throw ex;
        //    }
        //}

        public static List<Cadencier> GetCadencierForAssureur(string assurName)
        {
            try
            {
                List<Cadencier> cad;

                using (var context = new CompteResultatEntities())
                {
                    cad = context.Cadenciers.Where(c => c.AssureurName == assurName).ToList();
                }

                return cad;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void DeleteCadencierWithSpecificAssureurName(string assurName)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.Cadenciers.RemoveRange(context.Cadenciers.Where(c => c.AssureurName == assurName));
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void DeleteCadencierForSpecificYear(int year, string assName)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.Cadenciers.RemoveRange(context.Cadenciers.Where(c => c.Year == year && c.AssureurName == assName));
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int InsertCadencier(Cadencier cad)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.Cadenciers.Add(cad);
                    context.SaveChanges();

                    return cad.Id;
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
