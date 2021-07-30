using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompteResultat
{
    /// <summary>
    /// Summary description for ImageHandler
    /// </summary>
    public class ImageHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            //Checking whether the imagebytes session variable have anything else not doing anything

            if ((context.Session["ImageBytes"]) != null)
            {
                byte[] image = (byte[])(context.Session["ImageBytes"]);
                context.Response.ContentType = "image/JPEG";
                context.Response.BinaryWrite(image);
            }
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