<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionCRListes.aspx.cs" Inherits="CompteResultat.GestionCRListes" 
    EnableEventValidation = "false" EnableViewState="true"%>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
    <style>
        input[type="submit"]:disabled {background: #dddddd;}

    </style>
    <script type="text/javascript">  

        $(document).ready(function () { 
            $("#cmdImport").click(function (evt) {
                $("#divLoading").css("display", "block");
            });  
        });

        function OpenConfirmdeleteModal(deleteAllOrDB) {           
            $('#modalDeleteDescr').text('Est-ce que vous voulez vraiment supprimer toutes les données de la liste sélectionnée ?')
            $('#confirmDeleteModal').modal('show');
        }
     
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server" style="">
    <div style="margin-top:30px;"></div>

    <div style="float: left; width: 700px; margin-left: 20px;">
        <h1><asp:Literal  ID="Literal9" runat="server">Listes des Groupes et entreprises :</asp:Literal> </h1> 
        
        <asp:CheckBox ID="chkMyLists" runat="server" Checked="false" AutoPostBack="true"
              style="display:inline-block;margin-bottom:15px; margin-top:15px;" Text="&nbsp;&nbsp;Afficher uniquement mes propres listes" OnCheckedChanged="chkMyLists_CheckedChanged" />
        <br />  


        <%--GridView--%>
        <div id="gvDiv" style="margin-bottom:15px; height: 400px; width: 100%; overflow-y: scroll;overflow-x: hidden; border: 1px solid #A9A9A9; color:#3A4F63; text-decoration:none; "> 
                        
            <asp:GridView Width="100%" CellPadding="5" ID="gvCRListes" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" OnSorting="OnSorting"
            OnRowDataBound="gvCRListes_RowDataBound" OnRowCommand="gvCRListes_RowCommand" AllowSorting="true" AllowPaging="false" 
            OnSelectedIndexChanged = "gvCRListes_SelectedIndexChanged" OnRowCreated="gvCRListes_RowCreated"   >
                        
                <Columns>                            
                    <asp:BoundField Visible="false" DataField="Id"  /> 
                    <asp:BoundField DataField="Name" HeaderText="Liste" SortExpression="Name" HeaderStyle-Width="250px" ItemStyle-Wrap="False" HeaderStyle-HorizontalAlign="Center"  />
                    <asp:BoundField DataField="UserName" HeaderText="Utilisateur" HeaderStyle-Width="100px" SortExpression="UserName" HeaderStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type"  HeaderStyle-Width="70px" 
                        ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="AssurType" HeaderText="Type Assureur" SortExpression="AssurType"  HeaderStyle-Width="70px" 
                        ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                            
                    <asp:TemplateField Visible="true"  ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="50px" >
                        <HeaderTemplate>
                            <asp:Panel style="margin-left: auto; margin-right: auto; text-align: center;">
                                Exporter
                            <asp:Panel>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:ImageButton Width="24" Height="24" style="" ImageUrl="~/Images/export.png" ID="cmdExport" runat="server"  
                                CommandName="Export" CommandArgument='<%# Bind("Id") %>' /> 
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField Visible="true" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="50px">
                        <HeaderTemplate>
                            <asp:Panel style="margin-left: auto; margin-right: auto; text-align: center;">
                                Supprimer
                            <asp:Panel>
                        </HeaderTemplate>
                        <ItemTemplate >
                            <asp:ImageButton Width="24" Height="24" style="margin-right:10px;" ImageUrl="~/Images/deleteAll.png" ID="cmdDeleteDB" runat="server"  
                                    OnClick="ConfirmDelete" ToolTip="Supprimer toutes les données de la base de données associés à cet import" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>

                        <EditRowStyle BackColor="#00A8BC" />
                        <FooterStyle BackColor="#00A8BC" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#00A8BC" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#00A8BC" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#EFF3FB" />
                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#00A8BC" />
                        <SortedAscendingHeaderStyle BackColor="#00A8BC" />
                        <SortedDescendingCellStyle BackColor="#E9EBEF" />
                        <SortedDescendingHeaderStyle BackColor="#00A8BC" />
            </asp:GridView>
         </div>        

        <div style="display:none;"> 
            <asp:FileUpload Width="150" ID="uploadExcel" runat="server" onchange="this.form.submit()" /> 
        </div>
        <div style="margin-top:15px; float:right;">
            <asp:Button CssClass="ButtonBigBlue ButtonInline" style="margin-right:0px; width:300px;" ID="cmdImport" runat="server" 
                Text="Importer une nouvelle liste d'entreprises" ClientIDMode="Static" />  
                   
            <%-- <input type="submit" ID="cmdDelete" name="cmdDelete" class="ButtonBigRed ButtonInline" style="width:90px;" value="Supprimer" runat="server" />  --%>
        </div>

        <div runat="server" id="divLoading"  style="display:none" ClientIDMode="Static" >
            <img width="100px" height="100px" style="margin: 20px 5px 10px 140px;" src="Images/ajax-loader.gif" />
        </div>

        <asp:ValidationSummary style="margin-top:15px; float:left;" ForeColor="Red" ID="ValSummary" runat="server" />  

    </div>


    <%-- Repeater --%>

    <div style="float: left; margin-left:120px; "> 

        <h1><asp:Literal  ID="Literal1" runat="server">Contenu de la liste sélectionnée :</asp:Literal> </h1> 
        
        <div class="RepeaterCad"> 

        <asp:PlaceHolder ID="phHeader" Visible='false' runat="server">
            <asp:Label ID="lblEmpty" runat="server" Text="Il n'y a pas des données disponibles !"> </asp:Label>   
        </asp:PlaceHolder>

        <asp:Repeater ItemType="CompteResultat.DAL.CRGenListComp" SelectMethod="GetGroupEntreprise" ID="rptCad" runat="server" OnItemDataBound="rptCad_ItemDataBound">
            <HeaderTemplate>      
                  <table>
                      <tr><th>Groupe</th><th>Entreprise</th></tr>                    
            </HeaderTemplate>
            <FooterTemplate>
                </table>                
            </FooterTemplate>

            <ItemTemplate>
                <tr>
                    <td><%#: Item.GroupName %></td>
                    <td><%#: Item.Enterprise %></td>                    
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="alternate">
                    <td><%#: Item.GroupName %></td>
                    <td><%#: Item.Enterprise %></td>  
                </tr>
            </AlternatingItemTemplate>            
        </asp:Repeater>
            
        </div>     

    </div>

    <div  class="modal fade" id="confirmDeleteModal" tabindex="-1" role="dialog" aria-hidden="true">
      <div class="modal-dialog"  role="document">
        <div class="modal-content " style="background-color: #D0EFEE; color:#00606B; font-weight:500">
          <div class="modal-header">
            <h3 class="modal-title" style="font-size:17px; font-weight:500; color:red;" id="deleteModalTitle">Suppression de la liste sélectionnée !</h3>
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

