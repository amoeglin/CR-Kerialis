<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CRHistory.aspx.cs" Inherits="CompteResultat.CRHistory" EnableViewState="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <style type="text/css">
        .Hide
        {
            display: none; 
        }
    </style>

    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/js/bootstrap.min.js" integrity="sha384-JZR6Spejh4U02d8jOt6vLEHfe/JQGiRRSQQxSfFWpi1MquVdAyjUar5+76PVCmYl" crossorigin="anonymous"></script>
    
    <script type="text/javascript">   

        var collapseImage = 'images/minus.png';
        var expandImage = 'images/plus.png';   

        $(document).ready(function () {  
            $("#cmdImport").click(function (evt) {
                $("#divLoading").css("display", "block");
            });            
        });

        function ExpandImages(strGVState) {
            var objGVState = JSON.parse(strGVState);
            console.log(objGVState)

            for (var i = 0; i < objGVState.length; i++) {
                var Image = document.getElementById(objGVState[i].imageId)
                var isExpanded = objGVState[i].expanded
                var someSelected = false

                if (Image && isExpanded) {
                    Image.src = collapseImage;
                    Image.title = 'Collapse';
                    $(Image).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(Image).next().html() + "</td></tr>")
                }
            }
        }

        function ToggleImage(image) {

            //console.log('BEFORE: ', $(image).parent().parent().parent().html())
            //console.log($(image).closest("tr").html())

            if (image.title == 'Collapse') {
                image.src = expandImage;
                image.title = 'Expand';
                $(image).closest("tr").next().remove();

                $(image).prev()[0].value = "collapsed"
            }
            else {
                image.src = collapseImage;
                image.title = 'Collapse';
                $(image).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(image).next().html() + "</td></tr>")

                $(image).prev()[0].value = "expanded"
            }

            //console.log('AFTER: ', $(image).parent().html())
        }

        function OpenConfirmdeleteModal(deleteAllOrDB) { 
            $('#modalDeleteDescr').text('Est-ce que vous voulez vraiment supprimer toutes les comptes de résultats sélectionnés ?')
            $('#confirmDeleteModal').modal('show');
        }

        //handle scroll - save last scroll position
        $(function () {
            //recover the scroll postion
            $("#gvDiv").scrollTop($("#HiddenScrollTop").val());
        })
        $(function () {
            //save the scroll position
            $("#gvDiv").scroll(function () {
                $("#HiddenScrollTop").val($(this).scrollTop());
            });
        })
    </script>
       
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="float:left; margin-top: 10px; width:100%;">        
        <table border="0">
            
            <tr style="height:50px; text-align:left; vertical-align:text-top">
                <td colspan="2"> 
                    <h1 style="font-weight:600"><asp:Literal  ID="Literal1" runat="server">Comptes de Résultats Automatiques</asp:Literal> </h1>                                 
                </td>            
            </tr>

            <tr style="display:block">
                <td>
                    <table border="0" style="display:block">
                        <tr style="height:40px; text-align:left; vertical-align:middle">
                            <td style="width:240px;">                
                                <label id="lblImportType"  style=" margin-right:5px; font-weight:500; margin-bottom:12px; " runat="server" >Afficher les comptes de résultats :</label>
                            </td>
                            <td style="" >
                                <asp:RadioButtonList ValidateRequestMode="Disabled" AutoPostBack="true" style="font-weight:400;" RepeatDirection="Horizontal" 
                                    ID="radioReportType" OnSelectedIndexChanged="radioReportType_SelectedIndexChanged" runat="server" >
                                    <asp:ListItem style="margin-right:10px;" Selected><span style="" />&nbsp; Tous</asp:ListItem>
                                    <asp:ListItem style="margin-right:10px">&nbsp; Seulement Santé</asp:ListItem>
                                    <asp:ListItem style="margin-right:10px;">&nbsp; Seulement Prévoyance</asp:ListItem>                    
                                </asp:RadioButtonList>                                                
                            </td>
                            <td>
                                <label id="Label1"  style="margin-left:50px; font-weight:500;" runat="server" >Recherche par nom :</label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCRName" CssClass="element" style="width: 180px; margin-left:10px; margin-right:20px; margin-bottom:5px" runat="server" 
                                    ClientIDMode="Static" ></asp:TextBox> 
                            <td>
                                <asp:Button CssClass="ButtonBigBlue" style="width:100px; vertical-align:central; padding:0px; margin-bottom: -19px; " id="cmdSearch" Text="Rechercher" runat="server" 
                                    OnClick="cmdSearch_Click" ClientIDMode="Static" AutoPostBack="true"  /> &nbsp; 
                            </td>          
                        </tr>
                    </table>
                </td>                
            </tr>

            <tr>
                <td colspan="2"> 
                    <asp:HiddenField ID = "HiddenScrollTop" runat="server" Value="0" ClientIDMode="Static" />

                    <div id="gvDiv" style="height: 450px; width: 100%; overflow: scroll; border: 1px solid #A9A9A9; color:#3A4F63; text-decoration:none; "> 
                        
                        <asp:GridView CellPadding="5" ID="gvCR" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" OnSorting="OnSorting"
                        OnRowDataBound="gvCR_RowDataBound" OnRowCommand="gvCR_RowCommand" AllowSorting="true" AllowPaging="false" OnRowCreated="gvCR_RowCreated"   >
                        <%--<AlternatingRowStyle BackColor="#dcecf4"   />--%>

                        <Columns>                             
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdnState" runat="server" Value="collapsed" /> 
                                    <asp:Image ID="imgPlusMinus" onclick="javascript:ToggleImage(this);" runat="server" 
                                            ImageUrl="Images/plus.png" ToolTip="Expand" AutoPostBack="True" />

                                    <asp:Panel ID="pnlOrders" runat="server" Style="display: none">
                                        <asp:GridView ID="gvCRFiles" OnRowCreated="gvCRFiles_RowCreated" OnRowDataBound="gvCRFiles_RowDataBound" CellPadding="5" 
                                            runat="server" AutoGenerateColumns="false" CssClass="ChildGrid" DataKeyNames="Id" OnRowCommand="gvCRFiles_RowCommand" >
                                            <RowStyle BackColor="#D0EFEE" /> 

                                            <Columns> 
                                                <asp:BoundField Visible="false" DataField="Id" HeaderText="Id" />
                                                <asp:BoundField Visible="false" DataField="CRAutoId" HeaderText="CRAutoId" />
                                                <asp:BoundField DataField="Name" HeaderText="Nom" SortExpression="Name" /> 
                                                <asp:BoundField DataField="LevelGrEnt" HeaderText="Niveau" SortExpression="LevelGrEnt" />
                                                <asp:BoundField DataField="CompanyIds" HeaderText="Entreprises" HeaderStyle-Width="20px"  />   
                                                <asp:BoundField DataField="SubsidIds" HeaderText="Filiales" HeaderStyle-Width="20px"  />   
                                                <asp:BoundField DataField="ContractIds" HeaderText="Contrats" HeaderStyle-Width="20px"  />
                                                <asp:BoundField Visible="true" DataField="RaisonSociale" HeaderText="Raison Sociale"   />
                                                <asp:BoundField Visible="true" DataField="StructureCotisation" HeaderText="Structure Cotisation"  />
                                                <asp:BoundField Visible="false" DataField="Name"  /> 
                                                <asp:BoundField Visible="false" DataField="Name"  />
                                                <asp:BoundField Visible="false" DataField="Name"  />
                                                
                                                <asp:TemplateField Visible="true">
                                                    <ItemTemplate>
                                                        <asp:ImageButton Width="24" Height="24" style="margin-right:0px;" ImageUrl="~/Images/folder.png" ID="cmdReportFile" runat="server"  
                                                              CommandName="RedirectSingleReport" CommandArgument='<%# Bind("LevelGrEnt") %>'   /> 
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <%--TODO: Add: ListName, Type (Sante,Prev), Folder (test_2023-06-26T22-11-06), Niveau (Group, Entreprise)--%>
                            <asp:BoundField Visible="false" DataField="Id"  /> <%--HeaderText="Id" SortExpression="Id"   HeaderStyle-Width="105px" ItemStyle-Wrap="False"--%>
                            <asp:BoundField DataField="MainFolderName" HeaderStyle-Width="200px" HeaderText="Nom Compte de Résultat" SortExpression="MainFolderName" /> 
                            <asp:BoundField DataField="MainFolderName" HeaderStyle-Width="600px" HeaderText="Chemin Compte de Résultat" SortExpression="MainFolderName" />                            
                            <asp:BoundField DataField="ReporType" HeaderText="Type" SortExpression="ReporType"  />                            
                            <asp:BoundField DataField="UserName" HeaderText="Utilisateur" SortExpression="UserName"  />
                            <asp:BoundField Visible="true" DataField="CreationDateTime" DataFormatString="{0:g}" HeaderText="Date Génération" SortExpression="CreationDateTime" />
                            <asp:BoundField DataField="ListName" HeaderText="Nom de la Liste" SortExpression="ListName"  />                             
                           
                            <asp:TemplateField Visible="true">
                                <ItemTemplate>
                                    <asp:ImageButton Width="24" Height="24" style="margin-right:10px;" ImageUrl="~/Images/folder.png" ID="cmdFileManager" runat="server"  
                                        CommandName="RedirectFMCR" CommandArgument='<%# Bind("MainFolderName") %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField Visible="true">
                                <ItemTemplate>
                                    <asp:ImageButton Width="24" Height="24" style="margin-right:10px;" ImageUrl="~/Images/deleteAll.png" ID="cmdDeleteAll" runat="server"  
                                          OnClick="ConfirmDelete" ToolTip="Supprimer toutes les comptes de résultats sélectionnés" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                        <EditRowStyle BackColor="#00A8BC" />
                        <FooterStyle BackColor="#00A8BC" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#00A8BC" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#00A8BC" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#FFFF74" />
                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#00A8BC" />
                        <SortedAscendingHeaderStyle BackColor="#00A8BC" />
                        <SortedDescendingCellStyle BackColor="#E9EBEF" />
                        <SortedDescendingHeaderStyle BackColor="#00A8BC" />
                    </asp:GridView>

                    </ div >

                </td>
            </tr>
            
            <tr style="height:20px;"><td></td></tr>

            <tr style="height:20px; text-align:left">
                <td colspan="2"> 
                    <asp:ValidationSummary ForeColor="Red" ID="ValSummary" runat="server" />                                    
                </td>  
                
            </tr>

        </table>

    </div>

     <div  class="modal fade" id="confirmDeleteModal" tabindex="-1" role="dialog" aria-hidden="true">
      <div class="modal-dialog"  role="document">
        <div class="modal-content " style="background-color: #D0EFEE; color:#00606B; font-weight:500">
          <div class="modal-header">
            <h3 class="modal-title" style="font-size:17px; font-weight:500; color:red;" id="deleteModalTitle">Suppression des comptes de résultat !</h3>
            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body" style="font-size:15px;">
              <div class="container">  
                <div class="row">
                    <div class="col-md-12 " id="modalDeleteDescr"></div>                    
                </div>                  
              </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-info" data-dismiss="modal">Fermer</button>
            <asp:Button ID="btnConfirmDeleteAll" runat="server" Text="Oui" OnClick="btnDeleteAll_Click" CssClass="btn btn-danger" />            
          </div>
        </div>
      </div>
    </div>
   
</asp:Content>
