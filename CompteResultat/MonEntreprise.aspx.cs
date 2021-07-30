using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;
using System.Web.Configuration;

using CompteResultat.BL;
using CompteResultat.DAL;
using CompteResultat.Common;

namespace CompteResultat
{
    public partial class MonEntreprise : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        

        protected void Page_Load(object sender, EventArgs e)
        {
            try
             {               
                cmdSelectFile.Attributes.Add("onclick", "jQuery('#" + LogoUploader.ClientID + "').click();return false;");                
                cmdDeleteLogo.Visible = false;
                imgLogo.Visible = false;

                //first access to the page - fill all textboxes and display the logo
                if (!Page.IsPostBack)
                {
                    txtPassSMTP.Attributes["type"] = "password";

                    MyCompany myComp = BLMyCompany.GetMyCompanyInfo();

                    txtCompName.Text = myComp.Name;
                    txtAddresse.Text = myComp.Address;
                    txtPhone.Text = myComp.Phone;
                    txtEmail.Text = myComp.Email;
                    txtServerSMTP.Text = myComp.ServerSMTP;
                    txtPassSMTP.Text = myComp.PassSMTP;

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

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try
            {            
                //throw new Exception("error...");

                //save the logo file to the Images folder
                if (txtLogoPath.Text != "") 
                {
                    cmdDeleteLogo.Visible = true;
                    imgLogo.Visible = true;

                    //string logoDirectory = Path.Combine(Request.PhysicalApplicationPath, C.logoFolder);
                    //string fullUploadPath = Path.Combine(logoDirectory, txtLogoPath.Text);
                    //LogoUploader.PostedFile.SaveAs(fullUploadPath);

                    //string relativePath = fullUploadPath.Replace(Request.ServerVariables["APPL_PHYSICAL_PATH"], String.Empty);
                    //photoUrl = Page.ResolveUrl(relativePath);
                }                

                MyCompany myComp = new MyCompany
                {
                    Name = txtCompName.Text,
                    Address = txtAddresse.Text,
                    Phone = txtPhone.Text,
                    Logo = txtLogoPath.Text,
                    Email = txtEmail.Text,
                    ServerSMTP = txtServerSMTP.Text,
                    PassSMTP = txtPassSMTP.Text
                };

                BLMyCompany.UpdateMyCompany(myComp);

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

        protected void cmdDeleteLogo_Click(object sender, EventArgs e)
        {
            string logoDirectory = Path.Combine(Request.PhysicalApplicationPath, C.imageFolder);
            string fullUploadPath = Path.Combine(logoDirectory, txtLogoPath.Text);

            imgLogo.Visible = false;
            txtLogoPath.Text = "";
            cmdDeleteLogo.Visible = false;

            if (File.Exists(fullUploadPath))
                File.Delete(fullUploadPath);
        }
    }
}