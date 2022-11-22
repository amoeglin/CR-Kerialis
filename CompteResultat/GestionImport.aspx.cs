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
    public partial class GestionImport : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region GLOBAL PROPERTIES

        private int RowIndex
        {
            get { return ViewState["RowIndex"] != null ? int.Parse(ViewState["RowIndex"].ToString()) : -1; }
            set { ViewState["RowIndex"] = value; }
        }

        private string DeleteAllOrDB
        {
            get { return ViewState["DeleteAllOrDB"] != null ? ViewState["DeleteAllOrDB"].ToString() : "DB"; }
            set { ViewState["DeleteAllOrDB"] = value; }
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
        private string SortExpressionArchived
        {
            get { return ViewState["SortExpressionArchived"] != null ? ViewState["SortExpressionArchived"].ToString() : "All"; }
            set { ViewState["SortExpressionArchived"] = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
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
                gvImport.DataSource = Import.GetImports(this.SortExpression, this.SortDirection, this.SortExpressionArchived);
            }
            else
            {
                gvImport.DataSource = Import.GetImports();
            }
                        
            gvImport.DataBind();
        }               

        protected GVProperties ScanGrid()
        {
            int numberOfImportFilesSelected = 0;
            int numberOfDifferntImports = 0;
            List<int> importIds = new List<int>();
            GVProperties props = new GVProperties();

            foreach (GridViewRow row in gvImport.Rows)
            {
                //CheckBox cb = (CheckBox)row.FindControl("chkImport");
                int importId = Convert.ToInt32(gvImport.DataKeys[row.RowIndex].Value);                
                string importName = row.Cells[3].Text;
                //string importPath = row.Cells[6].Text;

                GridView gvImpFiles = (GridView)row.FindControl("gvImpFiles");

                foreach (GridViewRow row2 in gvImpFiles.Rows)
                {
                    CheckBox cb2 = (CheckBox)row2.FindControl("chkImport2");
                    if (cb2 != null && cb2.Checked)
                    {
                        int idFile = Convert.ToInt32(gvImpFiles.DataKeys[row2.RowIndex].Value);
                        numberOfImportFilesSelected++;
                        importIds.Add(importId);
                        props.lastId = idFile;

                        props.singleSelectId = importId;
                        props.singleSelectName = importName;
                    }
                }                
            }

            importIds = importIds.Distinct().ToList();
            numberOfDifferntImports = importIds.Count();

            props.numberOfDifferntImports = numberOfDifferntImports;
            props.numberOfImportFilesSelected = numberOfImportFilesSelected;

            return props;
        }

        protected void cmdImport_Click(object sender, EventArgs e)
        { 
            //generic properties 
            bool hasErr = false;
            int impId = 0;
            bool updateGroupCadExp = false;
            string uploadDirectory = Path.Combine(Request.PhysicalApplicationPath, C.uploadFolder);
            string newImportDirectory = "";

            DateTime dtProvOuverture;
            string provOuverture;
            if (! DateTime.TryParse(txtProvOuvertureDate.Text, out dtProvOuverture))
                dtProvOuverture = DateTime.Now;
            provOuverture = dtProvOuverture.ToShortDateString();

            try
            {
                GVProperties props = ScanGrid();
                
                //validation: Import name must be provided and at least 1 element must be selected
                if (txtNomImport.Text == "")
                    throw new Exception("Il faudra renseigner un nom pour l’import dans le champ 'Nom Import' !");

                if (props.numberOfImportFilesSelected == 0)
                    throw new Exception("Il faudra sélectionner au moins un fichier d'import !");

                string importName = txtNomImport.Text;

                // create new import folder - if at least one file was selected                
                newImportDirectory = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "Imports", importName + "-" + DateTime.Now.ToString("s").Replace(":", "-"));
                Directory.CreateDirectory(newImportDirectory);

                //if several import directories were selected, create a global ImportId  in the DB for all those selected imports
                if (props.numberOfDifferntImports > 0)
                {
                    Import imp = new Import { Name = importName, Date = DateTime.Today.Date, UserName = User.Identity.Name, 
                        ImportPath = newImportDirectory, Archived = false };
                    impId = Import.Insert(imp);                    
                }
               

                // Iterate through the Products.Rows property
                foreach (GridViewRow row in gvImport.Rows)
                {
                    // Access the CheckBox
                    CheckBox cb = (CheckBox)row.FindControl("chkImport");

                    int importId = Convert.ToInt32(gvImport.DataKeys[row.RowIndex].Value);
                    string impName = row.Cells[3].Text;
                    string importPath = row.Cells[6].Text;
                    bool archived = row.Cells[7].Text == "OUI" ? true : false;
                    
                    GridView gvImpFiles = (GridView)row.FindControl("gvImpFiles");

                    foreach (GridViewRow row2 in gvImpFiles.Rows)
                    {
                        CheckBox cb2 = (CheckBox)row2.FindControl("chkImport2");
                        if (cb2 != null && cb2.Checked)
                        {
                            int idFile = Convert.ToInt32(gvImpFiles.DataKeys[row2.RowIndex].Value);
                            string fileGroup = row2.Cells[2].Text;
                            string fileType = row2.Cells[3].Text;
                            string fileName = row2.Cells[4].Text;

                            //update of Group, Cad & Exp are very slow => we do it only on the very last import - lastId is the id of the file in the ImportFiles
                            if (idFile == props.lastId)
                                updateGroupCadExp = true;
                            else
                                updateGroupCadExp = false;
                            
                            string currentImportFilePath = Path.Combine(importPath, fileName);                                
                            UploadPaths uplPaths = GetUploadFilePaths(currentImportFilePath, newImportDirectory, uploadDirectory, fileGroup, fileType, fileName);

                            BLImport blImp = GetImportBasicConfig(uplPaths, importName, provOuverture, newImportDirectory, updateGroupCadExp);

                            //if several imports were selected in the GV, use the importId we created above                             
                            blImp.ImportId = impId;

                            if (props.numberOfDifferntImports == 1)
                            {
                                //if we have only 1 import, we are creating a new import in the Import table and we delete the old one
                                BLImport.CleanTablesForSpecificImportID(importId, false, false); 
                                BLImport.CleanupImportDirectory(importPath);
                            } else
                            {
                                //none of the old import files in the Imports directory are deleted 
                                if (!archived)
                                {
                                    //archive this import 
                                    BLImport.CleanTablesForSpecificImportID(importId, true, false);
                                }
                            }                                                        

                            blImp.DoImport();
                        }
                    } 
                }           

                //refresh the data grid
                BindMainGrid();

            }            
            catch (Exception ex)
            {
                hasErr = true;
                //### cleanup
                BLImport.CleanupImportDirectory(newImportDirectory);
                //Directory.Delete(importDirectory, true);
                UICommon.HandlePageError(ex, this.Page, "cmdImport_Click");
            }
            finally
            {
                if (!hasErr)
                {
                    txtNomImport.Text = "";                    
                }
            }
        }
        
        protected UploadPaths GetUploadFilePaths(string currentImportFilePath, string newImportPath, string uploadPath, string fileGroup, string fileType, string fileName)
        {
            UploadPaths paths = new UploadPaths();

            FileInfo currentImportFileFI = new FileInfo(currentImportFilePath);
            string prefix = User.Identity.Name + "_";
                        
            if (File.Exists(currentImportFilePath))
            {
                //save the files to be imported to the Upload folder and to the new Imports folder                
                string newImportFilePath = Path.Combine(newImportPath, fileName);
                currentImportFileFI.CopyTo(newImportFilePath, true);
                string uploadFilePath = Path.Combine(uploadPath, prefix + fileName);
                currentImportFileFI.CopyTo(uploadFilePath, true);

                if (fileGroup == C.cIMPFILEGROUPSANTE)
                {
                    if (fileType == C.cIMPFILETYPECOT)
                    {
                        paths.uploadPathCot = uploadFilePath;                        
                    }
                    else if (fileType == C.cIMPFILETYPEDEMO)
                    {
                        paths.uploadPathDemo = uploadFilePath;
                    }
                    else if (fileType == C.cIMPFILETYPEPREST)
                    {
                        paths.uploadPathPrest = uploadFilePath;
                    }
                }
                else
                {
                    if (fileType == C.cIMPFILETYPECOT)
                    {
                        paths.uploadPathCotPrev = uploadFilePath;
                    }
                    else if (fileType == C.cIMPFILETYPEDECOMP)
                    {
                        paths.uploadPathDecompPrev = uploadFilePath;
                    }
                    else if (fileType == C.cIMPFILETYPEPROVCLOT)
                    {
                        paths.uploadPathProv = uploadFilePath;
                    }
                    else if (fileType == C.cIMPFILETYPEPROVOUV)
                    {
                        paths.uploadPathProvOuverture = uploadFilePath;
                    }
                    else if (fileType == C.cIMPFILETYPESIN)
                    {
                        paths.uploadPathSinistrPrev = uploadFilePath;
                    }
                }
            }

            return paths;
        }

        protected BLImport GetImportBasicConfig(UploadPaths uplPaths, string importName, string provOuverture, string importDirectory, bool updateGroupCadExp)
        {
            string csvSep = WebConfigurationManager.AppSettings["CSVSEP"];
            string userName = User.Identity.Name;

            string uploadDirectory = Path.Combine(Request.PhysicalApplicationPath, C.uploadFolder);

            bool forceCompanySubsid = false;
            bool updateGroupes = updateGroupCadExp;
            bool updateExperience = updateGroupCadExp;
            bool updateCad = updateGroupCadExp;

            string prefix = userName + "_";
            string newPrestEntCSV = Path.Combine(uploadDirectory, prefix + C.eMOGImportFile.PrestationsEntMOG.ToString() + ".csv");
            string newPrestProdCSV = Path.Combine(uploadDirectory, prefix + C.eMOGImportFile.PrestationsProdMOG.ToString() + ".csv");
            string newCotEntCSV = Path.Combine(uploadDirectory, prefix + C.eMOGImportFile.CotisationsEntMOG.ToString() + ".csv");
            string newCotProdCSV = Path.Combine(uploadDirectory, prefix + C.eMOGImportFile.CotisationsProdMOG.ToString() + ".csv");
            string newDemoEntCSV = Path.Combine(uploadDirectory, prefix + C.eMOGImportFile.DemographyEntMOG.ToString() + ".csv");
            string newDemoProdCSV = Path.Combine(uploadDirectory, prefix + C.eMOGImportFile.DemographyProdMOG.ToString() + ".csv");
            string newOtherFieldsCSV = Path.Combine(uploadDirectory, prefix + C.eMOGImportFile.OtherFieldsMOG.ToString() + ".csv");
            string newCotPrevCSV = Path.Combine(uploadDirectory, prefix + C.eMOGImportFile.CotPrevMOG.ToString() + ".csv");
            string newSinistrePrevCSV = Path.Combine(uploadDirectory, prefix + C.eMOGImportFile.SinistrePrevMOG.ToString() + ".csv");
            string newDecompPrevCSV = Path.Combine(uploadDirectory, prefix + C.eMOGImportFile.DecompPrevMOG.ToString() + ".csv");
            string newProvCSV = Path.Combine(uploadDirectory, prefix + C.eMOGImportFile.ProvMOG.ToString() + ".csv");
            string newProvOuvertureCSV = Path.Combine(uploadDirectory, prefix + C.eMOGImportFile.ProvOuvertureMOG.ToString() + ".csv");
            string newExpCSV = Path.Combine(uploadDirectory, prefix + C.eMOGImportFile.ExpMOG.ToString() + ".csv");

            string tableForOtherFields = WebConfigurationManager.AppSettings["TableForOtherFields"];
            string configStringPrest = WebConfigurationManager.AppSettings[C.eConfigStrings.PrestSante.ToString()];
            string configStringCot = WebConfigurationManager.AppSettings[C.eConfigStrings.CotisatSante.ToString()];
            string configStringDemo = WebConfigurationManager.AppSettings[C.eConfigStrings.Demography.ToString()];
            string configStringOtherFields = WebConfigurationManager.AppSettings[C.eConfigStrings.OtherFields.ToString()];
            string configStringCotPrev = WebConfigurationManager.AppSettings[C.eConfigStrings.CotisatPrev.ToString()];
            string configStringSinistrPrev = WebConfigurationManager.AppSettings[C.eConfigStrings.SinistrePrev.ToString()];
            string configStringDecompPrev = WebConfigurationManager.AppSettings[C.eConfigStrings.DecomptePrev.ToString()];
            string configStringProv = WebConfigurationManager.AppSettings[C.eConfigStrings.Provisions.ToString()];
            string configStringExp = WebConfigurationManager.AppSettings[C.eConfigStrings.Experience.ToString()];

            // not used
            string uploadPathExp = "";            

            BLImport imp = new BLImport(userName, newPrestEntCSV, newPrestProdCSV, newCotEntCSV, newCotProdCSV, newDemoEntCSV, newDemoProdCSV, newOtherFieldsCSV,
                    newCotPrevCSV, newSinistrePrevCSV, newDecompPrevCSV, newProvCSV, newProvOuvertureCSV,
                    configStringPrest, configStringDemo, configStringCot, configStringOtherFields, configStringCotPrev, configStringSinistrPrev, configStringDecompPrev, configStringProv,
                    tableForOtherFields, importName, csvSep, uploadDirectory, importDirectory, uplPaths.uploadPathPrest, uplPaths.uploadPathCot, uplPaths.uploadPathDemo,
                    uplPaths.uploadPathCotPrev, uplPaths.uploadPathSinistrPrev, uplPaths.uploadPathDecompPrev, uplPaths.uploadPathProv, uplPaths.uploadPathProvOuverture,
                    newExpCSV, configStringExp, uploadPathExp, forceCompanySubsid, updateGroupes, updateExperience, updateCad, provOuverture);

            return imp;
        }

        protected void gvImport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //Import selImport = e.Row.DataItem as Import;
                    //int importId = int.Parse(DataBinder.Eval(e.Row.DataItem, "Id").ToString());

                    //Button cmd = e.Row.FindControl("cmdDelete") as Button;
                    //cmd.CommandArgument = importId.ToString();

                    ImageButton imgBtn = e.Row.FindControl("cmdDeleteDB") as ImageButton; 
                    TableCell statusCell = e.Row.Cells[7];
                    if (statusCell.Text == "True")
                    {
                        statusCell.Text = "OUI";
                        imgBtn.Enabled = false;
                        imgBtn.ImageUrl = "~/Images/dbDisabled.png";
                    }
                    else
                    {
                        statusCell.Text = "NON";
                        imgBtn.Enabled = true;
                        imgBtn.ImageUrl = "~/Images/deleteDB.png";
                    }

                    bool archived = Boolean.Parse((DataBinder.Eval(e.Row.DataItem, "Archived").ToString()));

                    if (archived)
                    {
                        e.Row.Attributes.Add("style", "background-color:#FFFF74");
                    }

                    int importId = int.Parse(gvImport.DataKeys[e.Row.RowIndex].Value.ToString());
                    GridView gvImpFiles = e.Row.FindControl("gvImpFiles") as GridView;
                    gvImpFiles.DataSource = ImportFile.GetImportFilesForId(importId);
                    gvImpFiles.DataBind();
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
        protected void gvImport_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //return;
            
            //if (e.CommandName == "DeleteImp")
            //{
            //    int importId;

            //    if (Int32.TryParse(e.CommandArgument.ToString(), out importId))
            //    {
            //        BLImport.CleanTablesForSpecificImportID(importId, true, false);
            //        BindMainGrid();
            //    }
            //}
            //if (e.CommandName == "DeleteImpAll")
            //{
            //    int rowIndex = Convert.ToInt32(e.CommandArgument);
            //    GridViewRow row = gvImport.Rows[rowIndex];

            //    int importId = Convert.ToInt32(gvImport.DataKeys[rowIndex].Value);
            //    string importDir = row.Cells[6].Text;

            //    BLImport.CleanTablesForSpecificImportID(importId, false, false);
            //    BLImport.CleanupImportDirectory(importDir);

            //    BindMainGrid();               
            //}
            
            //show import files in explorer
            if (e.CommandName == "RedirectFMImport")
            {
                string importPath = e.CommandArgument.ToString();
                string importDirectory = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "Imports");

                if (importPath != "")
                {
                    if(Directory.Exists(importPath))
                        Response.Redirect("~/FMImport.aspx?path=" + importPath);
                    else
                        Response.Redirect("~/FMImport.aspx?path=" + importDirectory);
                }
            }
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
            string searchText = txtImportFilter.Text;
            if (searchText == null || searchText == "")
                gvImport.DataSource = Import.GetImports();
            else
                gvImport.DataSource = Import.GetImportsByName(searchText);

            gvImport.DataBind();
        }

        protected void radioReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioReportType.SelectedIndex == 0)
            {
                //all imports
                this.SortExpression = "Date";
                this.SortExpressionArchived = "All";
                gvImport.DataSource = Import.GetImports();

            }
            else if (radioReportType.SelectedIndex == 1)
            {
                //only archived
                this.SortExpression = "OnlyArchived";
                this.SortExpressionArchived = "Archived";
                gvImport.DataSource = Import.GetImports("OnlyArchived", "DESC", "Archived");
            }
            else
            {
                //only active imports
                this.SortExpression = "OnlyNonArchived";
                this.SortExpressionArchived = "NonArchived";
                gvImport.DataSource = Import.GetImports("OnlyNonArchived", "DESC", "NonArchived");
            }

            gvImport.DataBind();
        }

        protected void ConfirmDelete(object sender, EventArgs e)
        {
            string id = (sender as ImageButton).ID;
            int rowIndex = Convert.ToInt32(((sender as ImageButton).NamingContainer as GridViewRow).RowIndex);
            RowIndex = rowIndex;

            //GridViewRow row = gvImport.Rows[rowIndex];
            //lblTest.Text = (row.FindControl("lblTest_Id") as Label).Text;            
            if (id == "cmdDeleteAll")
            {
                DeleteAllOrDB = "ALL";
                ClientScript.RegisterStartupScript(this.GetType(), "confirmDeleteAll", string.Format("OpenConfirmdeleteModal('{0}');", DeleteAllOrDB), true);
            }
            else if (id == "cmdDeleteDB")
            {
                DeleteAllOrDB = "DB";
                ClientScript.RegisterStartupScript(this.GetType(), "confirmDeleteDB", string.Format("OpenConfirmdeleteModal('{0}');", DeleteAllOrDB), true);
            }
        }

        protected void btnDeleteAll_Click(object sender, EventArgs e)
        {
            int rowIndex = RowIndex;
            GridViewRow row = gvImport.Rows[rowIndex];

            int importId = Convert.ToInt32(gvImport.DataKeys[rowIndex].Value);
            string importDir = row.Cells[6].Text;

            if (DeleteAllOrDB == "ALL")
            {
                BLImport.CleanTablesForSpecificImportID(importId, false, false);
                BLImport.CleanupImportDirectory(importDir);
            }
            else if (DeleteAllOrDB == "DB")
            {
                BLImport.CleanTablesForSpecificImportID(importId, true, false);
            }

            BindMainGrid();
        }

        #endregion



        #region NO LONGER REQUIRED

        //verify which checkboxes are selected => create an object => deserilize object => call JS function with deserialized obj
        protected void Page_Load_OLD(object sender, EventArgs e)
        {

            List<GVSelection> gvSelections = new List<GVSelection>();

            //check the hidden field in each row and if the value is: "expanded", add the id of the corresponding imageID
            //to the string that is used as an arg to call the ExpandImages JS function
            string imageIds = "";
            foreach (GridViewRow row in gvImport.Rows)
            {
                bool exp = false;
                Image imgPlusMinus = (Image)row.FindControl("imgPlusMinus");
                HiddenField hdnState = (HiddenField)row.FindControl("hdnState");
                CheckBox chkImport = (CheckBox)row.FindControl("chkImport");
                GridView gvChild = (GridView)row.FindControl("gvImpFiles");

                //int importId = Convert.ToInt32(gvImport.DataKeys[row.RowIndex].Value);

                if (hdnState.Value == "expanded")
                {
                    exp = true;
                }

                List<string> fileChkIds = new List<string>();
                bool someSelections = false;
                foreach (GridViewRow row2 in gvChild.Rows)
                {
                    CheckBox chkImport2 = (CheckBox)row2.FindControl("chkImport2");
                    if (chkImport2.Checked)
                    {
                        someSelections = true;
                        fileChkIds.Add(chkImport2.ClientID);
                    }

                }

                gvSelections.Add(new GVSelection { expanded = exp, imageId = imgPlusMinus.ClientID, chkId = chkImport.ClientID, fileChkIds = fileChkIds });
            }

            string json = JsonConvert.SerializeObject(gvSelections);
            
            ClientScript.RegisterStartupScript(this.GetType(), "Javascript", string.Format("ExpandImages('{0}');", json), true);
        }

        protected void chkImport_CheckedChanged_OLD(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;

            GridViewRow gvr = (GridViewRow)chk.NamingContainer;
            int importId = Convert.ToInt32(gvImport.DataKeys[gvr.RowIndex].Value);
            GridView gvImpFiles = (GridView)gvr.FindControl("gvImpFiles");
            foreach (GridViewRow row in gvImpFiles.Rows)
            {
                CheckBox cb = (CheckBox)row.FindControl("chkImport2");
                if (chk.Checked)
                {
                    cb.Checked = true;
                }
                else
                {
                    cb.Checked = false;
                }
            }

            //string cellvalue = gvImport.Rows[gvr.RowIndex].Cells[1].Text; 
        }


        #endregion

    }

    public class GVSelection
    {
        public bool expanded { get; set; }
        public string imageId { get; set; }
        public string chkId { get; set; }
        public List<string> fileChkIds { get; set; }
    }

    public class GVProperties
    {
        public int numberOfImportFilesSelected { get; set; }
        public int numberOfDifferntImports { get; set; }
        public int lastId { get; set; }
        public int singleSelectId { get; set; }
        public string singleSelectName { get; set; }
    }
}