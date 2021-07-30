<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MonEntreprise.aspx.cs" Inherits="CompteResultat.MonEntreprise" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <table>
        <tr style="height:50px; text-align:left; vertical-align:text-top">
            <td colspan="3"> 
                <h1><asp:Literal  ID="Literal8" runat="server">Paramètres de votre société :</asp:Literal> </h1>                                 
            </td>            
        </tr>

        <tr style="height:40px; text-align:center">
            <td style="text-align:left;">
                <asp:Literal ID="Literal1" runat="server">Raison sociale : </asp:Literal>
            </td>
            <td>
                <asp:TextBox Width="350" ID="txtCompName" runat="server"></asp:TextBox> 
                <asp:RequiredFieldValidator ID="ValReqCompName" runat="server" ControlToValidate="txtCompName" Font-Bold="True" Display="None"
                    ErrorMessage="You need to provide a value for the company name!">*</asp:RequiredFieldValidator>
            </td>
            <td>
            </td>
        </tr>

        <tr style="height:40px; text-align:center">
            <td style="text-align:left;">
                <asp:Literal ID="Literal2" runat="server">Adresse : </asp:Literal>
            </td>
            <td>
                <asp:TextBox Width="350" ID="txtAddresse" runat="server"></asp:TextBox> 
            </td>
            <td>
            </td>
        </tr>

        <tr style="height:40px; text-align:center">
            <td style="text-align:left;">
                <asp:Literal ID="Literal3" runat="server">Téléphone : </asp:Literal>    
            </td>
            <td>
                <asp:TextBox Width="350" ID="txtPhone" runat="server"></asp:TextBox> 
            </td>
            <td>
            </td>
        </tr>
        
         <tr style="height:40px; text-align:center">
            <td style="text-align:left;">
                <asp:Literal ID="Literal5" runat="server">Email : </asp:Literal>                    
            </td>
            <td>  
                <asp:TextBox Width="350" ID="txtEmail" runat="server"></asp:TextBox>                  
            </td>
            <td>
            </td>
        </tr>

         <tr style="height:40px; text-align:center">
            <td style="text-align:left;">  
                <asp:Literal ID="Literal6" runat="server">Serveur SMTP : </asp:Literal>                     
            </td>
            <td>  
                <asp:TextBox Width="350" ID="txtServerSMTP" runat="server"></asp:TextBox>                
            </td>
            <td>
            </td>
        </tr>

         <tr style="height:40px; text-align:center">
            <td style="text-align:left; padding-right:10px;">
                <asp:Literal ID="Literal7" runat="server">Mot de passe SMTP :   </asp:Literal>                     
            </td>
            <td> 
                <asp:TextBox Width="350" ID="txtPassSMTP" runat="server" ></asp:TextBox>                  
            </td>
            <td>
            </td>
        </tr>

        <tr style="height:40px; text-align:center">
            <td style="text-align:left;">
                <asp:Literal ID="Literal4" runat="server">Logo : </asp:Literal>                  
            </td>
            <td style="text-align:left;">
                <div style="display:none;"> 
                <asp:FileUpload Width="150" ID="LogoUploader" runat="server" onchange="this.form.submit()" /> 
                </div>
                <asp:TextBox Width="250" ID="txtLogoPath" runat="server" ></asp:TextBox>   
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 100px; height: 25px; margin-top:14px; "  ID="cmdSelectFile" runat="server" Text="Select" />                  
            </td>
            
        </tr>

        <tr style="height:40px; text-align:center">
            <td style="text-align:right;">                               
            </td>            
            <td >
                <asp:Image style="vertical-align:middle; padding:10px;"  ID="imgLogo" runat="server" Width="80" Height="80" />
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 100px; height: 25px; margin-top:34px; float: right; " ID="cmdDeleteLogo" runat="server" Text="Supprimer" OnClick="cmdDeleteLogo_Click" />
            </td>
        </tr>


        <tr style="height:60px; text-align:center">
            <td>                   
            </td>
            <td style="text-align:right">
                <asp:Button CssClass="ButtonBigBlue" ID="cmdSave" style="vertical-align:middle; display: inline; width: 100px; height: 25px; margin-top:14px; " runat="server" Text="Modifier" OnClick="cmdSave_Click" />                  
            </td>
            <td>
            </td>
        </tr>

        <tr style="height:60px; text-align:left">
            <td colspan="3"> 
                <asp:ValidationSummary ForeColor="Red" ID="ValSummary" runat="server" />                                    
            </td>            
        </tr>

    </table>

    
</asp:Content>
