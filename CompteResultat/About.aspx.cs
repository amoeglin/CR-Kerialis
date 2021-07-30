using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CompteResultat.Common;

namespace CompteResultat
{
    public partial class About : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            imgCompany.ImageUrl = C.imageRelFolder + "moeglin.png";

        }
    }
}