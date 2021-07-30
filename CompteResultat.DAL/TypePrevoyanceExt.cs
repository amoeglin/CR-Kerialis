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
    [MetadataType(typeof(TypePrevoyance.MetaData))]
    public partial class TypePrevoyance
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<TypePrevoyance> GetTypePrev()
        {
            try
            {
                List<TypePrevoyance> typePrev;

                using (var context = new CompteResultatEntities())
                {
                    typePrev = context.TypePrevoyances.OrderBy(p => p.CodeSinistre).ToList();
                }

                //if (typePrev == null || typePrev.Count == 0)
                //    throw new Exception("The 'TypePrevoyance' entity does not contain any data!");

                return typePrev;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }        

        public static List<string> GetSinistreLabels()
        {
            try
            {
                List<string> sinistreLabels;

                using (var context = new CompteResultatEntities())
                {
                    sinistreLabels = context.Database
                            .SqlQuery<string>("SELECT DISTINCT LabelSinistre FROM dbo.TypePrevoyance ORDER BY LabelSinistre")
                            .ToList<string>();
                }

                return sinistreLabels;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void DeleteTypePrevoyance()
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.TypePrevoyances.RemoveRange(context.TypePrevoyances);
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
                    context.Database.ExecuteSqlCommand("Delete From TypePrevoyance");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int InsertTypePrev(TypePrevoyance tp)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.TypePrevoyances.Add(tp);
                    context.SaveChanges();
                    return tp.Id;
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

