<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Purge.aspx.cs" Inherits="CompteResultat.Purge" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        $(document).ready(function () {             
            $("#cmdPurge").click(function (evt) {                
                $("#divLoading").css("display", "block");
            });  
        }); 
     
   </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <div style="float: left; width: 800px; margin-top:0px;">
        <h1><asp:Literal  ID="Literal1" runat="server">Purge des données :</asp:Literal> </h1>  
        <table>
            <tr style="height:50px; text-align:left; vertical-align:text-top">
                <%--<td colspan="3"> 
                    <h1><asp:Literal  ID="Literal9" runat="server">Purge des données :</asp:Literal> </h1>                                 
                </td> --%>
                <td>
                    <asp:CheckBox ID="chkPurgeAll" runat="server" Text="&nbsp;Supprimer également les imports archivées "  />
                </td>
                <td>
                    <asp:Button CssClass="ButtonBigBlue ButtonInline" style="vertical-align:middle; display: inline; width: 100px; height: 25px; margin-top:16px; margin-left: 15px; " 
                    OnClientClick="return confirm('Est-ce que vous êtes sur de vouloir supprimer tous les données de la base des données ?');"                         
                    ID="cmdPurge" runat="server" Text="Purge" OnClick="cmdPurge_Click" ClientIDMode="Static" />  
                </td>
            </tr>
            
        </table>

    </div>

    <div runat="server" id="divLoading"  style="display:none" ClientIDMode="Static" >
        <img width="100px" height="100px" style="margin: 70px 50px 10px 50px;" src="Images/ajax-loader.gif" />
    </div>

 </asp:Content>