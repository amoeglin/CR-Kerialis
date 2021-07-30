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
    [MetadataType(typeof(ReportTemplate.MetaData))]
    public partial class ReportTemplate
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static List<ReportTemplate> GetReportTemplates()
        {
            try
            {
                List<ReportTemplate> repTempl;

                using (var context = new CompteResultatEntities())
                {
                    repTempl = context.ReportTemplate.Where(e => e.Type != "SANTE_SYNT").OrderBy(r => r.DisplayOrder).ToList();
                }

                if (repTempl == null || repTempl.Count == 0)
                    throw new Exception("The 'ReportTemplates' entity does not contain any data!");

                return repTempl;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static string GetTemplateNameForId(int id)
        {
            try
            {
                string name;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.ReportTemplate.Where(c => c.Id == id).Select(c => c.Name);

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

        public static int GetTemplateIdForType(string templateType)
        {
            try
            {
                int id;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.ReportTemplate.Where(c => c.Type == templateType).Select(c => c.Id);

                    if (elements.Any())
                        id = elements.First();
                    else
                        id = -1;
                }

                return id;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static string GetTemplateFileNameForId(int id)
        {
            try
            {
                string name;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.ReportTemplate.Where(c => c.Id == id).Select(c => c.FileName);

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
       
        public static int GetYearsToCalcForId(int id)
        {
            try
            {
                int yearsToCalc;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.ReportTemplate.Where(c => c.Id == id).Select(c => c.YearsToCalc);

                    if (elements.Any())
                        if (elements.First().HasValue)
                            yearsToCalc = elements.First().Value;
                        else
                            yearsToCalc = 1;
                    else
                        yearsToCalc = 1;
                }

                return yearsToCalc;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public static bool GetWithOptionFlag(int id)
        {
            try
            {
                bool withOption;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.ReportTemplate.Where(c => c.Id == id).Select(c => c.WithOption);

                    if (elements.Any())
                        if (elements.First().HasValue)
                            withOption = elements.First().Value;
                        else
                            withOption = false;
                    else
                        withOption = false;
                }

                return withOption;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }


        public static C.eReportTemplateTypes GetTemplateTypeForId(int id)
        {
            try
            {
                C.eReportTemplateTypes type = C.eReportTemplateTypes.SANTE;

                using (var context = new CompteResultatEntities())
                {
                    var elements = context.ReportTemplate.Where(c => c.Id == id).Select(c => c.Type);

                    if (elements.Any())                        
                        type = (C.eReportTemplateTypes)Enum.Parse(typeof(C.eReportTemplateTypes), elements.First());                        
                    else
                        throw new Exception("GetTemplateTypeForId :: No template type was found for the report template with the following ID: " + id.ToString());
                }

                return type;
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
