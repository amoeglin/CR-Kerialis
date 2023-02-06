using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace CompteResultat
{
    /// <summary>
    /// Summary description for FileHandler
    /// </summary>
    public class FileHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        //https://stackoverflow.com/questions/15543136/download-file-via-handler-without-ending-current-response
        //https://web.archive.org/web/20160319051244/http://encosia.com/ajax-file-downloads-and-iframes/
        public void ProcessRequest(HttpContext context)
        {
            string fileName = "";

            try
            {
                if (context.Request.Files.Count > 0)
                {
                    HttpFileCollection files = context.Request.Files;
                    foreach (string key in files)
                    {
                        HttpPostedFile file = files[key];
                        fileName = context.Server.MapPath("~/App_Data/UploadTemp/" + file.FileName);

                        file.SaveAs(fileName);
                        context.Session["singleFileUpload"] = fileName;

                        //context.Response.Redirect("~/ImportData.aspx?fileToAnalyse=" + fileName, false);
                        //context.Server.TransferRequest("~/ImportData.aspx?fileToAnalyse=" + fileName, false);
                        //return;                    
                    }
                    context.Response.ContentType = "text/plain";
                    context.Response.Write(fileName);
                }
            }
            catch (Exception ex) { }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}