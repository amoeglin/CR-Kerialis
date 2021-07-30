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
    [MetadataType(typeof(AgeLabel.MetaData))]
    public partial class AgeLabel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<AgeLabel> GetAgeLabels()
        {
            try
            {
                List<AgeLabel> ageLabels;

                using (var context = new CompteResultatEntities())
                {
                    ageLabels = context.AgeLabels.OrderBy(a => a.Age).ToList();
                }

                if (ageLabels == null || ageLabels.Count == 0)
                    throw new Exception("The 'AgeLabel' entity does not contain any data!");

                return ageLabels;

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
