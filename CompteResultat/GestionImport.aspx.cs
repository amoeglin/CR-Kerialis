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

namespace CompteResultat
{
    public partial class GestionImport : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            //check the hidden field in each row and if the value is: "expanded", add the id of the corresponding imageID
            //to the string that is used as an arg to call the ExpandImages JS function
            string imageIds = "";
            foreach (GridViewRow row in gvImport.Rows)
            {
                Image imgPlusMinus = (Image)row.FindControl("imgPlusMinus");
                HiddenField hdnState = (HiddenField)row.FindControl("hdnState");
                Control chkImport = (Control)row.FindControl("chkImport");
                int importId = Convert.ToInt32(gvImport.DataKeys[row.RowIndex].Value);

                if (hdnState.Value == "expanded")
                {
                    imageIds += imgPlusMinus.ClientID + ",";
                }

            }

            imageIds = imageIds.TrimEnd(',');

            //string[] imgIds = new string[2];
            //imgIds[0] = id;
            //imgIds[1] = id2;
            //string imageIds = string.Join(",", imgIds);

            ClientScript.RegisterStartupScript(this.GetType(), "Javascript", string.Format("ExpandImages('{0}');", imageIds), true);

        }

        protected void imgPlusMinus_Load(object sender, EventArgs e)
        {
            Image img = (Image)sender;
            string title = img.ToolTip;
            var a = 3;

        }        

        protected void cmdImport_Click(object sender, EventArgs e)
        {
            string userName = "";
            string uploadDirectory = "";
            string importDirectory = "";
            string importName = "";
            bool hasErr = false;
            

            try
            {
                bool atLeastOneRowDeleted = false;
                // Iterate through the Products.Rows property
                foreach (GridViewRow row in gvImport.Rows)
                {
                    // Access the CheckBox
                    CheckBox cb = (CheckBox)row.FindControl("chkImport");

                    int importId = Convert.ToInt32(gvImport.DataKeys[row.RowIndex].Value);    

                    if (cb != null && cb.Checked)
                    {
                        GridView gvImpFiles = (GridView)row.FindControl("gvImpFiles");

                        foreach (GridViewRow row2 in gvImpFiles.Rows)
                        {
                            CheckBox cb2 = (CheckBox)row2.FindControl("chkImport2");
                            importId = Convert.ToInt32(gvImpFiles.DataKeys[row2.RowIndex].Value);
                        }   
                    }
                }





                #region INITIAL CONFIGURATION

                //initial configuration
                userName = User.Identity.Name;
                uploadDirectory = Path.Combine(Request.PhysicalApplicationPath, C.uploadFolder);
                importDirectory = Path.Combine(Request.PhysicalApplicationPath, "Import");

                string prefix = userName + "_";                

                string csvSep = WebConfigurationManager.AppSettings["CSVSEP"];

                #endregion

                //most basic validation
                if (txtNomImport.Text == "")
                    throw new Exception("Il faudra renseigner un nom pour l’import dans le champ 'Nom Import' !");

                //we need to provide at least 1 import file                
                //if (txtPrestPath.Text == "" && txtDemoPath.Text == "" && txtCotPath.Text == "" && txtExpPath.Text == ""
                //    && txtSinistrePrevPath.Text == "" && txtDecompPrevPath.Text == "" && txtCotPrevPath.Text == "" && txtProvPath.Text == "" && txtProvOuverturePath.Text == "")
                //    throw new Exception("Il faudra spécifier au moins un fichier d'import !");


                #region SAVE IMPORTED FILES TO SPECIFIC LOCATION

                //upload provided import files

                string filePath = "";
                //string dateTimeToday = DateTime.Now.ToString("s").Replace(":", "-");
                importDirectory = Path.Combine(Request.PhysicalApplicationPath, "App_Data", "Imports", importName + "-" + DateTime.Now.ToString("s").Replace(":", "-"));
                //Directory.CreateDirectory(importDirectory);

                //if (uploadPrestFile != null && uploadPrestFile.FileName.Length > 0 && txtPrestPath.Text != "")
                //{
                //    uploadPrestFile.SaveAs(uploadPathPrest);
                //    filePath = Path.Combine(importDirectory, txtPrestPath.Text);
                //    uploadPrestFile.SaveAs(filePath);
                //}

                
                #endregion

                #region VALIDATE IMPORT FILES - VERIFY IF ALL REQUIRED FIELDS ARE PRESENT

                //Validate imported files
                List<string> missingColumns;

                //verify PrestaSante Data
                //if (File.Exists(uploadPathPrest))
                //{
                //    missingColumns = BLImport.ImportFileVerification(C.eImportFile.PrestaSante, ref uploadPathPrest, configStringPrest);
                //    if (missingColumns.Count > 0)
                //    {
                //        string errMess = string.Format("Le fichier d'import : {0} manque les colonnes suivantes : {1} ", txtPrestPath.Text, string.Join(", ", missingColumns));
                //        throw new Exception(errMess);
                //    }
                //}            


                #endregion


                //Perform data import
                //BLImport imp = new BLImport(userName, newPrestEntCSV, newPrestProdCSV, newCotEntCSV, newCotProdCSV, newDemoEntCSV, newDemoProdCSV, newOtherFieldsCSV,
                //    newCotPrevCSV, newSinistrePrevCSV, newDecompPrevCSV, newProvCSV, newProvOuvertureCSV,
                //    configStringPrest, configStringDemo, configStringCot, configStringOtherFields, configStringCotPrev, configStringSinistrPrev, configStringDecompPrev, configStringProv,
                //    tableForOtherFields, importName, csvSep, uploadDirectory, importDirectory, uploadPathPrest, uploadPathCot, uploadPathDemo,
                //    uploadPathCotPrev, uploadPathSinistrPrev, uploadPathDecompPrev, uploadPathProv, uploadPathProvOuverture, newExpCSV, configStringExp, uploadPathExp, forceCompanySubsid,
                //    updateGroupes, updateExperience, updateCad, provOuverture);

                //imp.DoImport();

                //refresh the data grid
                //gvImport.DataBind();

            }            
            catch (Exception ex)
            {
                Directory.Delete(importDirectory, true);
                UICommon.HandlePageError(ex, this.Page, "cmdImport_Click");
            }
            finally
            {
                if (!hasErr)
                {
                    //clean all text fields
                    //txtCotPath.Text = "";
                    
                    //uploadExpFile = null;
                }
            }
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

                    int importId = int.Parse(gvImport.DataKeys[e.Row.RowIndex].Value.ToString());
                    GridView gvImpFiles = e.Row.FindControl("gvImpFiles") as GridView;
                    gvImpFiles.DataSource = ImportFile.GetImportFilesForId(importId);
                    // DataSourceID="odsCollege" DataKeyNames="Id"
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

        protected void gvImpFiles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //Import selImport = e.Row.DataItem as Import;
                    //int importId = int.Parse(DataBinder.Eval(e.Row.DataItem, "Id").ToString());

                    //Button cmd = e.Row.FindControl("cmdDelete") as Button;
                    //cmd.CommandArgument = importId.ToString();

                    string importId = gvImport.DataKeys[e.Row.RowIndex].Value.ToString();
                    GridView gvImpFiles = e.Row.FindControl("gvImpFiles") as GridView;
                    //gvImpFiles.DataSource = College.GetColleges();
                    // DataSourceID="odsCollege" DataKeyNames="Id"
                    // GetData(string.Format("select top 3 * from Orders where CustomerId='{0}'", importId));
                    //gvImpFiles.DataBind();
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

        protected void gvImport_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteImp")
            {
                int importId;

                if (Int32.TryParse(e.CommandArgument.ToString(), out importId))
                {
                    BLImport.CleanTablesForSpecificImportID(importId, false);

                    //refresh the data grid
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

        protected void chkImport_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            
            GridViewRow gvr = (GridViewRow)chk.NamingContainer;
            //string id = gvImport.Rows[gvr.RowIndex].Value.ToString();
            int importId = Convert.ToInt32(gvImport.DataKeys[gvr.RowIndex].Value);
            GridView gvImpFiles = (GridView)gvr.FindControl("gvImpFiles");
            foreach (GridViewRow row in gvImpFiles.Rows)
            {
                CheckBox cb = (CheckBox)row.FindControl("chkImport2");
                //check all boxes
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
            //Button cmd = e.Row.FindControl("cmdDelete") as Button;

                //GridView gvImpFiles = (GridView)row.FindControl("gvImpFiles");
                //foreach (GridViewRow row in gvImpFiles.Rows)
                //{
                //    // Access the CheckBox
                //    CheckBox cb = (CheckBox)row.FindControl("chkImport");

                //    int importId = Convert.ToInt32(gvImport.DataKeys[row.RowIndex].Value);

                //    if (cb != null && cb.Checked)
                //    {
                //        GridView gvImpFiles = (GridView)row.FindControl("gvImpFiles");

                //        foreach (GridViewRow row2 in gvImpFiles.Rows)
                //        {
                //            CheckBox cb2 = (CheckBox)row2.FindControl("chkImport2");
                //            importId = Convert.ToInt32(gvImpFiles.DataKeys[row2.RowIndex].Value);
                //        }
                //    }
                //}

        }

        
    }
}