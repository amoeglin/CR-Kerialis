using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Configuration;
using System.Web.Configuration;
using System.Data;
using System.Text.RegularExpressions;

using CompteResultat.BL;
using CompteResultat.DAL;
using CompteResultat.Common;
using System.Web.DynamicData;
using System.Web.ModelBinding;

namespace CompteResultat
{
    public partial class GestionEntreprises : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        protected string selComp = "";

        protected void Page_Load(object sender, EventArgs e)
        {                        
            try
            {                
                cmdSelectFile.Attributes.Add("onclick", "jQuery('#" + LogoUploader.ClientID + "').click();return false;");
                cmdDeleteLogo.Visible = false;
                imgLogo.Visible = false;
                
                //first access to the page - fill the treeview
                if (!Page.IsPostBack)
                {
                    PopulateTreeViewControl();

                    Company myComp = Company.GetCompanyInfo(selComp);

                    txtCompName.Text = myComp.Name;
                    txtAddresse.Text = myComp.Address;
                    txtPhone.Text = myComp.Telephone;
                    txtEmail.Text = myComp.Email;

                    if (myComp.Logo != null)
                    {
                        if (myComp.Logo != "")
                        {
                            txtLogoPath.Text = myComp.Logo;
                            cmdDeleteLogo.Visible = true;
                            imgLogo.Visible = true;
                            imgLogo.ImageUrl = "~/LogoHandler.ashx?filename=" + myComp.Logo;
                        }
                    }
                }

                //Display selected Logo & save the logo file to the image folder
                if (IsPostBack && LogoUploader.PostedFile != null)
                {
                    if (LogoUploader.PostedFile.FileName.Length > 0)
                    {
                        txtLogoPath.Text = LogoUploader.PostedFile.FileName;

                        cmdDeleteLogo.Visible = true;
                        imgLogo.Visible = true;
                        Session["ImageBytes"] = LogoUploader.FileBytes;
                        imgLogo.ImageUrl = "~/ImageHandler.ashx";

                        //save the logo file
                        //string logoDirectory = Path.Combine(Request.PhysicalApplicationPath, C.logoFolder);
                        //string fullUploadPath = Path.Combine(logoDirectory, txtLogoPath.Text);
                        //LogoUploader.PostedFile.SaveAs(fullUploadPath);
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

        private void PopulateTreeViewControl()
        {
            try
            {
                List<Company> parentCompanies = Company.GetParentCompanies();

                if (parentCompanies.Count > 0) selComp = parentCompanies[0].Name;

                foreach (Company comp in parentCompanies)
                {
                    string compValueString = BLCompany.CreateCompanyValueString(comp);
                    TreeNode parentCompNode = new TreeNode(comp.Name, compValueString);
                    parentCompNode.SelectAction = TreeNodeSelectAction.Select;
                    //parentCompNode.Expand();

                    //handle child nodes
                    List<Company> childCompanies = Company.GetChildCompanies(comp.Id);

                    foreach (Company child in childCompanies)
                    {
                        compValueString = BLCompany.CreateCompanyValueString(child);
                        TreeNode childCompNode = new TreeNode(child.Name, compValueString);
                        childCompNode.SelectAction = TreeNodeSelectAction.Select;
                        //childCompNode.ImageUrl = C.imageRelFolder + "comp.png";

                        parentCompNode.ChildNodes.Add(childCompNode);
                    }

                    tvCompany.Nodes.Add(parentCompNode);
                    parentCompNode.Collapse();
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


        protected void tvCompany_SelectedNodeChanged(object sender, EventArgs e)
        {            
            //get the value attribute and fill all textboxes
            string compValue = tvCompany.SelectedNode.Value;
            //selNode = tvCompany.SelectedNode;

            if (compValue != "")
            {
                Dictionary<int, string> companyProps = BLCompany.GetCompanyPropsAsDictionary(compValue);

                companyId.Value = companyProps[(int)C.eCompanyProperties.Id];
                txtCompName.Text = companyProps[(int)C.eCompanyProperties.Name];
                txtAddresse.Text = companyProps[(int)C.eCompanyProperties.Address];
                txtPhone.Text = companyProps[(int)C.eCompanyProperties.Telephone];
                txtEmail.Text = companyProps[(int)C.eCompanyProperties.Email];
                txtContactName.Text = companyProps[(int)C.eCompanyProperties.ContactName];
                txtLogoPath.Text = companyProps[(int)C.eCompanyProperties.Logo];
                
                if (txtLogoPath.Text != "")
                {                    
                    cmdDeleteLogo.Visible = true;
                    imgLogo.Visible = true;

                    imgLogo.ImageUrl = "~/LogoHandler.ashx?filename=" + txtLogoPath.Text;

                    //imgLogo.ImageUrl = Page.ResolveClientUrl(C.logoRelFolder + txtLogoPath.Text);
                }                
            }

            Session["SelectedCompanyNodeValue"] = tvCompany.SelectedNode.Value;
            //tvCompany.SelectedNode.Selected = false;
        }        

        protected void cmdDeleteLogo_Click(object sender, EventArgs e)
        {
            //string logoDirectory = Path.Combine(Request.PhysicalApplicationPath, C.logoFolder);
            //string fullUploadPath = Path.Combine(logoDirectory, txtLogoPath.Text);

            imgLogo.Visible = false;
            txtLogoPath.Text = "";
            cmdDeleteLogo.Visible = false;

            //### before deleting th logo file verify if any other resource still uses this logo file
            //if (File.Exists(fullUploadPath))
            //    File.Delete(fullUploadPath);
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {
                //save the logo file to the Images folder
                if (txtLogoPath.Text != "")
                {
                    cmdDeleteLogo.Visible = true;
                    imgLogo.Visible = true;
                }

                Company comp = new Company
                {
                    Id = int.Parse(companyId.Value),
                    Name = txtCompName.Text,
                    Address = txtAddresse.Text,
                    Telephone = txtPhone.Text,
                    Logo = txtLogoPath.Text,
                    Email = txtEmail.Text,
                    ContactName = txtContactName.Text
                };
                
                //Validate input data
                IValueProvider provider = new FormValueProvider(ModelBindingExecutionContext);
                if (!TryUpdateModel<Company>(comp, provider))
                    return;


                BLCompany.UpdateCompany(comp);

                //modify the value string of the selected node
                string nodeValue = Session["SelectedCompanyNodeValue"].ToString();
                string compValueString = BLCompany.CreateCompanyValueString(comp);

                //iterate all nodes and select the correct one
                //foreach (TreeNode nde in tvCompany.Nodes)
                //{
                //    if (nde.Value == nodeValue)
                //    {
                //        nde.Value = compValueString;
                //        nde.Select();
                //      break;
                //    }
                //}

                tvCompany.Nodes.Cast<TreeNode>().Where(node => node.Value == nodeValue)
                .ToList()
                .ForEach(node => {
                        node.Value = compValueString;
                    node.Select();
                    });
              

            }
            catch (Exception ex)
            {
                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                //myCustomValidator.ValidationGroup = "Group2";

                Page.Validators.Add(myCustomValidator);
            }
        }


        


        //### Old Code

        #region Populate Treenode OnDemand

        protected void tvCompany_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            if (e.Node.ChildNodes.Count == 0)
            {
                switch (e.Node.Depth)
                {
                    case 0:
                        PopulateParentCompanies(e.Node);
                        break;
                    case 1:
                        PopulateChildCompanies(e.Node);
                        break;
                }
            }
        }

        private void PopulateParentCompanies(TreeNode node)
        {
            List<Company> parentCompanies = Company.GetParentCompanies();

            foreach (Company comp in parentCompanies)
            {
                string compValueString = BLCompany.CreateCompanyValueString(comp);
                TreeNode NewNode = new TreeNode(comp.Name, compValueString);

                NewNode.PopulateOnDemand = true;
                NewNode.SelectAction = TreeNodeSelectAction.Expand;
                //NewNode.Expand();
                node.ChildNodes.Add(NewNode);
            }
        }

        private void PopulateChildCompanies(TreeNode node)
        {

            Dictionary<int, string> companyProps = BLCompany.GetCompanyPropsAsDictionary(node.Value);
            int parentId = int.Parse(companyProps[(int)C.eCompanyProperties.Id]);

            List<Company> childCompanies = Company.GetChildCompanies(parentId);

            foreach (Company comp in childCompanies)
            {
                string compValueString = BLCompany.CreateCompanyValueString(comp);
                TreeNode NewNode = new TreeNode(comp.Name, compValueString);

                NewNode.PopulateOnDemand = true;
                NewNode.SelectAction = TreeNodeSelectAction.Expand;
                node.ChildNodes.Add(NewNode);
            }
        }


        #endregion

        
    }
}