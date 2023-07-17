using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using CompteResultat.BL;
using CompteResultat.DAL;
using CompteResultat.Common;
using OfficeOpenXml;
using System.Drawing;
using System.Data;

namespace CompteResultat
{
    public partial class GestionCRListes : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region GLOBAL PROPERTIES

        string userName;

        private int CRListId
        {
            get { return ViewState["CRListId"] != null ? int.Parse(ViewState["CRListId"].ToString()) : -1; }
            set { ViewState["CRListId"] = value; }
        }

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
            get { return ViewState["SortExpression"] != null ? ViewState["SortExpression"].ToString() : "Name"; }
            set { ViewState["SortExpression"] = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            userName = User.Identity.Name;

            try
            {
                cmdImport.Attributes.Add("onclick", "jQuery('#" + uploadExcel.ClientID + "').click();return false;");

                if (!IsPostBack)
                {
                    BindMainGrid();
                }
                else
                {
                    //Handle the Delete Event
                    if (Request.Form["cmdDelete"] != null) { }

                    //Handle the import of new listes
                    if (uploadExcel.PostedFile != null)
                    {
                        if (uploadExcel.PostedFile.FileName.Length > 0)
                        {
                            string excelFile = Path.GetFileName(uploadExcel.PostedFile.FileName);
                            string uploadPath = Path.Combine(Request.PhysicalApplicationPath, "App_Data\\CRListes", excelFile);
                            uploadExcel.PostedFile.SaveAs(uploadPath);

                            ImportList(uploadPath);
                            BindMainGrid();
                        }
                    }
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionCadencier::Page_Load"); }
        }

        #region REPEATER

        protected void rptCad_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Repeater rpt = sender as Repeater; 
            
            if (rpt != null)
            {
                if (e.Item.ItemType == ListItemType.Footer)
                {
                    if (rpt.Items.Count < 1)
                    {
                        rptCad.Visible = false;
                        phHeader.Visible = true;
                    }
                    else
                    {
                        rptCad.Visible = true;
                        phHeader.Visible = false;
                    }
                }
            }
        }

        public List<CRGenListComp> GetGroupEntreprise()
        {
            try
            {
                if (CRListId != -1) return CRGenListComp.GetByCRListId(CRListId);
                else return null;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionCRListes::GetGroupEntreprise"); return null; }
        }

        #endregion

        #region GRIDVIEW

        private void BindMainGrid()
        {
            if (chkMyLists.Checked)
            {
                if (this.SortExpression != null)
                {
                    gvCRListes.DataSource = CRGenList.GetListsForUser(this.SortExpression, this.SortDirection, userName);
                }
                else
                {
                    gvCRListes.DataSource = CRGenList.GetListsForUser(userName);
                }
            }
            else
            {
                if (this.SortExpression != null)
                {
                    gvCRListes.DataSource = CRGenList.GetLists(this.SortExpression, this.SortDirection);
                }
                else
                {
                    gvCRListes.DataSource = CRGenList.GetLists();
                }
            }

            gvCRListes.DataBind();
            if (gvCRListes.Rows.Count > 0) gvCRListes.SelectRow(0);
            rptCad.DataBind();
        }

        protected void gvCRListes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {                
                if (e.Row.RowType == DataControlRowType.DataRow)
                {   
                    int importId = int.Parse(gvCRListes.DataKeys[e.Row.RowIndex].Value.ToString());

                    e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gvCRListes, "Select$" + e.Row.RowIndex);
                    e.Row.ToolTip = "Cliquez pour sélectionner cette ligne";                    
                }
            }
            catch (Exception ex)
            {
                //var myCustomValidator = new CustomValidator();
                //myCustomValidator.IsValid = false;
                //myCustomValidator.ErrorMessage = ex.Message;
                //Page.Validators.Add(myCustomValidator);
            }
        }

        protected void gvCRListes_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //e.Row.Cells[12].Visible = true;             
        }

        //handle button clicks
        protected void gvCRListes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Export")
            {
                //string id = e.CommandArgument.ToString();
                GridViewRow gvr = (GridViewRow)((ImageButton)e.CommandSource).NamingContainer;
                int rowIndex = gvr.RowIndex;
                GridViewRow row = gvCRListes.Rows[rowIndex];

                int listId = Convert.ToInt32(gvCRListes.DataKeys[rowIndex].Value);
                string listeName = row.Cells[1].Text;
                string type = row.Cells[3].Text;
                string assurType = row.Cells[4].Text;

                ExportCRListe(listeName, listId, type, assurType);
            }
        }

        protected void gvCRListes_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvCRListes.Rows)
            {                
                if (row.RowIndex == gvCRListes.SelectedIndex)
                {
                    CRListId = int.Parse(gvCRListes.DataKeys[row.RowIndex].Value.ToString());
                    row.BackColor =  ColorTranslator.FromHtml("#A1DCF2");
                    row.ToolTip = string.Empty;
                }
                else
                {
                    row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                    row.ToolTip = "Cliquez pour sélectionner cette ligne";
                }
            }
            rptCad.DataBind();
        }

        #endregion


        #region SECONDARY UI METHODS

        public void ExportCRListe(string listName, int crListId, string type, string assurType)
        {
            try
            {
                List<CRGenListComp> crComps = CRGenListComp.GetByCRListId(crListId);

                ExcelPackage pck = new ExcelPackage();
                var ws = pck.Workbook.Worksheets.Add("LISTE GROUPES ET ENTREPRISES");

                //write the header
                ws.Cells[1, 1].Value = "GROUPE";
                ws.Cells[1, 2].Value = "N°ENTREPRISE";
                ws.Cells[1, 3].Value = type;
                ws.Cells[1, 4].Value = assurType;

                int row = 2;

                foreach (CRGenListComp c in crComps)
                {
                    ws.Cells[row, 1].Value = c.GroupName;
                    ws.Cells[row, 2].Value = c.Enterprise;
                    row++;
                }

                string uploadPath = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "CRListes", listName + ".xlsx");

                pck.SaveAs(new FileInfo(uploadPath));
                UICommon.DownloadFile(this.Page, uploadPath);
            }
            catch (Exception ex)
            {
                if (ex.Message != "Thread was being aborted.")
                    log.Error(ex.Message);

                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                Page.Validators.Add(myCustomValidator);
            }
        }

        public void ImportList(string excelFilePath)
        {
            try
            {
                string listName = Path.GetFileNameWithoutExtension(excelFilePath);
                string type = "SANTE"; //TODO get info from Excel
                string assurType = "ENTREPRISE";
                string groupName = "";
                string enterprise = "";

                //read Excel file into datatable
                DataTable dt = G.ExcelToDataTable(excelFilePath, false);

                //check if there is alredy a list with the same name => delete it                
                CRGenList myList = CRGenList.GetSingleListName(listName);
                if (myList != null)
                {
                    CRGenList.DeleteListWithId(myList.Id);
                }

                int id = 0;
                int cnt = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (cnt == 0)
                    {
                        string excelType = row[2].ToString();
                        if (excelType.ToLower().Contains("sant")) { type = "SANTE"; }
                        else { type = "PREV"; }

                        excelType = row[3].ToString();
                        if (excelType.ToLower().Contains("prod")) { assurType = "PRODUIT"; }
                        else { assurType = "ENTREPRISE"; }

                        //create entry in CRGenList
                        id = CRGenList.Insert(new CRGenList { Name = listName, UserName = userName, Type = type, AssurType=assurType });

                    }
                    else
                    {
                        //validate => all fields must be specified  
                        //if (!Int32.TryParse(row[C.eExcelCadencier.Year.ToString()].ToString(), out year))
                        //    throw new Exception("One of the provided 'Year' values is not valid for the Cadencier you are trying to import !");
                        groupName = row[0].ToString();
                        enterprise = row[1].ToString();

                        int idComp = CRGenListComp.Insert(new CRGenListComp { CRListId = id, GroupName = groupName, Enterprise = enterprise });
                    }
                    cnt++;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        protected void OnSorting(object sender, GridViewSortEventArgs e)
        {
            this.SortDirection = this.SortDirection == "ASC" ? "DESC" : "ASC";
            this.SortExpression = e.SortExpression;
            BindMainGrid();
        }

        protected void btnDeleteAll_Click(object sender, EventArgs e)
        {
            int listId = Convert.ToInt32(gvCRListes.DataKeys[RowIndex].Value);
            CRGenList.DeleteListWithId(listId);

            // delete Excel file
            GridViewRow row = gvCRListes.Rows[RowIndex];            
            string listName = row.Cells[1].Text;
            string filePath = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "CRListes", listName + ".xlsx");

            if (File.Exists(filePath))
                File.Delete(filePath);

            BindMainGrid();
        }

        protected void ConfirmDelete(object sender, EventArgs e)
        {
            RowIndex = Convert.ToInt32(((sender as ImageButton).NamingContainer as GridViewRow).RowIndex);
            ClientScript.RegisterStartupScript(this.GetType(), "confirmDeleteAll", string.Format("OpenConfirmdeleteModal('{0}');", "ALL"), true);
        }

        protected void chkMyLists_CheckedChanged(object sender, EventArgs e)
        {
            BindMainGrid();
        }

        #endregion

    }
}