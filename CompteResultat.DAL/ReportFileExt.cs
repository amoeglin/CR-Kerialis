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
    [MetadataType(typeof(ReportFile.MetaData))]
    public partial class ReportFile
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static List<ReportFile> GetReportFilesForCRID(int crid)
        {
            try
            {
                List<ReportFile> reportFiles;

                using (var context = new CompteResultatEntities())
                {
                    reportFiles = context.ReportFiles.Where(r => r.CRId == crid).ToList();                    
                }

                return reportFiles;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        public static int Insert(ReportFile reportFile)
        {
            try
            {
                using (var context = new CompteResultatEntities())
                {
                    context.ReportFiles.Add(reportFile);
                    context.SaveChanges();

                    return reportFile.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void Update(ReportFile newRF)
        {
            try
            {
                if (newRF == null)
                    throw new Exception("The 'ReportFile' entity does not contain any data!");

                using (var context = new CompteResultatEntities())
                {
                    ReportFile oldRF = context.ReportFiles.Where(r => r.Id == newRF.Id).First();

                    oldRF.FileLocation = newRF.FileLocation;
                    oldRF.FileType = newRF.FileType;
                    oldRF.CRId = newRF.CRId;
                   
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static void Delete(int crId, string fileType)
        {
            try
            {
                ReportFile myReport;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.ReportFiles.Where(r => (r.CRId == crId && r.FileType == fileType));

                    if (elements.Any())
                    {
                        myReport = elements.First();

                        context.ReportFiles.Attach(myReport);
                        context.ReportFiles.Remove(myReport);
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





        public class MetaData
        {
            //[Display(Name = "Email address")]
            //[Required(ErrorMessage = "The email address is required")]
            //public string Email { get; set; }
        }

    }
}

