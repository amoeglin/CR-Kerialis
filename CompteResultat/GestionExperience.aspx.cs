using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using CompteResultat.BL;
using CompteResultat.DAL;
using CompteResultat.Common;
using System.Web.DynamicData;
using System.Web.ModelBinding;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace CompteResultat
{
    public partial class GestionExperience : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                cmdImport.Attributes.Add("onclick", "jQuery('#" + uploadExcel.ClientID + "').click();return false;");

                if (!IsPostBack)
                {
                    rptExp.DataBind();
                }
                else
                {
                    //Handle the Delete Event
                    if (Request.Form["cmdDelete"] != null) { }

                    //Handle the import of groups & guarantees
                    if (uploadExcel.PostedFile != null)
                    {
                        if (uploadExcel.PostedFile.FileName.Length > 0)
                        {
                            //Import Garanties
                            string excelFile = Path.GetFileName(uploadExcel.PostedFile.FileName);

                            string excelDirectory = Path.Combine(Request.PhysicalApplicationPath, C.excelFolder);
                            string fullUploadPath = Path.Combine(excelDirectory, excelFile);
                            uploadExcel.PostedFile.SaveAs(fullUploadPath);
                            
                            string assurId = lbAssur.SelectedItem.Value.ToString();
                            string assurName = lbAssur.SelectedItem.Text.ToString();
                            int iAssurId = -1;

                            if (int.TryParse(assurId, out iAssurId))
                            {
                                if (iAssurId == -1)
                                    return;

                                BLExperience.ImportExperienceForAssureur(assurName, fullUploadPath, true);

                                //refresh the tree
                                lbAssur.DataBind();

                                if (ItemExists(assurName))
                                {
                                    SelectItem(assurName);
                                    rptExp.DataBind();
                                }
                                else
                                {
                                    if (lbAssur.Items.Count > 0)
                                    {
                                        SelectItem(lbAssur.Items[0].Text);
                                        rptExp.DataBind();
                                    }
                                    else
                                        rptExp.DataBind();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionExperience::Page_Load"); }
        }

        public IEnumerable<C_TempExpData> GetExperience()
        {
            string assurName;

            try
            {
                if (lbAssur.SelectedItem != null)
                {
                    assurName = lbAssur.SelectedItem.Text.ToString();
                    //###
                    return C_TempExpData.GetExpDataForAssureur(assurName);
                }
                
                return null;                
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionExperience::GetExperience"); return null; }
        }

        public List<Assureur> GetAssureurs()
        {
            try
            {
                List<Assureur> assur;

                assur = Assureur.GetAllAssureurs();                    

                return assur;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionExperience::GetAssureurs"); return null; }
        }        

        protected void lbAssur_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lbAssur.SelectedItem != null)
                {
                    Session["SelectedAssureurIndex"] = lbAssur.SelectedIndex;
                    Session["SelectedAssureurName"] = lbAssur.SelectedItem.Text;

                    string assurId = lbAssur.SelectedItem.Value.ToString();
                    string assurName = lbAssur.SelectedItem.Text.ToString();

                    rptExp.DataBind();
                    //UpdateTreeView(assurName);
                }
                else
                {
                    if (lbAssur.Items.Count > 0)
                        lbAssur.Items[0].Selected = true;
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionExperience::lbAssur_SelectedIndexChanged"); }
        }

        protected void cmdExport_Click(object sender, EventArgs e)
        {
            string uploadPath = "";

            try
            {
                string assurName = lbAssur.SelectedItem.Text.ToString();

                //List<C_TempExpData> groupsSante = C_TempExpData.GetExpData(assurName); 
                ExcelPackage pack = BLExperience.ExportExperienceForAssureur(assurName);

                uploadPath = Path.Combine(Request.PhysicalApplicationPath, C.uploadFolder);
                uploadPath = Path.Combine(uploadPath, User.Identity.Name + "_" + assurName + "_Experience.xlsx");

                pack.SaveAs(new FileInfo(uploadPath));

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

        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            try
            {
                //handle the delete event
                string assurName = lbAssur.SelectedItem.Text.ToString();

                Session["SelectedAssureurIndex"] = lbAssur.SelectedIndex;

                if (!string.IsNullOrWhiteSpace(assurName))
                    C_TempExpData.DeleteExperienceWithSpecificAssureurName(assurName);
                else
                    throw new Exception("Please select the name of the 'Assureur' for which you want to delete the Experience!");

                //refresh the tree
                lbAssur.DataBind();

                if (ItemExists(assurName))
                {
                    SelectItem(assurName);
                    rptExp.DataBind();
                }
                else
                {
                    if (lbAssur.Items.Count > 0)
                    {
                        SelectItem(lbAssur.Items[0].Text);
                        rptExp.DataBind();
                    }
                    else
                        rptExp.DataBind();
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionExperience::cmdDelete_Click"); }
        }

        protected void cmdRecreate_Click(object sender, EventArgs e)
        {
            try
            {
                //BLGroupsAndGaranties.RecreateGroupsGarantiesSanteFromPresta();
                BLExperience.RecreateExperienceFromPresta();

                rptExp.DataBind();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);

                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                Page.Validators.Add(myCustomValidator);
            }
        }

        protected void lbAssur_DataBound(object sender, EventArgs e)
        {
            try
            {
                //if no item is selected, select the first item in the list
                if (lbAssur.SelectedIndex == -1 && lbAssur.Items.Count > 0)
                {
                    SelectItem(lbAssur.Items[0].Text);
                    rptExp.DataBind();
                }

                EnableDisableButtons();
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionExperience::lbAssur_DataBound"); }
        }

        protected void rptExp_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Repeater rpt = sender as Repeater; // Get the Repeater control object.
            //PlaceHolder phHeader;

            //if (e.Item.ItemType == ListItemType.Header)
            //{
            //    //phHeader
            //    phHeader = e.Item.FindControl("phHeader") as PlaceHolder;
            //    if (phHeader != null)
            //    {
            //        phHeader.Visible = true;
            //    }
            //}

            // If the Repeater contains no data.
            if (rpt != null )
            {
                if (e.Item.ItemType == ListItemType.Footer)
                {
                    // Show info Label (if no data is present).
                    //Label lblNoData = e.Item.FindControl("lblEmpty") as Label;
                    //if (lblNoData != null)
                    //{
                    //    lblNoData.Visible = true;
                    //}

                    if (rpt.Items.Count < 1)
                    {
                        rptExp.Visible = false;
                        phHeader.Visible = true;
                    }
                    else
                    {
                        rptExp.Visible = true;
                        phHeader.Visible = false;
                    }
                }

                //if (e.Item.ItemType == ListItemType.Header)
                //{
                //    //phHeader
                //    PlaceHolder phHeader = e.Item.FindControl("phHeader") as PlaceHolder;
                //    if (phHeader != null)
                //    {
                //        phHeader.Visible = false;
                //    }
                //}
            }  
        }


        #region Private Methods

        private void EnableDisableButtons()
        {
            try
            {
                if (lbAssur.SelectedItem == null)
                {
                    cmdExport.Enabled = false;
                    cmdImport.Enabled = false;
                    cmdDelete.Enabled = false;
                }
                else
                {
                    cmdExport.Enabled = true;
                    cmdImport.Enabled = true;
                    cmdDelete.Enabled = true;
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionExperience::EnableDisableButtons"); }
        }

        private void SelectItem(string itemName)
        {
            try
            {
                foreach (ListItem li in lbAssur.Items)
                {
                    if (li.Text == itemName)
                        li.Selected = true;
                    else
                        li.Selected = false;
                }
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionExperience::SelectItem"); }
        }

        private bool ItemExists(string itemName)
        {
            try
            {
                foreach (ListItem li in lbAssur.Items)
                {
                    if (li.Text == itemName)
                        return true;
                }

                return false;
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionExperience::ItemExists"); return false; }
        }




        #endregion
        
    }
}