<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionTypePrev.aspx.cs" Inherits="CompteResultat.GestionTypePrev" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
    <style>
        input[type="submit"]:disabled {background: #dddddd;}

    </style>
    <script type="text/javascript">

        $(document).ready(function () { 
            $("#cmdImport").click(function (evt) {
                $("#divLoading").css("display", "block");
            });  

            $("#cmdRecreate").click(function (evt) {
                $("#divLoading").css("display", "block");
            });
        }); 
     
   </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div style="float: left; width: 500px;">
        <h1><asp:Literal  ID="Literal9" runat="server">Type Prevoyance :</asp:Literal> </h1>
        
        <div class="RepeaterTP"> 

            <asp:PlaceHolder ID="phHeader" Visible='false' runat="server">
                <asp:Label ID="lblEmpty" runat="server" Text="Il n'y a pas des données disponibles !"> </asp:Label>   
            </asp:PlaceHolder>

            <asp:Repeater ItemType="CompteResultat.DAL.TypePrevoyance" SelectMethod="GetTP" ID="rptTP" runat="server" OnItemDataBound="rptTP_ItemDataBound">
                <HeaderTemplate>      
                      <table>
                          <tr><th>Code Sinistre</th><th>Label Sinistre</th></tr>                    
                </HeaderTemplate>
                <FooterTemplate>
                    </table>                
                </FooterTemplate>

                <ItemTemplate>
                    <tr>
                        <td ><%#: Item.CodeSinistre %></td>
                        <td style="width: 500px"><%#: Item.LabelSinistre %></td>                    
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="alternate">
                        <td><%#: Item.CodeSinistre %></td>
                        <td><%#: Item.LabelSinistre %></td> 
                    </tr>
                </AlternatingItemTemplate>            
            </asp:Repeater>
            
        </div> 


        <div style="display:none;"> 
            <asp:FileUpload Width="150" ID="uploadExcel" runat="server" onchange="this.form.submit()" /> 
        </div>
        <div style="margin-top:15px; float:left;">
            <asp:Button CssClass="ButtonBigBlue ButtonInline" style="margin-right:20px; width:90px;" ID="cmdImport" runat="server" Text="Import" ClientIDMode="Static" />  
            <asp:Button CssClass="ButtonBigBlue ButtonInline" style="margin-right:20px; width:90px;" ID="cmdExport" runat="server" Text="Export" OnClick="cmdExport_Click"  /> 
            <asp:Button CssClass="ButtonBigRed ButtonInline" style="width:90px;" ID="cmdDelete" runat="server" Text="Supprimer" OnClick="cmdDelete_Click" ClientIDMode="Static"  /> 
          
        </div>

        <div style="margin-top:5px; float:left;">
            <asp:Button CssClass="ButtonBigBlue ButtonInline" style="margin-right:0px; width:320px;" ID="cmdRecreate" runat="server" Text="Mettre à jour à partir des SINISTRES" ClientIDMode="Static" OnClick="cmdRecreate_Click" />  
        </div>

        <div runat="server" id="divLoading"  style="display:none" ClientIDMode="Static" >
            <img width="100px" height="100px" style="margin: 20px 5px 10px 140px;" src="Images/ajax-loader.gif" />
        </div>

        <asp:ValidationSummary style="margin-top:15px; float:left;" ForeColor="Red" ID="ValSummary" runat="server" />  

    </div>

</asp:Content>
