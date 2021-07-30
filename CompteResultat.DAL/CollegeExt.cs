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
    [MetadataType(typeof(College.MetaData))]
    public partial class College
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<College> GetColleges()
        {
            try
            {
                List<College> coll;

                using (var context = new CompteResultatEntities())
                {
                    coll = context.Colleges.OrderBy(c => c.Name).ToList();
                }

                if (coll == null || coll.Count == 0)
                    throw new Exception("The 'Colleges' entity does not contain any data!");

                return coll;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static string GetCollegeNameForId(int id)
        {
            try
            {
                string name;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.Colleges.Where(c => c.Id == id).Select(c => c.Name);

                    if (elements.Any())
                        name = elements.First();
                    else
                        name = C.cINVALIDSTRING;
                }

                return name;
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
