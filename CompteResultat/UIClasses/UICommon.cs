using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using System.Text;

namespace CompteResultat
{
    public class UICommon
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static void HandlePageError(Exception except, Page myPage, string source, bool logging = true)
        {
            try
            {
                //### Response.End() causes problems => for now, exclude those specific errors
                if (source == "CompteResultatManuel::cmdStartExcel_Click" || source == "CompteResultatManuel::cmdStartPPT_Click")
                    return;
                
                //this error is raised when we call Response.End() on the DownloadFile method
                if (except.Message == "Thread was being aborted.")
                    return;

                if (logging)
                    log.Error("-- SOURCE: " + source + " --- ERROR MESSAGE: " + except.Message);

                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = source + " :: " + except.Message;
                myCustomValidator.Text = myCustomValidator.ErrorMessage;
                myPage.Validators.Add(myCustomValidator);
            }
            catch (Exception ex)
            {
                if (logging)
                    log.Error(ex.Message);

                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                myPage.Validators.Add(myCustomValidator);
            }
        }

        public static void DownloadFile(Page page, string filePath)
        {
            try
            {
                FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                    page.Response.Clear();
                    page.Response.ClearHeaders();
                    page.Response.ClearContent();
                    page.Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                    //Response.AddHeader("Content-Type", "application/Excel");
                    page.Response.AddHeader("Content-Length", file.Length.ToString());
                    page.Response.ContentType = "text/plain";
                    //Response.ContentType = "application/vnd.xls";
                    page.Response.Flush();
                    page.Response.TransmitFile(file.FullName);
                    //Response.WriteFile(file.FullName); 
                    page.Response.End();
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, page, "UICommon::DownloadFile", false); }

        }

        public static string ShowPopUpMsg(string msg)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("alert('");
            sb.Append(msg.Replace("\n", "\\n").Replace("\r", "").Replace("'", "\\'"));
            sb.Append("');");
            return sb.ToString();
            
        }
    }
}