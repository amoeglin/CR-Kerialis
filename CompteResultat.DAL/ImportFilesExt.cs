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
    [MetadataType(typeof(ImportFile.MetaData))]
    public partial class ImportFile
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<ImportFile> GetImportFiles()
        {
            try
            {
                List<ImportFile> impFiles;

                using (var context = new CompteResultatEntities())
                {
                    impFiles = context.ImportFiles.OrderBy(c => c.FileGroup).ToList();
                }

                if (impFiles == null || impFiles.Count == 0)
                    throw new Exception("The 'ImportFiles' entity does not contain any data!");

                return impFiles;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static List<ImportFile> GetImportFilesForId(int importId)
        {
            try
            {
                List<ImportFile> impFiles;

                using (var context = new CompteResultatEntities())
                {
                    impFiles = context.ImportFiles.Where(i => i.ImportId == importId).ToList(); //.Select(i => i.Name);

                    //if (!impFiles.Any())

                }

                return impFiles;
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
