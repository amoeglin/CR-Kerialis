using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using CompteResultat.Common;

namespace CompteResultat
{
    /// <summary>
    /// Summary description for LogoH
    /// </summary>
    public class LogoHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            context.Response.ContentType = "image/png";
            var basePath = Path.Combine(context.Request.PhysicalApplicationPath, C.logoFolder);
            var filepath = Path.Combine(basePath, context.Request.QueryString["filename"]);
            //var filepath = @"C:\DEV\CompteResultat\CompteResultat\App_Data\Logos\_Logo_Formation.jpg"; // + context.Request.QueryString["filename"];
            context.Response.WriteFile(filepath);
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


