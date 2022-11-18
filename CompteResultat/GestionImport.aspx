<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionImport.aspx.cs" Inherits="CompteResultat.GestionImport" EnableViewState="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    
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
                    for (var j = 0; j < allCheckBoxes.length; j++) {
                        if (allCheckBoxes[j].id == chkId) {
                            allCheckBoxes[j].checked = parentIsChecked
                            $(':checkbox#' + allCheckBoxes[j].id).attr('checked', parentIsChecked);
                        }
                    } 
                }
            }
        }

        function ToggleImage(image) { 
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
        }

        //handle treeview scroll - save last scroll position
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

        //### not needed
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
            <tr>
                <td>
                    <table border="0" style="display:block">
                        <tr style="height:40px; text-align:left; vertical-align:middle">
                            <td style="width:130px;">                
                                <label id="lblImportType"  style=" margin-right:5px; font-weight:500; " runat="server" >Display Imports :</label>
                            </td>
                            <td style="" >
                                <asp:RadioButtonList ValidateRequestMode="Disabled" AutoPostBack="true" style="font-weight:400;" RepeatDirection="Horizontal" 
                                    ID="radioReportType" OnSelectedIndexChanged="radioReportType_SelectedIndexChanged" runat="server" >
                                    <asp:ListItem style="margin-right:10px;" Selected><span style="" />&nbsp; Imports</asp:ListItem>
                                    <asp:ListItem style="margin-right:10px">&nbsp; Only Archived</asp:ListItem>
                                    <asp:ListItem style="margin-right:10px;">&nbsp; Only Active Imports</asp:ListItem>                    
                                </asp:RadioButtonList>                                                
                            </td>
                            <td>
                                <label id="Label1"  style="margin-left:50px; font-weight:500;" runat="server" >Search by import name :</label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtImportFilter" CssClass="element" style="width: 180px; margin-left:10px; margin-right:20px; margin-bottom:5px" runat="server" 
                                    ClientIDMode="Static" AutoPostBack="true" ></asp:TextBox> 
                            <td>
                                <asp:Button CssClass="ButtonBigBlue" style="width:100px; vertical-align:central; padding:0px; margin-bottom: -19px; " id="cmdSearch" Text="Rechercher" runat="server" OnClick="cmdSearch_Click" ClientIDMode="Static" /> &nbsp; 
                            </td>          
                        </tr>
                    </table>
                </td>                
            </tr>

            <tr>
                <td colspan="2">                   
                    <%--<asp:HiddenField ID="hdn" runat="server" Value="5" />--%>

                    <asp:HiddenField ID = "HiddenScrollTop" runat="server" Value="0" ClientIDMode="Static" />

                    <%-- wrap the Gridview in a div that defines a scrollbar --%>
                    <div id="gvDiv" style="height: 450px; width: 1580px; overflow-y: scroll;overflow-x: hidden; border: 1px solid #A9A9A9; color:#3A4F63; text-decoration:none; "> 
                        
                        <asp:GridView CellPadding="5" ID="gvImport" runat="server" DataKeyNames="Id" AutoGenerateColumns="False" OnSorting="OnSorting"
                        OnRowDataBound="gvImport_RowDataBound" OnRowCommand="gvImport_RowCommand" AllowSorting="true" AllowPaging="false"   >
                        <%--<AlternatingRowStyle BackColor="White" />--%>

                        <Columns>                             
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdnState" runat="server" Value="collapsed" /> 
                                    <asp:Image ID="imgPlusMinus" onclick="javascript:ToggleImage(this);" runat="server" 
                                            ImageUrl="Images/plus.png" ToolTip="Expand" AutoPostBack="True" />

                                    <asp:Panel ID="pnlOrders" runat="server" Style="display: none">
                                        <asp:GridView ID="gvImpFiles" CellPadding="5" runat="server" AutoGenerateColumns="false" CssClass="ChildGrid" DataKeyNames="Id" >
                                            <RowStyle BackColor="#D0EFEE" /> 

                                            <Columns>
                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkImport2" runat="server" Checked="false"  />
                                                    </ItemTemplate>
                                                </asp:TemplateField>                                                

                                                <asp:BoundField Visible="false" DataField="Id" HeaderText="Id" SortExpression="Id" />
                                                <asp:BoundField DataField="FileGroup" HeaderText="File Group" SortExpression="FileGroup" />  
                                                <asp:BoundField DataField="FileType" HeaderText="File Type" SortExpression="FileType" />   
                                                <asp:BoundField DataField="FileName" HeaderText="File Name" SortExpression="FileName" />  

                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <%--<asp:HiddenField ID="hdnState" runat="server" Value="collapsed" /> --%>
                                    <%--<asp:CheckBox OnCheckedChanged="chkImport_CheckedChanged" ID="CheckBox1" runat="server" AutoPostBack="True" />--%>
                                    <asp:CheckBox ID="chkImport" runat="server" onclick="javascript:SelectChildBoxes(this);" Checked="false" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField Visible="false" DataField="Id"  /> <%--HeaderText="Id" SortExpression="Id"--%>
                            <asp:BoundField DataField="Name" HeaderText="Nom du lot" SortExpression="Name" />
                            <asp:BoundField DataField="Date" DataFormatString="{0:d}" HeaderText="Date Import" SortExpression="Date" />
                            <asp:BoundField DataField="UserName" HeaderText="Utilisateur" SortExpression="UserName"  />
                            <asp:BoundField DataField="ImportPath" HeaderText="Dossier Import" SortExpression="ImportPath" ItemStyle-Wrap="True" HeaderStyle-Width="1000px" />
                            <asp:BoundField DataField="Archived" HeaderText="Archivé" SortExpression="Archived"  />

                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:ImageButton Width="24" Height="24" style="margin-right:10px;" ImageUrl="~/Images/folder.png" ID="cmdFileManager" runat="server"  
                                        CommandName="RedirectFMImport" CommandArgument='<%# Bind("ImportPath") %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:ImageButton Width="24" Height="24" style="margin-right:10px;" ImageUrl="~/Images/deleteDB.png" ID="cmdDelete" runat="server"  
                                        CommandName="DeleteImp" CommandArgument='<%# Bind("ID") %>' ToolTip="Delete only imported data from the database"
                                        OnClientClick="return confirm('Confirmer la suppression du lot ?');"  /> 
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:ImageButton Width="24" Height="24" style="margin-right:10px;" ImageUrl="~/Images/deleteAll.png" ID="cmdDeleteAll" runat="server"  
                                        CommandName="DeleteImpAll" CommandArgument='<%# Bind("ID") %>' ToolTip="Delete all data in the database as well as all import files "
                                        OnClientClick="return confirm('Confirmer la suppression du lot ?');"  /> 
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
                            <td style="text-align:left;">
                                <label id="Label2"  style="font-weight:500; margin-right:15px;" runat="server" >Date Prov Ouverture : </label>
                            </td>
                            <td>
                                <asp:TextBox style="margin-bottom:5px;" runat="server" ID="txtProvOuvertureDate" TextMode="Date" Width="200"  />
                            <td>
                            
                            <td style="text-align:left;">
                                <label id="Label3" style="font-weight:500; margin-left:50px; " runat="server" >Nom Import : </label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtNomImport" CssClass="element" style="width:300px; margin-left:10px; margin-right:20px; margin-bottom:5px" runat="server" ClientIDMode="Static"  ></asp:TextBox> 
                            <td>
                                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle;  width: 105px;     
                                    padding:0px; margin-bottom: -19px; " id="cmdImport" Text="Importer" runat="server" OnClick="cmdImport_Click" ClientIDMode="Static" /> &nbsp; 
                            </td>
                            <td>
                                <div runat="server" id="divLoading"  style="display:none" ClientIDMode="Static" >
                                    <img width="50px" height="50px" style="margin-left:50px;" src="Images/ajax-loader.gif" />
                                </div>
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


</asp:Content>
