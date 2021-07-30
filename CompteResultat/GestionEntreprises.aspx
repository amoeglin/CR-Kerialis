<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionEntreprises.aspx.cs" Inherits="CompteResultat.GestionEntreprises" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <div style="float: left; width: 400px;">
        <table>
            <tr style="height:50px; text-align:left; vertical-align:text-top">
                <td colspan="3"> 
                    <h1><asp:Literal  ID="Literal9" runat="server">Liste des entreprises :</asp:Literal> </h1>                                 
                </td>            
            </tr>
        </table>
        
        <asp:TreeView  CssClass="TreeView" Width="350" Height="500"  ID="tvCompany" SkinID="TOC" runat="server" OnTreeNodePopulate="tvCompany_TreeNodePopulate" 
            OnSelectedNodeChanged="tvCompany_SelectedNodeChanged"  >

            <NodeStyle CssClass="NodeStyle" />
            <HoverNodeStyle Font-Italic="true" Font-Bold="true" ForeColor="Red"/>
            <SelectedNodeStyle Font-Bold="true" />
            
        </asp:TreeView>

    </div>
    
    <div style="float: left; margin-left:40px; ">        
        
        <table>
        <tr style="height:50px; text-align:left; vertical-align:text-top">
            <td colspan="3"> 
                <h1><asp:Literal  ID="Literal8" runat="server">Paramètres de l’entreprise sélectionné :</asp:Literal> </h1>                                 
            </td>            
        </tr>

        <tr style="height:40px; text-align:center">
            <td style="text-align:left;">
                <asp:Literal ID="Literal1" runat="server">Raison sociale : </asp:Literal>
                <asp:HiddenField ID="companyId" runat="server" />
            </td>
            <td>
                <asp:TextBox Width="350" ID="txtCompName" runat="server"></asp:TextBox> 
                <asp:RequiredFieldValidator ID="ValReqCompName" runat="server" ControlToValidate="txtCompName" Font-Bold="True" Display="None"
                    ErrorMessage="You need to provide a value for the company name!">*</asp:RequiredFieldValidator>
            </td>
        </tr>

        <tr style="height:40px; text-align:center">
            <td style="text-align:left;">  
                <asp:Literal ID="Literal6" runat="server" Text="Nom contact : "></asp:Literal>                     
            </td>
            <td>  
                <asp:TextBox Width="350" ID="txtContactName" runat="server"></asp:TextBox>                
            </td>
        </tr>

        <tr style="height:40px; text-align:center">
            <td style="text-align:left;">
                <asp:Literal ID="Literal2" runat="server">Adresse : </asp:Literal>
            </td>
            <td>
                <asp:TextBox Width="350" ID="txtAddresse" runat="server"></asp:TextBox> 
            </td>
        </tr>

        <tr style="height:40px; text-align:center">
            <td style="text-align:left;">
                <asp:Literal ID="Literal3" runat="server">Téléphone : </asp:Literal>    
            </td>
            <td>
                <asp:TextBox Width="350" ID="txtPhone" runat="server"></asp:TextBox> 
            </td>
        </tr>
        
         <tr style="height:40px; text-align:center">
            <td style="text-align:left;">
                <asp:Literal ID="Literal5" runat="server">Email : </asp:Literal>                    
            </td>
            <td>  
                <asp:TextBox Width="350" ID="txtEmail" runat="server"></asp:TextBox>                  
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
                <asp:Button CssClass="ButtonBigBlue ButtonInline" style="vertical-align:middle; display: inline; width: 100px; height: 25px; margin-top:16px; margin-left: 15px; " 
                    ID="cmdSelectFile" runat="server" Text="Select" />                  
            </td>
        </tr>

        <tr style="height:40px; text-align:center">
            <td style="text-align:left;"></td>            
            <td >
                <asp:Image style="vertical-align:middle; padding:10px;"  ID="imgLogo" runat="server" Width="80" Height="80" />
                <asp:Button CssClass="ButtonBigBlue ButtonInline" style="vertical-align:middle; display: inline; width: 100px; height: 25px; margin-top:34px; float: right; " 
                    ID="cmdDeleteLogo" runat="server" Text="Supprimer" OnClick="cmdDeleteLogo_Click" />
            </td>
        </tr>

        <tr style="height:60px; text-align:center">
            <td>                   
            </td>
            <td style="text-align:right">
                <asp:Button CssClass="ButtonBigBlue ButtonInline" style="vertical-align:middle; display: inline; width: 100px; height: 25px; margin-top:0px; float: right; " 
                    ID="cmdSave" runat="server" Text="Modifier" OnClick="cmdSave_Click" />                  
            </td>
        </tr>

        <tr style="height:60px; text-align:left">
            <td colspan="3"> 
                <asp:ValidationSummary ForeColor="Red" ID="ValSummary" runat="server" />                                    
            </td>            
        </tr>

    </table>

    </div>
   
    
</asp:Content>
