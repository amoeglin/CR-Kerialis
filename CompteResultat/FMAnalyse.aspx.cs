﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CompteResultat
{
    public partial class FMAnalyse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = Request.QueryString["path"];
            if (path != null && path != "")
            {
                ASPxFileManager2.Settings.RootFolder = path;
            }

        }
    }
}