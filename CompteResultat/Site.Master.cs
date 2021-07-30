using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CompteResultat.Common;
using System.Web.Configuration;

namespace CompteResultat
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected bool IsAdmin = false;
        protected void Page_Load(object sender, EventArgs e)
        {

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            SoftVersion.Text = "Version : " + version;
            SoftVersion2.Text = "Version : " + version;

            foreach (MenuItem menuitem in menu.Items)
            {
                if (menuitem.Value == "Purge")
                {
                    // coloring the main menu item
                    menuitem.Text = "<div style='color: Red; font-weight: bold;'>" + menuitem.Text + "</div>";  
                }  
            }
            if (Page.User.IsInRole(C.eUserRoles.Administrateur.ToString()))
            {
                IsAdmin = true;
            }

            string logo = WebConfigurationManager.AppSettings["Logo"];
            imgLogo.ImageUrl = "./Images/" + logo;
        }
    }
}