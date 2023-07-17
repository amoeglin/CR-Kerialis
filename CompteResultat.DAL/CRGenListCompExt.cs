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
    [MetadataType(typeof(Import.MetaData))]
    public partial class CRGenListComp
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                
        
        public static List<CRGenListComp> GetByCRListId(int id)
        {
            try
            {
                List<CRGenListComp> lists;
                using (var context = new CompteResultatEntities())
                {
                    lists = context.CRGenListComps.Where(i => i.CRListId == id).ToList();
                }

                return lists;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static int Insert(CRGenListComp list)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {                    
                    context.CRGenListComps.Add(list);
                    context.SaveChanges();

                    return list.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void DeleteWithCRListId(int crListId)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    var elements = context.CRGenListComps.Where(i => i.CRListId == crListId);

                    if (elements.Any())
                    {
                        context.CRGenListComps.Remove(elements.First());
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
            //[Display(Name = "Import Name")]
            //[Required(ErrorMessage = "Please provide a name for the import!")]
            //public string Name { get; set; }

        }

    }
}
