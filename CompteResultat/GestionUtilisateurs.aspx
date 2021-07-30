<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionUtilisateurs.aspx.cs" Inherits="CompteResultat.GestionUtilisateurs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div style="float: left; width: 400px;">
        <table>
            <tr style="height:50px; text-align:left; vertical-align:text-top">
                <td colspan="2"> 
                    <h1><asp:Literal  ID="Literal9" runat="server">Liste des utilisateurs :</asp:Literal> </h1>                                 
                </td>            
            </tr>
        
            <tr style="height:50px; text-align:left; vertical-align:text-top">
                <td colspan="2"> 
                    <asp:ListBox ID="lbUsers" runat="server" DataTextField="UserName" DataValueField="UserName" Height="250px" Width="374px" 
                          AutoPostBack="true" OnSelectedIndexChanged="lbUsers_SelectedIndexChanged" >            
                    </asp:ListBox>
                </td>            
            </tr>

            <tr style="height:60px; text-align:left">
                <td colspan="3"> 
                    <asp:ValidationSummary ForeColor="Red" ID="ValSummary" runat="server" />                                    
                </td>            
            </tr>

        </table>        

    </div>


    <div style="float: left; margin-left:50px; "> 
        <table>
            <tr style="height:50px; text-align:left; vertical-align:text-top">
                <td colspan="2"> 
                    <h1><asp:Literal ID="Literal1" runat="server"></asp:Literal> </h1>                                 
                </td>            
            </tr>

            <tr style="height:40px; text-align:center">
                <td style="text-align:left;">
                    <asp:Literal ID="Literal4" runat="server">Nom Utilisateur : </asp:Literal>
                </td>
                <td>
                    <asp:TextBox Width="350" ID="txtUserName" runat="server"></asp:TextBox> 
                </td>                
            </tr>

            <tr style="height:40px; text-align:center">
                <td style="text-align:left;">
                    <asp:Literal ID="Literal5" runat="server">Mot de passe : </asp:Literal>
                </td>
                <td>
                    <asp:TextBox Width="350" ID="txtPwd" runat="server"></asp:TextBox> 
                </td>                
            </tr>

            <tr style="height:40px; text-align:center">
                <td style="text-align:left;">
                    <asp:Literal ID="Literal2" runat="server">Prenom : </asp:Literal>
                </td>
                <td>
                    <asp:TextBox Width="350" ID="txtFirstName" runat="server"></asp:TextBox> 
                </td>                
            </tr>

            <tr style="height:40px; text-align:center">
                <td style="text-align:left;">
                    <asp:Literal ID="Literal3" runat="server">Nom : </asp:Literal>
                </td>
                <td>
                    <asp:TextBox Width="350" ID="txtLastName" runat="server"></asp:TextBox> 
                </td>                
            </tr>

            <tr style="height:40px; text-align:center">
                <td style="text-align:left;">
                    <asp:Literal ID="Literal6" runat="server">Email : </asp:Literal>
                </td>
                <td>
                    <asp:TextBox Width="350" ID="txtEmail" runat="server"></asp:TextBox> 
                </td>                
            </tr>

            <tr style="height:40px; text-align:center">
                <td style="text-align:left;">
                    <asp:Literal ID="Literal7" runat="server">Role utilisateur : </asp:Literal>
                </td>
                <td>
                    <asp:DropDownList ID="cmbRole" runat="server" Width="350">
                    </asp:DropDownList>                    
                </td>                
            </tr>


            <tr style="height:50px; text-align:left; vertical-align:text-top">
                <td colspan="2" style="text-align:right;">                     
                    <asp:Button CssClass="ButtonBigRed ButtonInline" style="width: 115px;" ID="cmdDelete" runat="server" Text="Supprimer" OnClick="cmdDelete_Click"  /> &nbsp;&nbsp;
                    <asp:Button CssClass="ButtonBigBlue ButtonInline" style="width: 115px;" ID="cmdAdd" runat="server" Text="Ajouter" OnClick="cmdAdd_Click"  />   &nbsp;&nbsp;
                    <asp:Button CssClass="ButtonBigBlue ButtonInline" style="width: 115px;" ID="cmdSave" runat="server" Text="Sauvegarder" OnClick="cmdSave_Click"  />                 
                </td>                            
            </tr>

        </table>
        
    </div>

</asp:Content>
