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

        //### not needed
        window.onload = function () {
            //grid = document.getElementById('<%= this.gvImport.ClientID %>');           
            //rows = grid.getElementsByTagName('tr');
            //upperBound = parseInt('<%= this.gvImport.Rows.Count %>');
            //rows = grid.getElementsByTagName('tr');
        }        

        function ExpandImages(imageIds) {
            var ids = imageIds.split(',');

            for (var i = 0; i < ids.length; i++) {
                Image = document.getElementById(ids[i]);

                if (Image) {
                    Image.src = collapseImage;
                    Image.title = 'Collapse';
                    $(Image).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(Image).next().html() + "</td></tr>")
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

    </script>
       
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="float:left; margin-top: 10px;">        
        <table>
            
            <tr style="height:50px; text-align:left; vertical-align:text-top">
                <td colspan="2"> 
                    <h1><asp:Literal  ID="Literal1" runat="server">Gestion des données importées</asp:Literal> </h1>                                 
                </td>            
            </tr>

            <tr>
                <td colspan="2">
                    <asp:ObjectDataSource DataObjectTypeName="CompteResultat.DAL.Import" ID="odsImport" runat="server" TypeName="CompteResultat.DAL.Import" 
                        SelectMethod="GetImports" SortParameterName="sortExpression" ></asp:ObjectDataSource>
                    
                    <%--<asp:HiddenField ID="hdn" runat="server" Value="5" />--%>

                    <asp:GridView CellPadding="5" ID="gvImport" runat="server" DataSourceID="odsImport" DataKeyNames="Id" AutoGenerateColumns="False" 
                        OnRowDataBound="gvImport_RowDataBound" OnRowCommand="gvImport_RowCommand" AllowSorting="true" AllowPaging="true" PageSize="10" >
                        <AlternatingRowStyle BackColor="White" />

                        <Columns>                             
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdnState" runat="server" Value="collapsed" /> 
                                    <asp:Image OnLoad="imgPlusMinus_Load" ID="imgPlusMinus" onclick="javascript:ToggleImage(this);" runat="server" 
                                            ImageUrl="Images/plus.png" ToolTip="Expand" AutoPostBack="True" />

                                    <%--<asp:HiddenField ID="hdnState2" runat="server" Value="collapsed" />--%>      
                                    
                                    <%--<img ID="plusMinus" runat="server" alt="" style="cursor: pointer" src="Images/plus.png" />--%>
                                    <asp:Panel ID="pnlOrders" runat="server" Style="display: none">
                                        <asp:GridView ID="gvImpFiles" CellPadding="5" runat="server" AutoGenerateColumns="false" CssClass="ChildGrid" DataKeyNames="Id" >
                                            <RowStyle BackColor="#D0EFEE" /> 

                                            <Columns>
                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkImport2" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>                                                

                                                <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" />
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
                                    <asp:CheckBox OnCheckedChanged="chkImport_CheckedChanged" ID="chkImport" runat="server" AutoPostBack="True" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" />
                            <asp:BoundField DataField="Name" HeaderText="Nom du lot" SortExpression="Name" />
                            <asp:BoundField DataField="Date" DataFormatString="{0:d}" HeaderText="Date Import" SortExpression="Date" />
                            <asp:BoundField DataField="UserName" HeaderText="Utilisateur" SortExpression="UserName" />
                            <asp:BoundField DataField="ImportPath" HeaderText="Dossier Import" SortExpression="ImportPath" ItemStyle-Wrap="True" HeaderStyle-Width="500px" />


                            <%--<asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:ImageButton style="margin-right:10px;" ImageUrl="~/Images/FM24.jpg" ID="cmdFileManager" runat="server"  
                                        CommandName="RedirectFMImport" CommandArgument='<%# Bind("ImportPath") %>' /> 
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="">
                                <ItemTemplate>
                                    <asp:ImageButton style="margin-right:10px;" ImageUrl="~/Images/deletePath16.jpg" ID="cmdDelete" runat="server"  
                                        CommandName="DeleteImp" CommandArgument='<%# Bind("ID") %>'
                                        OnClientClick="return confirm('Confirmer la suppression du lot ?');"  /> 
                                </ItemTemplate>
                            </asp:TemplateField>--%>

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
                </td>
            </tr>

            <tr style="height:20px;"><td></td></tr>

            <tr style="text-align:center">
                <td style="text-align:left;">
                    <label>Nom Import : </label>                  
                </td>
                <td style="text-align:left;">                
                    <asp:TextBox style="margin-right:49px;" Width="500" ID="txtNomImport" runat="server" ></asp:TextBox> 
                    <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 105px; height: 23px; margin-top:0px; 
                        margin-bottom: 3px;" ID="cmdImport" runat="server" Text="Importer" OnClick="cmdImport_Click" ClientIDMode="Static" />                              
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



     <div runat="server" id="divLoading"  style="display:none" ClientIDMode="Static" >
        <img width="100px" height="100px" style="margin: 70px 50px 10px 50px;" src="Images/ajax-loader.gif" />
    </div>

</asp:Content>
