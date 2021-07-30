using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;

namespace CompteResultat.DAL
{
    [MetadataType(typeof(CompteResult.MetaData))]
    public partial class CRPlanning
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<CRPlanning> GetCRPlannngForComptesResultat(int crId)
        {
            try
            {
                List<CRPlanning> crps;

                using (var context = new CompteResultatEntities())
                {
                    crps = context.CRPlannings.Where(c => c.CRId == crId).ToList();
                }

                return crps;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int Insert(CRPlanning crp)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.CRPlannings.Add(crp);
                    context.SaveChanges();

                    return crp.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void Update(CRPlanning newPl)
        {
            try
            {
                if (newPl == null)
                    throw new Exception("The 'CRPlanning' entity does not contain any data!");

                using (var context = new CompteResultatEntities())
                {
                    //CRPlanning oldPl = context.CRPlannings.Where(c => c.Id == newPl.Id).First();
                    var elements = context.CRPlannings.Where(c => c.Id == newPl.Id);

                    if (elements.Any())
                    {
                        CRPlanning oldPl = elements.First();

                        oldPl.DateArret = newPl.DateArret;
                        oldPl.DatePlanification = newPl.DatePlanification;
                        oldPl.DateTraitement = newPl.DateTraitement;
                        oldPl.DebutPeriode = newPl.DebutPeriode;
                        oldPl.FinPeriode = newPl.FinPeriode;

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
