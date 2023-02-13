<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionImport.aspx.cs" Inherits="CompteResultat.GestionImport" EnableViewState="true" %>

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
            //console.log(strGridViewState)
            var objGVState = JSON.parse(strGVState);
            console.log(objGVState)

            for (var i = 0; i < objGVState.length; i++) {
                var Image = document.getElementById(objGVState[i].imageId)
                var ChkBoxParent = document.getElementById(objGVState[i].chkId)
                var isExpanded = objGVState[i].expanded
                var someSelected = false

                if (Image && isExpanded) {
                    Image.src = collapseImage;
                    Image.title = 'Collapse';
                    $(Image).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(Image).next().html() + "</td></tr>")
                }

                //iterate all rows in child grid and select checkboxes
                var childRows = objGVState[0].fileChkIds
                for (var j = 0; j < childRows.length; j++) {
                    var ChkBoxChild = document.getElementById(childRows[j])
                    if (ChkBoxChild) {
                        someSelected = true
                        ChkBoxChild.checked = true
                    }
                }

                //if at least one child row was selected, select also the checkbox on the parent
                if (ChkBoxParent && someSelected) {
                    ChkBoxParent.checked = true
                }
            }

            //return;
            ////var ids = imageIds.split(',');

            //for (var i = 0; i < ids.length; i++) {
            //    Image = document.getElementById(ids[i]);

            //    if (Image) {
            //        Image.src = collapseImage;
            //        Image.title = 'Collapse';
            //        $(Image).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(Image).next().html() + "</td></tr>")
            //    }
            //} 
        }

        function SelectChildBoxes(chkBox) {
            var chkParent = chkBox.id
            var parentIsChecked = chkBox.checked
            var html = $(chkBox).parent().parent().html()
            var chkBoxes = $(html).find(':checkbox')
            var allCheckBoxes = $(':checkbox')                  
            
            for (var i = 0; i < chkBoxes.length; i++) {
                if (chkBoxes[i].id != chkParent) {
                    var chkId = chkBoxes[i].id

                    //###
                    //chkBoxes[i].checked = parentIsChecked
                    //$(':checkbox#' + chkBoxes[i].id).attr('checked', parentIsChecked);

                    for (var j = 0; j < allCheckBoxes.length; j++) {
                        if (allCheckBoxes[j].id == chkId) {
                            allCheckBoxes[j].checked = parentIsChecked
                            $(':checkbox#' + allCheckBoxes[j].id).attr('checked', parentIsChecked);
                        }
                    } 
                }
            }
        }

        function UnselectBoxes(chkBox) {
            var isChecked = chkBox.checked            
            var allCheckBoxes = $(':checkbox')            

            for (var j = 0; j < allCheckBoxes.length; j++) {
                if (allCheckBoxes[j].id == chkBox.id) {
                    console.log(allCheckBoxes.length, j, chkBox.id )
                    allCheckBoxes[j].checked = isChecked
                    $(':checkbox#' + allCheckBoxes[j].id).attr('checked', isChecked);
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
            if (deleteAllOrDB == "ALL") {
                $('#modalDeleteDescr').text('Est-ce que vous voulez vraiment supprimer toutes les données de la base de données ainsi que tous les fichiers d’import archivés associés à cet import ?')
                $('#confirmDeleteModal').modal('show');
            }
            else {
                $('#modalDeleteDescr').text('Est-ce que vous voulez vraiment supprimer toutes les données de la base de données associés à cet import ?')
                $('#confirmDeleteModal').modal('show');
            }
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


        //not needed
        window.onload = function () {
            //grid = document.getElementById('<%= this.gvImport.ClientID %>');
            //rows = grid.getElementsByTagName('tr');
            //upperBound = parseInt('<%= this.gvImport.Rows.Count %>');
            //rows = grid.getElementsByTagName('tr');
        } 

    </script>
       
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="float:left; margin-top: 10px;">        
        <table border="0">
            
            <tr style="height:50px; text-align:left; vertical-align:text-top">
                <td colspan="2"> 
                    <h1 style="font-weight:600"><asp:Literal  ID="Literal1" runat="server">Gestion des données importées</asp:Literal> </h1>                                 
                </td>            
            </tr>

            <tr style="display:block">
                <td>
                    <table border="0" style="display:block">
                        <tr style="height:40px; text-align:left; vertical-align:middle">
                            <td style="width:190px;">                
                                <label id="lblImportType"  style=" margin-right:5px; font-weight:500; margin-bottom:12px; " runat="server" >Afficher les imports :</label>
                            </td>
                            <td style="" >
                                <asp:RadioButtonList ValidateRequestMode="Disabled" AutoPostBack="true" style="font-weight:400;" RepeatDirection="Horizontal" 
                                    ID="radioReportType" OnSelectedIndexChanged="radioReportType_SelectedIndexChanged" runat="server" >
                                    <asp:ListItem style="margin-right:10px;" Selected><span style="" />&nbsp; Tous les imports</asp:ListItem>
                                    <asp:ListItem style="margin-right:10px">&nbsp; Seulement imports actifs</asp:ListItem>
                                    <asp:ListItem style="margin-right:10px;">&nbsp; Seulement imports inactifs</asp:ListItem>                    
                                </asp:RadioButtonList>                                                
                            </td>
                            <td>
                                <label id="Label1"  style="margin-left:50px; font-weight:500;" runat="server" >Recherche par nom d’import :</label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtImportFilter" CssClass="element" style="width: 180px; margin-left:10px; margin-right:20px; margin-bottom:5px" runat="server" 
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

                    <div id="gvDiv" style="height: 450px; width: 1660px; overflow-y: scroll;overflow-x: hidden; border: 1px solid #A9A9A9; color:#3A4F63; text-decoration:none; "> 
                        
                        <asp:GridView CellPadding="5" ID="gvImport" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" OnSorting="OnSorting"
                        OnRowDataBound="gvImport_RowDataBound" OnRowCommand="gvImport_RowCommand" AllowSorting="true" AllowPaging="false" OnRowCreated="gvImport_RowCreated"   >
                        <%--<AlternatingRowStyle BackColor="White" />--%>

                        <Columns>                             
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdnState" runat="server" Value="collapsed" /> 
                                    <asp:Image ID="imgPlusMinus" onclick="javascript:ToggleImage(this);" runat="server" 
                                            ImageUrl="Images/plus.png" ToolTip="Expand" AutoPostBack="True" />

                                    <asp:Panel ID="pnlOrders" runat="server" Style="display: none">
                                        <asp:GridView ID="gvImpFiles" OnRowCreated="gvImpFiles_RowCreated" OnRowDataBound="gvImpFiles_RowDataBound" CellPadding="5" 
                                            runat="server" AutoGenerateColumns="false" CssClass="ChildGrid" DataKeyNames="Id" OnRowCommand="gvImpFiles_RowCommand" >
                                            <RowStyle BackColor="#D0EFEE" /> 

                                            <Columns>
                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:CheckBox onclick="javascript:UnselectBoxes(this);" ID="chkImport2" runat="server" Checked="false"  />
                                                    </ItemTemplate>
                                                </asp:TemplateField>                                                

                                                <asp:BoundField Visible="false" DataField="Id" HeaderText="Id" SortExpression="Id" />
                                                <asp:BoundField DataField="FileGroup" HeaderText="Group" SortExpression="FileGroup" />  
                                                <asp:BoundField DataField="FileType" HeaderText="Type" SortExpression="FileType" />   
                                                <asp:BoundField DataField="FileName" HeaderText="Nom Fichier" SortExpression="FileName" />  
                                                
                                                <asp:TemplateField HeaderText="Date Prov Ouverture">
                                                    <ItemTemplate>                                                        
                                                        <asp:Label ID="lblProvOuverture" runat="server" Text="-"  />
                                                    </ItemTemplate>
                                                </asp:TemplateField> 
                                                
                                                <asp:BoundField DataField="NbRowsCsv" HeaderText="Nb Lignes Fichier" /> 
                                                <asp:BoundField DataField="AmountCsv" HeaderText="Montants Fichier" />
                                                <asp:BoundField DataField="NbRowsDb" HeaderStyle-BackColor="" ItemStyle-BackColor="#F7A000" HeaderText="Nb Lignes Base" />
                                                <asp:BoundField DataField="AmountDb" ItemStyle-BackColor="#F7A000" HeaderText="Montants Base" />
                                                <asp:BoundField DataField="DifferenceRows" ItemStyle-BackColor="#37BBE8" HeaderText="Ecart Nb Lignes" />
                                                <asp:BoundField DataField="DifferenceAmount" ItemStyle-BackColor="#37BBE8" HeaderText="Ecart Montants" />

                                                <asp:BoundField Visible="false" DataField="IsDifference"  />
                                                
                                                <%--<asp:TemplateField Visible="false" >
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnNameImport" Value="" runat="server"  />
                                                    </ItemTemplate>
                                                </asp:TemplateField> --%>

                                                <asp:TemplateField Visible="true">
                                                    <ItemTemplate>
                                                        <asp:ImageButton Width="24" Height="24" style="margin-right:0px;" ImageUrl="~/Images/analyse-g.png" ID="cmdAnalyseFile" runat="server"  
                                                              CommandName="RedirectFMAnalyse2" CommandArgument='<%# Bind("FileName") %>'   /> 
                                                        
                                                        <%-- <%#Eval("FileName")+","+ Eval("FileGroup")%>     CommandArgument='<%# Bind("FileName") %>'    --%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <%--<asp:TemplateField Visible="true">
                                                    <ItemTemplate>
                                                        <asp:HyperLink Target="_blank" href="Analyse\test.html" runat="server">AAA</asp:HyperLink>    
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>

                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkImport" runat="server" onclick="javascript:SelectChildBoxes(this);" Checked="false" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField Visible="false" DataField="Id"  /> <%--HeaderText="Id" SortExpression="Id"--%>
                            <asp:BoundField DataField="Name" HeaderText="Nom du lot" SortExpression="Name" HeaderStyle-Width="105px" ItemStyle-Wrap="False" />
                            <asp:BoundField DataField="Date" DataFormatString="{0:d}" HeaderText="Date Import" SortExpression="Date" HeaderStyle-Width="105px" ItemStyle-Wrap="False" />
                            <asp:BoundField DataField="UserName" HeaderText="Utilisateur" SortExpression="UserName"  />
                            <asp:BoundField DataField="ImportPath" HeaderText="Dossier Import" SortExpression="ImportPath" ItemStyle-Wrap="True" HeaderStyle-Width="850px" />
                            <asp:BoundField DataField="Archived" HeaderText="Import Actif" SortExpression="Archived"  />
                            <asp:BoundField DataField="ProvOuvertureDate" HeaderText="" ItemStyle-CssClass="Hide" HeaderStyle-CssClass="Hide"  />

                            <asp:TemplateField Visible="true">
                                <ItemTemplate>
                                    <asp:ImageButton Width="24" Height="24" style="margin-right:10px;" ImageUrl="~/Images/folder.png" ID="cmdFileManager" runat="server"  
                                        CommandName="RedirectFMImport" CommandArgument='<%# Bind("ImportPath") %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField Visible="true">
                                <ItemTemplate>
                                    <asp:ImageButton Width="24" Height="24" style="margin-right:10px;" ImageUrl="~/Images/deleteDB.png" ID="cmdDeleteDB" runat="server"  
                                          OnClick="ConfirmDelete" ToolTip="Supprimer toutes les données de la base de données associés à cet import" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField Visible="true">
                                <ItemTemplate>
                                    <asp:ImageButton Width="24" Height="24" style="margin-right:10px;" ImageUrl="~/Images/deleteAll.png" ID="cmdDeleteAll" runat="server"  
                                          OnClick="ConfirmDelete" ToolTip="Supprimer toutes les données de la base de données ainsi que tous les fichiers d’import archivés associés à cet import" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField Visible="true">
                                <ItemTemplate>
                                    <asp:ImageButton Width="24" Height="24" style="margin-right:0px;" ImageUrl="~/Images/analyse-y.png" ID="cmdAnalyseFolder" runat="server"  
                                          CommandName="RedirectFMAnalyse" CommandArgument='<%# Bind("Name") %>'   />                                   
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

                    </ div >

                </td>
            </tr>
            
            <tr style="height:20px;"><td></td></tr>

            <tr>
                <td>
                    <table border="0" style="display:block">
                        <tr style="height:40px; text-align:left; vertical-align:middle">
                            <td style="text-align:left; ">
                                <label id="Label2"  style="font-weight:500; margin-right:15px;" runat="server" >Date Prov Ouverture : </label>
                            </td>
                            <td style="">
                                <asp:TextBox style="margin-bottom:5px;" runat="server" ID="txtProvOuvertureDate" TextMode="Date" Width="200"  />
                            <td>
                            
                            <td style="text-align:left;">
                                <label id="Label3" style="font-weight:500; margin-left:50px; " runat="server" >Nom Import : </label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtNomImport" CssClass="element" style="width:300px; margin-left:10px; margin-right:20px; margin-bottom:5px" runat="server" ClientIDMode="Static"  ></asp:TextBox> 
                            </td>
                               
                            <td>
                                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle;  width: 105px; margin-right:40px;     
                                    padding:0px; margin-bottom: -19px; " id="cmdImport" Text="Importer" runat="server" OnClick="cmdImport_Click" ClientIDMode="Static" /> &nbsp; 
                            </td>
                            <td>
                                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle;  width: 220px; margin-right:20px;      
                                    padding:0px; margin-bottom: -19px; " id="cmdAnalyse" Text="Controle des données importées" runat="server" OnClick="cmdAnalyse_Click" ClientIDMode="Static" /> &nbsp; 
                            </td>
                            <td>
                                <div runat="server" id="divLoading"  style="display:none" ClientIDMode="Static" >
                                    <img width="50px" height="50px" style="margin-left:50px;" src="Images/ajax-loader.gif" />
                                </div>
                            </td>
                        </tr>
                        <tr><td></td><td></td><td></td><td></td><td></td>
                            <td style="">
                                <asp:CheckBox Checked="true" ID="chkAnalyse" runat="server" Text="&nbsp;&nbsp;Avec controle" Font-Size="Small" />              
                            </td>
                            <td style="">
                                <asp:CheckBox Checked="true" ID="chkOnlyNonAnalyzed" runat="server" Text="&nbsp;&nbsp;Uniquement les imports pas encore contrôlées" Font-Size="Small" />              
                            </td>
                        </tr>
                    </table>
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
            <h3 class="modal-title" style="font-size:17px; font-weight:500; color:red;" id="deleteModalTitle">Suppression des données d’import !</h3>
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
