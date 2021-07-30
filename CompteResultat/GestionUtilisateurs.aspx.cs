using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web.Profile;

using CompteResultat.BL;
using CompteResultat.DAL;
using CompteResultat.Common;
using System.Web.DynamicData;
using System.Web.ModelBinding;


namespace CompteResultat
{
    public partial class GestionUtilisateurs : System.Web.UI.Page
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Page_Load(object sender, EventArgs e)
        {
            //string key64 = G.GetRandomKey(64);
            //string key32 = G.GetRandomKey(32);
            
            if (!IsPostBack)
            {
                //create user roles                 
                foreach (C.eUserRoles ur in Enum.GetValues(typeof(C.eUserRoles)))
                {
                    if (!Roles.RoleExists(ur.ToString()))
                        Roles.CreateRole(ur.ToString());

                    cmbRole.Items.Add(ur.ToString());
                }
               
                cmbRole.SelectedIndex = 0;

                RefreshUserList("");
            }            
        }

        protected void lbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FillUserData();                
            }
            catch (Exception ex)
            {
                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                Page.Validators.Add(myCustomValidator);
            }

        }

        protected void cmdDelete_Click(object sender, EventArgs e)
        {
            //### ask for confirmation

            if (lbUsers.SelectedItem != null)
            {
                MembershipUser selUser = Membership.GetUser(lbUsers.SelectedItem.Text);

                if (selUser != null)
                {
                    ProfileManager.DeleteProfile(selUser.UserName);
                    Membership.DeleteUser(selUser.UserName);

                    RefreshUserList("");
                }
            }
        }

        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            try
            {
                //### validate provided input parameters

                //make sure the user name does not already exist
                MembershipUser selUser = Membership.GetUser(txtUserName.Text);

                if (selUser != null)
                {
                    //user already exists
                    throw new Exception("Un utilisateur avec le même nom existe déjà !");
                }

                //create the user
                selUser = Membership.CreateUser(txtUserName.Text, txtPwd.Text, txtEmail.Text);
                Roles.AddUserToRole(txtUserName.Text, cmbRole.SelectedItem.Text);
                ProfileBase profile = ProfileBase.Create(selUser.UserName);

                if (profile != null)
                {
                    profile["FirstName"] = txtFirstName.Text;
                    profile["LastName"] = txtLastName.Text;

                    profile.Save();
                }

                RefreshUserList(txtUserName.Text);

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
            try
            {
                //### validate provided input parameters
                //verify if the username already exists - if not, the user wants to change the username
                MembershipUser oldUser = Membership.GetUser(lbUsers.SelectedItem.Text);
                MembershipUser newUser = null;
                ProfileBase profile = null;
                string userName = lbUsers.SelectedItem.Text;

                if (oldUser.UserName != txtUserName.Text)
                {
                    // the username was changed: delete the old user & create a new one
                    Membership.DeleteUser(oldUser.UserName);
                    newUser = Membership.CreateUser(txtUserName.Text, txtPwd.Text, txtEmail.Text);
                    profile = ProfileBase.Create(newUser.UserName);
                    userName = txtUserName.Text;
                    Roles.AddUserToRole(userName, cmbRole.SelectedItem.Text);
                }
                else
                {
                    //update the user
                    string oldPwd = oldUser.GetPassword();
                    oldUser.ChangePassword(oldPwd, txtPwd.Text);
                    oldUser.Email = txtEmail.Text;

                    Membership.UpdateUser(oldUser);

                    if (Roles.IsUserInRole(oldUser.UserName, C.eUserRoles.Administrateur.ToString()))
                        Roles.RemoveUserFromRole(oldUser.UserName, C.eUserRoles.Administrateur.ToString());
                    if (Roles.IsUserInRole(oldUser.UserName, C.eUserRoles.Utilisateur.ToString()))
                        Roles.RemoveUserFromRole(oldUser.UserName, C.eUserRoles.Utilisateur.ToString());

                    if (!Roles.IsUserInRole(oldUser.UserName, cmbRole.SelectedItem.Text))
                        Roles.AddUserToRole(oldUser.UserName, cmbRole.SelectedItem.Text);

                    profile = ProfileBase.Create(oldUser.UserName);
                }               

                if (profile != null)
                {
                    profile["FirstName"] = txtFirstName.Text;
                    profile["LastName"] = txtLastName.Text;                    

                    profile.Save();
                }

                RefreshUserList(userName);

            }
            catch (Exception ex)
            {
                var myCustomValidator = new CustomValidator();
                myCustomValidator.IsValid = false;
                myCustomValidator.ErrorMessage = ex.Message;
                Page.Validators.Add(myCustomValidator);
            }
        }

        private void FillUserData()
        {
            try
            {
                if (lbUsers.SelectedItem != null)
                {
                    MembershipUser selUser = Membership.GetUser(lbUsers.SelectedItem.Text);

                    if (selUser != null)
                    {
                        ProfileBase profile = ProfileBase.Create(selUser.UserName);

                        txtUserName.Text = selUser.UserName;
                        txtPwd.Text = selUser.GetPassword();
                        txtEmail.Text = selUser.Email;

                        if (profile != null)
                        {
                            txtFirstName.Text = profile["FirstName"].ToString();
                            txtLastName.Text = profile["LastName"].ToString();
                        }

                        //fill user role
                        if (Roles.GetRolesForUser(selUser.UserName).Length > 0)
                            cmbRole.SelectedIndex = cmbRole.Items.IndexOf(cmbRole.Items.FindByText(Roles.GetRolesForUser(selUser.UserName)[0]));
                        else
                            cmbRole.SelectedIndex = 1;
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

        private void RefreshUserList(string userName)
        {
            lbUsers.DataSource = Membership.GetAllUsers();
            lbUsers.DataBind();

            if (lbUsers.Items.Count > 0)
            {
                if (userName != "")
                    lbUsers.SelectedIndex = lbUsers.Items.IndexOf(lbUsers.Items.FindByText(userName));               
                else
                    lbUsers.SelectedIndex = 0;

                FillUserData();
            }
        }


    }



    
}