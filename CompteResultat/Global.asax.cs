using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Configuration;

using log4net;
using CompteResultat.Common;


[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace CompteResultat
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();

            C.imageFolder = WebConfigurationManager.AppSettings["ImageFolder"];
            C.imageRelFolder = G.GetRelativeFolderPath(C.imageFolder);
            C.logoFolder = WebConfigurationManager.AppSettings["LogoFolder"];
            C.logoRelFolder = G.GetRelativeFolderPath(C.logoFolder);
            C.excelFolder = WebConfigurationManager.AppSettings["ExcelFolder"];
            C.excelRelFolder = G.GetRelativeFolderPath(C.excelFolder);
            C.uploadFolder = WebConfigurationManager.AppSettings["UploadFolder"];
            C.uploadRelFolder = G.GetRelativeFolderPath(C.logoFolder);
            C.reportTemplateFolder = WebConfigurationManager.AppSettings["ReportTemplateFolder"];
            C.reportTemplateRelFolder = G.GetRelativeFolderPath(C.reportTemplateFolder);
            C.excelCRFolder = WebConfigurationManager.AppSettings["ExcelCRFolder"];
            C.excelCRRelFolder = G.GetRelativeFolderPath(C.excelCRFolder);

            C.maxPrestaLines = int.Parse(WebConfigurationManager.AppSettings["MaxPrestaLines"]);
            C.maxPrestaIterations = int.Parse(WebConfigurationManager.AppSettings["MaxPrestaIterations"]);

            C.ExcelTemplateGlobalCompanySante = WebConfigurationManager.AppSettings["ExcelTemplateGlobalCompanySante"];
            C.ExcelTemplateGlobalCompanyPrevoyance = WebConfigurationManager.AppSettings["ExcelTemplateGlobalCompanyPrevoyance"];
            //C.ExcelTemplateGlobalSubsid = WebConfigurationManager.AppSettings["ExcelTemplateGlobalSubsid"];

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}