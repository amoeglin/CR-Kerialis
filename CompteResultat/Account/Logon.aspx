<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Logon.aspx.cs" Inherits="CompteResultat.Account.Logon" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Log In
    </h2>
    <p>
        Rentrez votre nom d’utilisateur et votre mot de passe
        <asp:HyperLink ID="RegisterHyperLink" runat="server" EnableViewState="false">Enregistrez-vous</asp:HyperLink> pour créer une nouvelle compte d’utilisateur
    </p>

    <asp:Login DestinationPageUrl="~/SynthesePrev.aspx" ID="LoginUser" UserNameLabelText="Nom d'utilisateur : " PasswordLabelText="Mot de passe : " LabelStyle-CssClass="Label" 
        LoginButtonStyle-CssClass="ButtonBigBlue ButtonLogin" TitleText="" DisplayRememberMe="False" runat="server" TextBoxStyle-CssClass="Textbox" OnLoggedIn="LoginUser_LoggedIn"
         ></asp:Login>

    <p>
        <a href="~/Account/RecoverPassword.aspx" id="changePwdLink" runat="server">Récupérer votre mot de passe</a>
    </p>

</asp:Content>
