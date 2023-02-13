using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.Configuration;
using System.Data;

using CompteResultat.BL;
using CompteResultat.DAL;
using CompteResultat.Common;
using System.Web.DynamicData;
using System.Web.ModelBinding;

namespace CompteResultat
{
    public partial class ImportData : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string importName;
        private string provOuverture; 
        private HttpPostedFile uploadPrestFile;
        private HttpPostedFile uploadCotFile;
        private HttpPostedFile uploadDemoFile;
        private HttpPostedFile uploadCotPrevFile;
        private HttpPostedFile uploadProvFile;
        private HttpPostedFile uploadProvOuvertureFile;
        private HttpPostedFile uploadDecompPrevFile;
        private HttpPostedFile uploadSinistrePrevFile;
        private HttpPostedFile uploadExpFile;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {                
                //string fileToAnalyse = Request.QueryString["fileToAnalyse"];  
                if(Session["singleFileUpload"] != null && Session["singleFileUpload"] != "" )
                {
                    string fileToAnalyse = Session["singleFileUpload"] as string;
                    Session["singleFileUpload"] = "";
                    pnlAnalyse.Visible = true;

                    litAnalyse.Text = BLAnalyse.ManualFileAnalyse(fileToAnalyse);                    
                }

                cmdSelectPrest.Attributes.Add("onclick", "jQuery('#" + uplPrest.ClientID + "').click();return false;");
                cmdSelectCot.Attributes.Add("onclick", "jQuery('#" + uplCot.ClientID + "').click();return false;");
                cmdSelectDemo.Attributes.Add("onclick", "jQuery('#" + uplDemo.ClientID + "').click();return false;");

                cmdSelectCotPrev.Attributes.Add("onclick", "jQuery('#" + uplCotPrev.ClientID + "').click();return false;");
                cmdSelectSinistrePrev.Attributes.Add("onclick", "jQuery('#" + uplSinistrePrev.ClientID + "').click();return false;");
                cmdSelectDecompPrev.Attributes.Add("onclick", "jQuery('#" + uplDecompPrev.ClientID + "').click();return false;");
                cmdSelectProv.Attributes.Add("onclick", "jQuery('#" + uplProv.ClientID + "').click();return false;");
                cmdSelectProvOuverture.Attributes.Add("onclick", "jQuery('#" + uplProvOuverture.ClientID + "').click();return false;");

                cmdSelectExp.Attributes.Add("onclick", "jQuery('#" + uplExp.ClientID + "').click();return false;");

                if (!IsPostBack)
                {
                    int year = DateTime.Now.Year;
                    if(txtProvOuvertureDate.Text == "")
                        txtProvOuvertureDate.Text = (year-1).ToString() + "-01-01";

                    gvImport.Sort("Date", SortDirection.Descending);

                    Session[C.eUploadSessionVar.UploadPrestFile.ToString()] = null;
                    Session[C.eUploadSessionVar.UploadCotFile.ToString()] = null;
                    Session[C.eUploadSessionVar.UploadDemoFile.ToString()] = null;

                    Session[C.eUploadSessionVar.UploadCotPrevFile.ToString()] = null;
                    Session[C.eUploadSessionVar.UploadDecompPrevFile.ToString()] = null;
                    Session[C.eUploadSessionVar.UploadSinistrePrevFile.ToString()] = null;
                    Session[C.eUploadSessionVar.UploadProvFile.ToString()] = null;
                    Session[C.eUploadSessionVar.UploadProvOuvertureFile.ToString()] = null;

                    Session[C.eUploadSessionVar.UploadExpFile.ToString()] = null;
                }

                if (IsPostBack)
                {
                    importName = txtNomImport.Text;
                    provOuverture = txtProvOuvertureDate.Text;

                    if (uplPrest.PostedFile != null && uplPrest.PostedFile.FileName.Length > 0)
                    {
                        txtPrestPath.Text = Path.GetFileName(uplPrest.PostedFile.FileName);
                        Session[C.eUploadSessionVar.UploadPrestFile.ToString()] = uplPrest.PostedFile;
                    }

                    if (uplCot.PostedFile != null && uplCot.PostedFile.FileName.Length > 0)
                    {
                        txtCotPath.Text = Path.GetFileName(uplCot.PostedFile.FileName);
                        Session[C.eUploadSessionVar.UploadCotFile.ToString()] = uplCot.PostedFile;
                    }

                    if (uplDemo.PostedFile != null && uplDemo.PostedFile.FileName.Length > 0)
                    {
                        txtDemoPath.Text = Path.GetFileName(uplDemo.PostedFile.FileName);
                        Session[C.eUploadSessionVar.UploadDemoFile.ToString()] = uplDemo.PostedFile;
                    }

                    if (uplSinistrePrev.PostedFile != null && uplSinistrePrev.PostedFile.FileName.Length > 0)
                    {
                        txtSinistrePrevPath.Text = Path.GetFileName(uplSinistrePrev.PostedFile.FileName);
                        Session[C.eUploadSessionVar.UploadSinistrePrevFile.ToString()] = uplSinistrePrev.PostedFile;
                    }

                    if (uplCotPrev.PostedFile != null && uplCotPrev.PostedFile.FileName.Length > 0)
                    {
                        //string fileOnly = Path.GetFileName(uplCotPrev.PostedFile.FileName);
                        //txtCotPrevPath.Text = fileOnly;
                        txtCotPrevPath.Text = Path.GetFileName(uplCotPrev.PostedFile.FileName);
                        Session[C.eUploadSessionVar.UploadCotPrevFile.ToString()] = uplCotPrev.PostedFile;
                    }

                    if (uplDecompPrev.PostedFile != null && uplDecompPrev.PostedFile.FileName.Length > 0)
                    {
                        txtDecompPrevPath.Text = Path.GetFileName(uplDecompPrev.PostedFile.FileName);
                        Session[C.eUploadSessionVar.UploadDecompPrevFile.ToString()] = uplDecompPrev.PostedFile;
                    }

                    if (uplProv.PostedFile != null && uplProv.PostedFile.FileName.Length > 0)
                    {
                        txtProvPath.Text = Path.GetFileName(uplProv.PostedFile.FileName);
                        Session[C.eUploadSessionVar.UploadProvFile.ToString()] = uplProv.PostedFile;
                    }

                    if (uplProvOuverture.PostedFile != null && uplProvOuverture.PostedFile.FileName.Length > 0)
                    {
                        txtProvOuverturePath.Text = Path.GetFileName(uplProvOuverture.PostedFile.FileName);
                        Session[C.eUploadSessionVar.UploadProvOuvertureFile.ToString()] = uplProvOuverture.PostedFile;
                    }

                    if (uplExp.PostedFile != null && uplExp.PostedFile.FileName.Length > 0)
                    {
                        txtExpPath.Text = Path.GetFileName(uplExp.PostedFile.FileName);
                        Session[C.eUploadSessionVar.UploadExpFile.ToString()] = uplExp.PostedFile;
                    }
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
        
        public class RowTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                Literal lit = new Literal();
                container.Controls.Add(lit);

                container.DataBinding += (src, args) =>
                {
                    IDataItemContainer dc = ((IDataItemContainer)container);
                    Import imp = (Import)dc.DataItem;
                    lit.Text =
                        string.Format("<tr {0}><td>{1}</td><td>{2}</td><td>{3}</td></tr>",
                        dc.DataItemIndex % 2 == 1 ? "class=\"alternate\"" : string.Empty,
                        imp.Name, imp.Date, imp.UserName);
                };
            }
        }

        protected void imgDelPrestSante_Click(object sender, ImageClickEventArgs e)
        {
            try
            {                
                txtPrestPath.Text = ""; 
                uploadPrestFile = null;                
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "imgDelPrestSante_Click"); }
        }

        protected void imgSelectCot_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtCotPath.Text = "";
                uploadCotFile = null;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "imgSelectCot_Click"); }
        }

        protected void imgSelectDemo_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtDemoPath.Text = "";
                uploadDemoFile = null;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "imgSelectDemo_Click"); }
        }

        protected void imgSelectDecompPrev_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtDecompPrevPath.Text = "";
                uploadDecompPrevFile = null;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "imgSelectDecompPrev_Click"); }
        }

        protected void imgSelectSinistrePrev_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtSinistrePrevPath.Text = "";
                uploadSinistrePrevFile = null;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "imgSelectSinistrePrev_Click"); }
        }

        protected void imgSelectCotPrev_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtCotPrevPath.Text = "";
                uploadCotPrevFile = null;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "imgSelectCotPrev_Click"); }
        }

        protected void imgSelectProv_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtProvPath.Text = "";
                uploadProvFile = null;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "imgSelectProv_Click"); }
        }

        protected void imgSelectExp_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtExpPath.Text = "";
                uploadExpFile = null;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "imgSelectExp_Click"); }
        }

        protected void imgSelectProvOuverture_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                txtProvOuverturePath.Text = "";
                uploadProvOuvertureFile = null;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "imgSelectProvOuverture_Click"); }
        }

        protected void cmdImport_Click(object sender, EventArgs e)
        {
            string userName = "";
            string uploadDirectory = "";
            string importDirectory = "";
            string analyseDirectory = "";
            bool hasErr = false;
            bool forceCompanySubsid = chkForceCompSubsid.Checked;
            bool updateGroupes = chkGroupes.Checked;
            bool updateExperience = chkExp.Checked;
            bool updateCad = chkCad.Checked;
            bool analyseData = chkAnalyse.Checked;

            try
            {
                #region INITIAL CONFIGURATION

                //initial configuration
                userName = User.Identity.Name;
                uploadDirectory = Path.Combine(Request.PhysicalApplicationPath, C.uploadFolder);
                importDirectory = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "Imports");
                analyseDirectory = Path.Combine(Request.PhysicalApplicationPath, "Analyse");

                string prefix = userName + "_";
                string uploadPathPrest = Path.Combine(uploadDirectory, prefix + txtPrestPath.Text);
                string uploadPathCot = Path.Combine(uploadDirectory, prefix + txtCotPath.Text);
                string uploadPathDemo = Path.Combine(uploadDirectory, prefix + txtDemoPath.Text);
                string uploadPathCotPrev = Path.Combine(uploadDirectory, prefix + txtCotPrevPath.Text);
                string uploadPathSinistrPrev = Path.Combine(uploadDirectory, prefix + txtSinistrePrevPath.Text);
                string uploadPathDecompPrev = Path.Combine(uploadDirectory, prefix + txtDecompPrevPath.Text);
                string uploadPathProv = Path.Combine(uploadDirectory, prefix + txtProvPath.Text);
                string uploadPathProvOuverture = Path.Combine(uploadDirectory, prefix + txtProvOuverturePath.Text);
                string uploadPathExp = Path.Combine(uploadDirectory, prefix + txtExpPath.Text);

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

                uploadPrestFile = (HttpPostedFile)Session[C.eUploadSessionVar.UploadPrestFile.ToString()];
                uploadCotFile = (HttpPostedFile)Session[C.eUploadSessionVar.UploadCotFile.ToString()];
                uploadDemoFile = (HttpPostedFile)Session[C.eUploadSessionVar.UploadDemoFile.ToString()];
                uploadCotPrevFile = (HttpPostedFile)Session[C.eUploadSessionVar.UploadCotPrevFile.ToString()];
                uploadDecompPrevFile = (HttpPostedFile)Session[C.eUploadSessionVar.UploadDecompPrevFile.ToString()];
                uploadProvFile = (HttpPostedFile)Session[C.eUploadSessionVar.UploadProvFile.ToString()];
                uploadProvOuvertureFile = (HttpPostedFile)Session[C.eUploadSessionVar.UploadProvOuvertureFile.ToString()];
                uploadSinistrePrevFile = (HttpPostedFile)Session[C.eUploadSessionVar.UploadSinistrePrevFile.ToString()];
                uploadExpFile = (HttpPostedFile)Session[C.eUploadSessionVar.UploadExpFile.ToString()];

                string csvSep = WebConfigurationManager.AppSettings["CSVSEP"];

                #endregion

                //most basic validation
                if (txtNomImport.Text == "")
                    throw new Exception("Il faudra renseigner un nom pour l’import dans le champ 'Nom Import' !");

                //we need to provide at least 1 import file                
                if (txtPrestPath.Text == "" && txtDemoPath.Text == "" && txtCotPath.Text == "" && txtExpPath.Text == ""
                    && txtSinistrePrevPath.Text == "" && txtDecompPrevPath.Text == "" && txtCotPrevPath.Text == "" && txtProvPath.Text == "" && txtProvOuverturePath.Text == "")
                    throw new Exception("Il faudra spécifier au moins un fichier d'import !");


                //### the following is no longer required
                //make sure  a file is provided for the other fields table
                //### add PREV fields
                //if (tableForOtherFields == C.eImportFile.PrestaSante.ToString())                
                //    if (txtPrestPath.Text == "")
                //        throw new Exception("Il faudra spécifier un fichier du type .csv ou .xlsx dans le champ 'Fichier CSV Prestations' !");
                //if (tableForOtherFields == C.eImportFile.CotisatSante.ToString())
                //    if (txtCotPath.Text == "")
                //        throw new Exception("Il faudra spécifier un fichier du type .csv ou .xlsx dans le champ 'Fichier CSV Cotisations' !");
                //if (tableForOtherFields == C.eImportFile.Demography.ToString())
                //    if (txtDemoPath.Text == "")
                //        throw new Exception("Il faudra spécifier un fichier du type .csv ou .xlsx dans le champ 'Fichier CSV Démographie' !");

                #region SAVE IMPORTED FILES TO SPECIFIC LOCATION

                //upload provided import files

                string filePath = "";
                //string dateTimeToday = DateTime.Now.ToString("s").Replace(":", "-");
                importDirectory = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "Imports", importName + "-" + DateTime.Now.ToString("s").Replace(":", "-"));
                Directory.CreateDirectory(importDirectory);

                //Delete Aalyse directory & re-create it
                analyseDirectory = Path.Combine(Request.PhysicalApplicationPath, "Analyse", importName);
                BLImport.CleanupImportDirectory(analyseDirectory);
                Directory.CreateDirectory(analyseDirectory);

                if (uploadPrestFile != null && uploadPrestFile.FileName.Length > 0 && txtPrestPath.Text != "")
                {                    
                    uploadPrestFile.SaveAs(uploadPathPrest);
                    filePath = Path.Combine(importDirectory, txtPrestPath.Text);
                    uploadPrestFile.SaveAs(filePath);
                }

                if (uploadCotFile != null && uploadCotFile.FileName.Length > 0 && txtCotPath.Text != "")
                {
                    uploadCotFile.SaveAs(uploadPathCot);                    
                    filePath = Path.Combine(importDirectory, txtCotPath.Text);
                    uploadCotFile.SaveAs(filePath);
                }

                if (uploadDemoFile != null && uploadDemoFile.FileName.Length > 0 && txtDemoPath.Text != "")
                {
                    uploadDemoFile.SaveAs(uploadPathDemo);
                    filePath = Path.Combine(importDirectory, txtDemoPath.Text);
                    uploadDemoFile.SaveAs(filePath);
                }

                if (uploadSinistrePrevFile != null && uploadSinistrePrevFile.FileName.Length > 0 && txtSinistrePrevPath.Text != "")
                {
                    uploadSinistrePrevFile.SaveAs(uploadPathSinistrPrev);
                    filePath = Path.Combine(importDirectory, txtSinistrePrevPath.Text);
                    uploadSinistrePrevFile.SaveAs(filePath);
                }

                if (uploadDecompPrevFile != null && uploadDecompPrevFile.FileName.Length > 0 && txtDecompPrevPath.Text != "")
                {
                    uploadDecompPrevFile.SaveAs(uploadPathDecompPrev);
                    filePath = Path.Combine(importDirectory, txtDecompPrevPath.Text);
                    uploadDecompPrevFile.SaveAs(filePath);
                }

                if (uploadProvFile != null && uploadProvFile.FileName.Length > 0 && txtProvPath.Text != "")
                {
                    uploadProvFile.SaveAs(uploadPathProv);
                    filePath = Path.Combine(importDirectory, txtProvPath.Text);
                    uploadProvFile.SaveAs(filePath);
                }

                if (uploadProvOuvertureFile != null && uploadProvOuvertureFile.FileName.Length > 0 && txtProvOuverturePath.Text != "")
                {
                    uploadProvOuvertureFile.SaveAs(uploadPathProvOuverture);
                    filePath = Path.Combine(importDirectory, txtProvOuverturePath.Text);
                    uploadProvOuvertureFile.SaveAs(filePath);
                }

                if (uploadCotPrevFile != null && uploadCotPrevFile.FileName.Length > 0 && txtCotPrevPath.Text != "")
                {
                    uploadCotPrevFile.SaveAs(uploadPathCotPrev);
                    filePath = Path.Combine(importDirectory, txtCotPrevPath.Text);
                    uploadCotPrevFile.SaveAs(filePath);
                }

                if (uploadExpFile != null && uploadExpFile.FileName.Length > 0 && txtExpPath.Text != "")
                {
                    uploadExpFile.SaveAs(uploadPathExp);
                    filePath = Path.Combine(importDirectory, txtExpPath.Text);
                    uploadExpFile.SaveAs(filePath);
                }                

                #endregion

                #region VALIDATE IMPORT FILES - VERIFY IF ALL REQUIRED FIELDS ARE PRESENT

                //Validate imported files
                List<string> missingColumns;

                //verify PrestaSante Data
                if (File.Exists(uploadPathPrest))
                {
                    missingColumns = BLImport.ImportFileVerification(C.eImportFile.PrestaSante, ref uploadPathPrest, configStringPrest);
                    if (missingColumns.Count > 0)
                    {
                        string errMess = string.Format("Le fichier d'import : {0} manque les colonnes suivantes : {1} ", txtPrestPath.Text, string.Join(", ", missingColumns));
                        throw new Exception(errMess);
                    }
                }

                //verify CotisatSante Data
                if (File.Exists(uploadPathCot))
                {
                    missingColumns = BLImport.ImportFileVerification(C.eImportFile.CotisatSante, ref uploadPathCot, configStringCot);
                    if (missingColumns.Count > 0)
                    {
                        string errMess = string.Format("Le fichier d'import : {0} manque les colonnes suivantes : {1} ", txtCotPath.Text, string.Join(", ", missingColumns));
                        throw new Exception(errMess);
                    }
                }

                //verify Demo Data
                if (File.Exists(uploadPathDemo))
                {
                    missingColumns = BLImport.ImportFileVerification(C.eImportFile.Demography, ref uploadPathDemo, configStringDemo);
                    if (missingColumns.Count > 0)
                    {
                        string errMess = string.Format("Le fichier d'import : {0} manque les colonnes suivantes : {1} ", txtDemoPath.Text, string.Join(", ", missingColumns));
                        throw new Exception(errMess);
                    }
                }

                //verify CotisatPrev Data
                if (File.Exists(uploadPathCotPrev))
                {
                    missingColumns = BLImport.ImportFileVerification(C.eImportFile.CotisatPrev, ref uploadPathCotPrev, configStringCotPrev);
                    if (missingColumns.Count > 0)
                    {
                        string errMess = string.Format("Le fichier d'import : {0} manque les colonnes suivantes : {1} ", txtCotPrevPath.Text, string.Join(", ", missingColumns));
                        throw new Exception(errMess);
                    }
                }

                //verify SinistrePrev Data
                if (File.Exists(uploadPathSinistrPrev))
                {
                    missingColumns = BLImport.ImportFileVerification(C.eImportFile.SinistrePrev, ref uploadPathSinistrPrev, configStringSinistrPrev);
                    if (missingColumns.Count > 0)
                    {
                        string errMess = string.Format("Le fichier d'import : {0} manque les colonnes suivantes : {1} ", txtSinistrePrevPath.Text, string.Join(", ", missingColumns));
                        throw new Exception(errMess);
                    }
                }

                //verify DecomptePrev Data
                if (File.Exists(uploadPathDecompPrev))
                {
                    missingColumns = BLImport.ImportFileVerification(C.eImportFile.DecompPrev, ref uploadPathDecompPrev, configStringDecompPrev);
                    if (missingColumns.Count > 0)
                    {
                        string errMess = string.Format("Le fichier d'import : {0} manque les colonnes suivantes : {1} ", txtDecompPrevPath.Text, string.Join(", ", missingColumns));
                        throw new Exception(errMess);
                    }
                }

                //verify Provisions Data
                if (File.Exists(uploadPathProv))
                {
                    missingColumns = BLImport.ImportFileVerification(C.eImportFile.Provisions, ref uploadPathProv, configStringProv);
                    if (missingColumns.Count > 0)
                    {
                        string errMess = string.Format("Le fichier d'import : {0} manque les colonnes suivantes : {1} ", txtProvPath.Text, string.Join(", ", missingColumns));
                        throw new Exception(errMess);
                    }
                }

                //verify Provisions Ouverture Data
                if (File.Exists(uploadPathProvOuverture))
                {
                    missingColumns = BLImport.ImportFileVerification(C.eImportFile.Provisions, ref uploadPathProvOuverture, configStringProv);
                    if (missingColumns.Count > 0)
                    {
                        string errMess = string.Format("Le fichier d'import : {0} manque les colonnes suivantes : {1} ", txtProvOuverturePath.Text, string.Join(", ", missingColumns));
                        throw new Exception(errMess);
                    }
                }

                //verify Experience Data
                if (File.Exists(uploadPathExp))
                {
                    missingColumns = BLImport.ImportFileVerification(C.eImportFile.Exp, ref uploadPathExp, configStringExp);
                    if (missingColumns.Count > 0)
                    {
                        string errMess = string.Format("Le fichier d'import : {0} manque les colonnes suivantes : {1} ", txtExpPath.Text, string.Join(", ", missingColumns));
                        throw new Exception(errMess);
                    }
                }

                #endregion


                //### Other Validations ? configStrings not empty, files & directories exist

                //Perform data import
                BLImport imp = new BLImport(userName, newPrestEntCSV, newPrestProdCSV, newCotEntCSV, newCotProdCSV, newDemoEntCSV, newDemoProdCSV, newOtherFieldsCSV, 
                    newCotPrevCSV, newSinistrePrevCSV, newDecompPrevCSV, newProvCSV, newProvOuvertureCSV,
                    configStringPrest, configStringDemo, configStringCot, configStringOtherFields, configStringCotPrev, configStringSinistrPrev, configStringDecompPrev, configStringProv,
                    tableForOtherFields, importName, csvSep, uploadDirectory, importDirectory, uploadPathPrest, uploadPathCot, uploadPathDemo,
                    uploadPathCotPrev, uploadPathSinistrPrev, uploadPathDecompPrev, uploadPathProv, uploadPathProvOuverture, newExpCSV, configStringExp, uploadPathExp, forceCompanySubsid, 
                    updateGroupes, updateExperience, updateCad, analyseData, provOuverture);

                imp.DoImport();

                //refresh the data grid
                gvImport.DataBind();

            }
            //catch (Exception ex)
            //{
            //    hasErr = true;
            //    BLImport.CleanupImportFiles(uploadDirectory, userName);

            //    var myCustomValidator = new CustomValidator();
            //    myCustomValidator.IsValid = false;
            //    myCustomValidator.ErrorMessage = ex.Message;
            //    Page.Validators.Add(myCustomValidator);
            //}
            catch (Exception ex) {
                if(Directory.Exists(importDirectory))
                    Directory.Delete(importDirectory, true);

                UICommon.HandlePageError(ex, this.Page, "cmdImport_Click");
            }
            finally
            {
                if (!hasErr)
                {
                    //clean all text fields
                    txtCotPath.Text = "";
                    txtCotPrevPath.Text = "";
                    txtDecompPrevPath.Text = "";
                    txtProvPath.Text = "";
                    txtProvOuverturePath.Text = "";
                    txtDemoPath.Text = "";
                    txtNomImport.Text = "";
                    txtPrestPath.Text = "";
                    txtSinistrePrevPath.Text = "";
                    txtExpPath.Text = "";

                    uploadPrestFile = null;
                    uploadCotFile = null;
                    uploadDemoFile = null;
                    uploadCotPrevFile = null;
                    uploadDecompPrevFile = null;
                    uploadProvFile = null;
                    uploadProvOuvertureFile = null;
                    uploadSinistrePrevFile = null;
                    uploadExpFile = null;
                }
            }
        }        
        
        protected void gvImport_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteImp")
            {
                int importId;

                if (Int32.TryParse(e.CommandArgument.ToString(), out importId))
                {                    
                    BLImport.CleanTablesForSpecificImportID(importId, true, false);                    
                    gvImport.DataBind();
                }
            }
            if (e.CommandName == "RedirectFMImport")
            {
                string importPath = e.CommandArgument.ToString();

                if (importPath != "")
                {
                    Response.Redirect("~/FMImport.aspx?path=" + importPath);
                }
            }
        }

        


        #region NOT REQUIRED

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





        #endregion

        
    }
}