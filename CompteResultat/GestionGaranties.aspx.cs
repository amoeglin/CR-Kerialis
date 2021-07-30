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
    public partial class GestionGaranties : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                cmdImport.Attributes.Add("onclick", "jQuery('#" + uploadExcel.ClientID + "').click();return false;");                

                if (!IsPostBack )
                {
                    lbAssur.SelectedIndex = 0;
                    UpdateTreeView(C.cDEFAULTASSUREUR);
                }
                else
                {                    
                    //Handle the Delete Event
                    if (Request.Form["cmdDelete"] != null) {  }                       

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

                            //string compId = lbCompanies.SelectedItem.Value.ToString();
                            //int iCompId = -1;
                            string assurId = lbAssur.SelectedItem.Value.ToString();
                            string assurName = lbAssur.SelectedItem.Text.ToString();
                            int iAssurId = -1;

                            if (int.TryParse(assurId, out iAssurId))
                            {
                                if (iAssurId == -1)
                                    assurName = C.cDEFAULTASSUREUR;
                                BLGroupsAndGaranties.ImportGroupsGarantiesSanteForAssureur(assurName, fullUploadPath, true);

                                //refresh the tree
                                lbAssur.DataBind();

                                if (ItemExists(assurName))
                                {
                                    SelectItem(assurName);
                                    UpdateTreeView(assurName);
                                }
                                else
                                {
                                    if (lbAssur.Items.Count > 0)
                                    {
                                        SelectItem(lbAssur.Items[0].Text);
                                        UpdateTreeView(lbAssur.Items[0].Text);
                                    }
                                    else
                                        tvGaranties.Nodes.Clear();
                                }


                                //lbAssur.DataBind();
                                //string selAssur = Session["SelectedAssureurName"].ToString();
                                //SelectItem(selAssur);
                            }
                        }
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

        protected override void OnPreRender(EventArgs e)
        {            
            
        }

        public List<Assureur> GetAssureurs([Control] bool chkAssur)
        {
            try
            {
                List<Assureur> assur;

                if (chkAssur)
                {
                    assur = Assureur.GetAssureursWithoutGroupsAndGarantiesSante();                    
                }
                else
                {
                    assur = Assureur.GetAllAssureurs();
                    assur.Insert(0, new Assureur { Id = -1, Name = C.cDEFAULTASSUREUR });
                }                

                return assur;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);

                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                Page.Validators.Add(myCustomValidator);

                return null;
            }
        }

        private void UpdateTreeView(string assureurName)
        {
            try
            {
                tvGaranties.Nodes.Clear();
                
                List<GroupGarantySante> ggs = GroupGarantySante.GetGroupsAndGarantiesForAssureur(assureurName);

                // add all notes for Sante
                TreeNode nodeSante = new TreeNode("Groups et garanties pour : " + assureurName, "");
                nodeSante.SelectAction = TreeNodeSelectAction.Expand;
                tvGaranties.Nodes.Add(nodeSante);

                //get all groups
                List<string> allGroups = ggs.OrderBy(g => g.GroupName).Select(g => g.GroupName).Distinct().ToList();

                foreach (string group in allGroups)
                {
                    if (!string.IsNullOrWhiteSpace(group))
                    {
                        TreeNode groupNode = new TreeNode(group, "");

                        //get garanties for group
                        List<string> garanties = ggs.Where(g => g.GroupName == group).OrderBy(g => g.GarantyName).Select(g => g.GarantyName).Distinct().ToList();

                        foreach (string gar in garanties)
                        {
                            if (!string.IsNullOrWhiteSpace(gar))
                            {
                                TreeNode garantyNode = new TreeNode(gar, "");
                                groupNode.ChildNodes.Add(garantyNode);
                            }
                        }                        

                        groupNode.Expand();
                        nodeSante.ChildNodes.Add(groupNode);
                    }
                }

                nodeSante.Expand();

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
        
        protected void lbAssur_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbAssur.SelectedItem != null)
            {
                Session["SelectedAssureurIndex"] = lbAssur.SelectedIndex;
                Session["SelectedAssureurName"] = lbAssur.SelectedItem.Text;

                string assurId = lbAssur.SelectedItem.Value.ToString();
                string assurName = lbAssur.SelectedItem.Text.ToString();

                UpdateTreeView(assurName);
            }
            else
            {
                if (lbAssur.Items.Count > 0)
                    lbAssur.Items[0].Selected = true;
            }
        }

        protected void cmdExport_Click(object sender, EventArgs e)
        {
            string uploadPath = "";

            try
            {                
                string assurName = lbAssur.SelectedItem.Text.ToString();

                List<GroupGarantySante> groupsSante = GroupGarantySante.GetGroupsAndGarantiesForAssureur(assurName); 
                ExcelPackage pack = BLGroupsAndGaranties.ExportGroupsGarantiesSanteForAssureur(assurName);
                
                uploadPath = Path.Combine(Request.PhysicalApplicationPath, C.uploadFolder);
                uploadPath = Path.Combine(uploadPath, User.Identity.Name + "_" + assurName + "_GroupsEtGuarantees.xlsx");
                              
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
                    GroupGarantySante.DeleteGroupsWithSpecificAssureurName(assurName);
                else
                    throw new Exception("Please select the name of the 'Assureur' for which you want to delete groups and guarantees!");

                //refresh the tree
                lbAssur.DataBind();

                if (ItemExists(assurName))
                {
                    SelectItem(assurName);
                    UpdateTreeView(assurName);
                }
                else
                {
                    if (lbAssur.Items.Count > 0)
                    {
                        SelectItem(lbAssur.Items[0].Text);
                        UpdateTreeView(lbAssur.Items[0].Text);
                    }
                    else
                        tvGaranties.Nodes.Clear();
                }
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

        protected void cmdRecreate_Click(object sender, EventArgs e)
        {
            try
            {
                BLGroupsAndGaranties.RecreateGroupsGarantiesSanteFromPresta();                
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
                    UpdateTreeView(lbAssur.Items[0].Text);
                }

                EnableDisableButtons();
            }
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionGaranties::lbAssur_DataBound"); }
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
            catch (Exception ex) { UICommon.HandlePageError(ex, this.Page, "GestionGaranties::EnableDisableButtons");  }
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
            catch (Exception ex)
            {
                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                Page.Validators.Add(myCustomValidator);
            }
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
            catch (Exception ex)
            {
                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                Page.Validators.Add(myCustomValidator);

                return false;
            }
        }

        #endregion


        #region OLD METHODS
        
        //protected void cmdImport_Click(object sender, EventArgs e)
        //{
        //}

        //private void OLD_UpdateTreeView(int id)
        //{
        //    try
        //    {
        //        tvGaranties.Nodes.Clear();

        //        //List<GroupSante> groupSante = GroupSante.GetGroupsAndGarantiesForCompany(id);
        //        //List<GroupPrev> groupPrev = GroupPrev.GetGroupsAndGarantiesForCompany(id);
        //        List<GroupSante> groupSante = GroupSante.GetGroupsAndGarantiesForAssureur(id);
        //        //List<GroupPrev> groupPrev = GroupPrev.GetGroupsAndGarantiesForAssureur(id);

        //        // add all notes for Sante
        //        TreeNode nodeSante = new TreeNode("Santé", "");
        //        nodeSante.SelectAction = TreeNodeSelectAction.Expand;
        //        tvGaranties.Nodes.Add(nodeSante);

        //        foreach (GroupSante gr in groupSante)
        //        {
        //            TreeNode groupNode = new TreeNode(gr.Name, "");

        //            //we will have several duplicates => get only the first garanty and don't display other garanties with the same name
        //            foreach (GarantySante gar in gr.GarantySantes.GroupBy(g => g.Name).Select(group => group.First()))
        //            {
        //                TreeNode garantyNode = new TreeNode(gar.Name, "");
        //                groupNode.ChildNodes.Add(garantyNode);
        //            }

        //            groupNode.Expand();
        //            nodeSante.ChildNodes.Add(groupNode);
        //        }

        //        nodeSante.Expand();

        //        // add all notes for Prev
        //        TreeNode nodePrev = new TreeNode("Prévoyance", "");
        //        nodePrev.SelectAction = TreeNodeSelectAction.Expand;
        //        tvGaranties.Nodes.Add(nodePrev);

        //        //foreach (GroupPrev gr in groupPrev)
        //        //{
        //        //    TreeNode groupNode = new TreeNode(gr.Name, "");

        //        //    foreach (GarantyPrev gar in gr.GarantyPrevs)
        //        //    {
        //        //        TreeNode garantyNode = new TreeNode(gar.Name, "");
        //        //        groupNode.ChildNodes.Add(garantyNode);
        //        //    }

        //        //    groupNode.Expand();
        //        //    nodePrev.ChildNodes.Add(groupNode);
        //        //}

        //        nodePrev.Expand();

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);

        //        var myCustomValidator = new CustomValidator();
        //        myCustomValidator.IsValid = false;
        //        myCustomValidator.ErrorMessage = ex.Message;
        //        Page.Validators.Add(myCustomValidator);
        //    }

        //}
        
        //public List<Company> GetParentCompanies()
        //{
        //    try
        //    {
        //        List<Company> parentCompanies = Company.GetParentCompanies();
        //        parentCompanies.Insert(0, new Company { Id = -1, Name = "Paramètres par défaut" });

        //        return parentCompanies;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message);

        //        var myCustomValidator = new CustomValidator();
        //        myCustomValidator.IsValid = false;
        //        myCustomValidator.ErrorMessage = ex.Message;
        //        Page.Validators.Add(myCustomValidator);

        //        return null;
        //    }
        //}
        

        #endregion

       
    }
}