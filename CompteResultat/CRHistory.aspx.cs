using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Configuration;
using System.Data;

using CompteResultat.BL;
using CompteResultat.DAL;
using CompteResultat.Common;
using Newtonsoft.Json;

//https://www.c-sharpcorner.com/UploadFile/7eb164/gridview-control-in-Asp-Net/
//https://www.aspsnippets.com/Articles/Display-data-in-GridView-from-database-in-ASPNet-using-C-and-VBNet.aspx
//https://www.educba.com/asp-dot-net-gridview/

namespace CompteResultat
{  
    public partial class CRHistory : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region GLOBAL PROPERTIES

        private int RowIndex
        {
            get { return ViewState["RowIndex"] != null ? int.Parse(ViewState["RowIndex"].ToString()) : -1; }
            set { ViewState["RowIndex"] = value; }
        }

        private string SortDirection
        {
            get { return ViewState["SortDirection"] != null ? ViewState["SortDirection"].ToString() : "ASC"; }
            set { ViewState["SortDirection"] = value; }
        }

        private string SortExpression
        {
            get { return ViewState["SortExpression"] != null ? ViewState["SortExpression"].ToString() : "Date"; }
            set { ViewState["SortExpression"] = value; }
        }

        private string SortExpressionType
        {
            get { return ViewState["SortExpressionType"] != null ? ViewState["SortExpressionType"].ToString() : "All"; }
            set { ViewState["SortExpressionType"] = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            string AutoCRDirectory = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "AutoCR");
            if (!Directory.Exists(AutoCRDirectory)) Directory.CreateDirectory(AutoCRDirectory);

            if (!this.IsPostBack)
            {
                //txtProvOuvertureDate.Text = DateTime.Now.ToShortDateString();
                this.BindMainGrid();
            }
        }

        private void BindMainGrid()
        {            
            if (this.SortExpression != null)
            {                
                //this.SortDirection = this.SortDirection == "ASC" ? "DESC" : "ASC";
                gvCR.DataSource = CRAutoMain.GetCRAutos(this.SortExpression, this.SortDirection, this.SortExpressionType);
            }
            else
            {
                gvCR.DataSource = CRAutoMain.GetCRAutos();
            }
                        
            gvCR.DataBind();
        }               

        protected void ScanGrid()
        {
            foreach (GridViewRow row in gvCR.Rows)
            {
                //CheckBox cb = (CheckBox)row.FindControl("chkImport");
                //int importId = Convert.ToInt32(gvCR.DataKeys[row.RowIndex].Value);                
                //string importName = row.Cells[3].Text;
                //string importPath = row.Cells[6].Text;

                GridView gvCRFiles = (GridView)row.FindControl("gvCRFiles");

                foreach (GridViewRow row2 in gvCRFiles.Rows)
                { 
                    //int idFile = Convert.ToInt32(gvCRFiles.DataKeys[row2.RowIndex].Value);

                    //CheckBox cb2 = (CheckBox)row2.FindControl("chkImport2");                    
                }                
            }
        }

        protected void gvCR_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {  
                    //ImageButton cmdDelDB = e.Row.FindControl("cmdDeleteAll") as ImageButton;                   
                    //TableCell statusCell = e.Row.Cells[7];                    

                    int crAutoId = int.Parse(gvCR.DataKeys[e.Row.RowIndex].Value.ToString());                    
                    GridView gvCRFiles = e.Row.FindControl("gvCRFiles") as GridView;

                    string type = e.Row.Cells[4].Text;
                    string listName = e.Row.Cells[7].Text;
                    string rootCRAutoPath = Server.MapPath(C.excelCRAutoFolder);
                    rootCRAutoPath = Path.Combine(rootCRAutoPath, listName);
                    string crPathSantePrev = "";

                    if (type.ToLower().Contains("sant")) crPathSantePrev = Path.Combine(rootCRAutoPath, "Sante");
                    else crPathSantePrev = Path.Combine(rootCRAutoPath, "Prev");

                    //crPathSantePrev = Path.Combine(crPathSantePrev, e.Row.Cells[2].Text);

                    e.Row.Cells[3].Text = crPathSantePrev;
                    e.Row.Cells[3].Font.Size = 10;

                    gvCRFiles.DataSource = CompteResult.GetComptesResultatByCRAutoId(crAutoId); 
                    gvCRFiles.DataBind();
                }
            }
            catch (Exception ex)
            {
                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                Page.Validators.Add(myCustomValidator);
            }
        }

        protected void gvCR_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //e.Row.Cells[12].Visible = true; 
        }

        protected void gvCRFiles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //var nfi = new System.Globalization.NumberFormatInfo { NumberGroupSeparator = " " };
                System.Globalization.NumberFormatInfo nfi = new System.Globalization.CultureInfo("fr-FR", false).NumberFormat;
                nfi.NumberDecimalDigits = 2;
                System.Globalization.NumberFormatInfo nfi2 = new System.Globalization.CultureInfo("fr-FR", false).NumberFormat;
                nfi2.NumberDecimalDigits = 0;

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //int id = int.Parse(gvCRFiles.DataKeys[e.Row.RowIndex].Value.ToString());
                    string compList = e.Row.Cells[4].Text;
                    string subsidList = e.Row.Cells[5].Text;
                    string contrList = e.Row.Cells[6].Text;
                    int maxLength = 30;

                    string comps = Company.GetCompanyNamesFromIdList(compList);
                    if (comps.Length > maxLength)
                    {
                        string shortComp = comps.Substring(0, maxLength) + "...";
                        e.Row.Cells[4].Text = shortComp;
                    }
                    else
                    {
                        e.Row.Cells[4].Text = comps;
                    }

                    string subsids = Company.GetCompanyNamesFromIdList(subsidList);
                    if (subsids.Length > maxLength)
                    {
                        string shortSub = subsids.Substring(0, maxLength) + "...";
                        e.Row.Cells[5].Text = shortSub;
                    }
                    else
                    {
                        e.Row.Cells[5].Text = subsids;
                    }

                    if (contrList.Length > maxLength)
                    {
                        string shortContr = contrList.Substring(0, maxLength) + "...";
                        e.Row.Cells[6].Text = shortContr;
                    }
                    else
                    {
                        e.Row.Cells[6].Text = contrList;
                    }

                    e.Row.Cells[4].ToolTip = comps;
                    e.Row.Cells[5].ToolTip = subsids;
                    e.Row.Cells[6].ToolTip = contrList;

                    //##### TODO => add 2 fields to Table: CompteResult => RaisonSociale & StructureCotisation => update DB Model
                    //Modify BLCompteResultat=>MapDALObject => add 2 fields mentioned above - under: if (autoReport)...
                    //Add those 2 fields to CRHistory.aspx :: Line 155
                    //The fields are only displayed if: NIVEAU == Entreprise

                    string grEnt = e.Row.Cells[3].Text;
                    if(grEnt != "Entreprise")
                    {
                        e.Row.Cells[7].Text = "-";
                        e.Row.Cells[8].Text = "-"; 
                    }


                    var myRow = ((e.Row.NamingContainer.Parent.Parent.Parent) as GridViewRow);
                    GridViewRow mainGridViewRow = ((e.Row.NamingContainer.Parent.Parent.Parent) as GridViewRow);
                    string importId = gvCR.DataKeys[mainGridViewRow.RowIndex].Value.ToString();
                    string importName = mainGridViewRow.Cells[2].Text;
                    e.Row.Cells[9].Text = importName;
                    string type = mainGridViewRow.Cells[4].Text;
                    e.Row.Cells[10].Text = type;
                    string listName = mainGridViewRow.Cells[9].Text;
                    e.Row.Cells[11].Text = listName;
                }
            }
            catch (Exception ex)
            {
                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                Page.Validators.Add(myCustomValidator);
            }
        }

        //handle button clicks
        protected void gvCR_RowCommand(object sender, GridViewCommandEventArgs e)
        {  
            if (e.CommandName == "RedirectFMCR")
            {
                GridViewRow gvr = (GridViewRow)((ImageButton)e.CommandSource).NamingContainer;
                int rowIndex = gvr.RowIndex;
                GridViewRow row = gvCR.Rows[rowIndex];

                string crFolder = row.Cells[3].Text;
                string type = row.Cells[4].Text;
                string listName = row.Cells[7].Text;

                string rootCRAutoPath = Server.MapPath(C.excelCRAutoFolder);
                rootCRAutoPath = Path.Combine(rootCRAutoPath, listName);
                string crPathSantePrev = "";

                if (type.ToLower().Contains("sant")) crPathSantePrev = Path.Combine(rootCRAutoPath, "Sante");
                else crPathSantePrev = Path.Combine(rootCRAutoPath, "Prev");

                if (crPathSantePrev != "")
                {
                    if (Directory.Exists(crPathSantePrev))
                        Response.Redirect("~/FMImport.aspx?path=" + crPathSantePrev);
                    else
                        Response.Redirect("~/FMImport.aspx?path=" + rootCRAutoPath);
                }
            }
        }

        protected void gvCRFiles_RowCommand(object sender, GridViewCommandEventArgs e)
        {            
            if (e.CommandName == "RedirectSingleReport")
            {
                GridViewRow gvr = (GridViewRow)((ImageButton)e.CommandSource).NamingContainer;
                
                string crFolder = gvr.Cells[7].Text;
                string type = gvr.Cells[8].Text;
                string listName = gvr.Cells[9].Text;
                string groupEnt = e.CommandArgument.ToString();

                string rootCRAutoPath = Server.MapPath(C.excelCRAutoFolder);
                rootCRAutoPath = Path.Combine(rootCRAutoPath, listName);
                string crPathSantePrev = "";
                string crPathFinal = "";

                if (type.ToLower().Contains("sant")) crPathSantePrev = Path.Combine(rootCRAutoPath, "Sante");
                else crPathSantePrev = Path.Combine(rootCRAutoPath, "Prev");

                if(groupEnt.ToLower().Contains("grou")) crPathFinal = Path.Combine(crPathSantePrev, "Group", crFolder);
                else crPathFinal = Path.Combine(crPathSantePrev, "Entreprise", crFolder);


                if (crPathFinal != "")
                {
                    if (Directory.Exists(crPathFinal))
                        Response.Redirect("~/FMImport.aspx?path=" + crPathFinal);
                    else
                        Response.Redirect("~/FMImport.aspx?path=" + crPathSantePrev);
                }
            }
        }

        protected void gvCRFiles_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //e.Row.Cells[12].Visible = true;
        }


        #region SECONDARY UI METHODS

        protected void OnSorting(object sender, GridViewSortEventArgs e)
        {
            this.SortDirection = this.SortDirection == "ASC" ? "DESC" : "ASC";
            this.SortExpression = e.SortExpression;
            BindMainGrid();
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtCRName.Text;
            if (searchText == null || searchText == "")
                gvCR.DataSource = CRAutoMain.GetCRAutos();
            else
                gvCR.DataSource = CRAutoMain.GetCRByName(searchText);

            gvCR.DataBind();
        }

        protected void radioReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioReportType.SelectedIndex == 0)
            {
                //all imports
                this.SortExpression = "Date";
                gvCR.DataSource = CRAutoMain.GetCRAutos();

            }
            else if (radioReportType.SelectedIndex == 1)
            {
                //only SANTE
                this.SortExpression = "SANTE";
                gvCR.DataSource = CRAutoMain.GetCRAutos("CreationDateTime", "DESC", "SANTE");
            }
            else
            {
                //only PREV
                this.SortExpression = "PREV";
                gvCR.DataSource = CRAutoMain.GetCRAutos("CreationDateTime", "DESC", "PREV"); 
            }

            gvCR.DataBind();
        }

        protected void ConfirmDelete(object sender, EventArgs e)
        {
            //string id = (sender as ImageButton).ID;
            RowIndex = Convert.ToInt32(((sender as ImageButton).NamingContainer as GridViewRow).RowIndex);            
            ClientScript.RegisterStartupScript(this.GetType(), "confirmDeleteAll", string.Format("OpenConfirmdeleteModal('{0}');", ""), true);
        }

        protected void btnDeleteAll_Click(object sender, EventArgs e)
        {
            int rowIndex = RowIndex;
            GridViewRow row = gvCR.Rows[rowIndex];

            int crAutoId = Convert.ToInt32(gvCR.DataKeys[rowIndex].Value);
            string crName = row.Cells[2].Text;
            string crFolder = row.Cells[3].Text;
            //string type = row.Cells[4].Text;
            //string listName = row.Cells[7].Text;

            string rootCRAutoPath = Server.MapPath(C.excelCRAutoFolder);
            //rootCRAutoPath = Path.Combine(rootCRAutoPath, listName);
            //string crPathSantePrev = "";
            //string crPathFinal = "";

            //if (type.ToLower().Contains("sant")) crPathSantePrev = Path.Combine(rootCRAutoPath, "Sante");
            //else crPathSantePrev = Path.Combine(rootCRAutoPath, "Prev");

            //string[] paths = { crPathSantePrev, "Group", crFolder };
            //crPathFinal = Path.Combine(paths); // crPathSantePrev, "Group", crFolder);
            //if (Directory.Exists(crPathFinal)) Directory.Delete(crPathFinal, true);

            //crPathFinal = Path.Combine(crPathSantePrev, "Entreprise", crFolder);
            //if (Directory.Exists(crPathFinal)) Directory.Delete(crPathFinal, true);

            string grFolder = Path.Combine(crFolder, "Group", crName);
            string entFolder = Path.Combine(crFolder, "Entreprise", crName);

            if (Directory.Exists(grFolder)) Directory.Delete(grFolder, true);
            if (Directory.Exists(entFolder)) Directory.Delete(entFolder, true);

            //if (Directory.Exists(crFolder)) Directory.Delete(crFolder, true);
            CRAutoMain.Delete(crAutoId);             

            BindMainGrid();
        }

        #endregion
                
    }
    
}