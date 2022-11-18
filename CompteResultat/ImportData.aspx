<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportData.aspx.cs" Inherits="CompteResultat.ImportData" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
 
    <style>

        label1 
        {
            display: block;width:500px; margin-right:20px;
        }

    </style>

    <script type="text/javascript">

        $(document).ready(function () { 
            $("#cmdImport").click(function (evt) {
                $("#divLoading").css("display", "block");
            });  
        }); 
     
   </script>
       
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <%-- 
    <div id="wrapper">
        <div style="float:left; margin-right:100px;  " >left</div>        
        <div style="" >right</div>

        <div style="margin-top:50px;  ">up</div>
        <div style="margin-top:50px;  ">down</div>
    </div> 
    --%>
    
    <div style="float:left; margin-top: 10px;">

        <table>
        <tr style="height:40px; text-align:left; vertical-align:text-top; margin-bottom: 10px; margin-top:200px; ">
            <td style=" margin-top:200px; " colspan="2"> 
                <h1><asp:Literal  ID="Literal8" runat="server">Import Santé</asp:Literal> </h1>                                 
            </td>            
        </tr>

        <tr style="text-align:center">
            <td style="text-align:left; width: 180px;">
                <label>Prestations Santé : </label>                  
            </td>
            <td style="text-align:left;">
                <div style="display:none;"><asp:FileUpload Width="150" ID="uplPrest" runat="server" onchange="this.form.submit()" /></div>
                <asp:TextBox style="margin-right:20px;" Width="500" ID="txtPrestPath" runat="server" Enabled="false" ></asp:TextBox>
                <asp:ImageButton style="margin-right:10px;" ImageUrl="~/Images/deletePath16.jpg" ID="imgDelPrestSante" runat="server" OnClick="imgDelPrestSante_Click" />
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 105px; height: 23px; margin-top:0px; margin-bottom: 3px;" ID="cmdSelectPrest" runat="server" Text="Sélectionner" />                                  
            </td>            
        </tr>

        <tr style="text-align:center">
            <td style="text-align:left;">
                <label>Cotisations Santé : </label>                  
            </td>
            <td style="text-align:left;">
                <div style="display:none;"><asp:FileUpload Width="150" ID="uplCot" runat="server" onchange="this.form.submit()" /></div>
                <asp:TextBox style="margin-right:20px;" Width="500" ID="txtCotPath" runat="server" Enabled="false" ></asp:TextBox> 
                <asp:ImageButton style="margin-right:10px;" ImageUrl="~/Images/deletePath16.jpg" ID="imgSelectCot" runat="server" OnClick="imgSelectCot_Click"  />  
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 105px; height: 23px; margin-top:0px; margin-bottom: 3px;" ID="cmdSelectCot" runat="server" Text="Sélectionner" />                  
            </td>            
        </tr>

        <tr style="text-align:center">
            <td style="text-align:left;">
                <label>Démographie : </label>                  
            </td>
            <td style="text-align:left;">
                <div style="display:none;"><asp:FileUpload Width="150" ID="uplDemo" runat="server" onchange="this.form.submit()" /></div>
                <asp:TextBox style="margin-right:20px;" Width="500" ID="txtDemoPath" runat="server" Enabled="false" ></asp:TextBox>  
                <asp:ImageButton style="margin-right:10px;" ImageUrl="~/Images/deletePath16.jpg" ID="imgSelectDemo" runat="server" OnClick="imgSelectDemo_Click"  /> 
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 105px; height: 23px; margin-top:0px; margin-bottom: 3px;" ID="cmdSelectDemo" runat="server" Text="Sélectionner" />                  
            </td>            
        </tr>
        </table>


        <table>
        <tr style="height:50px; text-align:left; vertical-align:text-top">
            <td colspan="3" style="text-align:left; width: 180px;"> 
                <h1><asp:Literal  ID="Literal2" runat="server">Import Prévoyance</asp:Literal> </h1>                                 
            </td>             
        </tr>

         <tr style="text-align:center">
            <td style="text-align:left; width: 180px;">
                <label>Décomptes Prévoyance : </label>                  
            </td>            
            <td colspan="2" style="text-align:left;">
                <div style="display:none;"><asp:FileUpload Width="150" ID="uplDecompPrev" runat="server" onchange="this.form.submit()" /></div>
                <asp:TextBox style="margin-right:20px;" Width="500" ID="txtDecompPrevPath" runat="server" Enabled="false" ></asp:TextBox>
                <asp:ImageButton style="margin-right:10px;" ImageUrl="~/Images/deletePath16.jpg" ID="imgSelectDecompPrev" runat="server" OnClick="imgSelectDecompPrev_Click" />   
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 105px; height: 23px; margin-top:0px; margin-bottom: 3px;" ID="cmdSelectDecompPrev" runat="server" Text="Sélectionner" />                  
            </td>            
        </tr>

        <tr style="text-align:center">
            <td style="text-align:left;">
                <label>Sinistres Prévoyance : </label>                  
            </td>
            <td colspan="2" style="text-align:left;">
                <div style="display:none;"><asp:FileUpload Width="150" ID="uplSinistrePrev" runat="server" onchange="this.form.submit()" /></div>
                <asp:TextBox style="margin-right:20px;" Width="500" ID="txtSinistrePrevPath" runat="server" Enabled="false" ></asp:TextBox>   
                <asp:ImageButton style="margin-right:10px; height: 16px;" ImageUrl="~/Images/deletePath16.jpg" ID="imgSelectSinistrePrev" runat="server" OnClick="imgSelectSinistrePrev_Click"  />
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 105px; height: 23px; margin-top:0px; margin-bottom: 3px;" ID="cmdSelectSinistrePrev" runat="server" Text="Sélectionner" />                  
            </td>            
        </tr>

        <tr style="text-align:center">
            <td style="text-align:left;">
                <label>Cotisations Prévoyance : </label>                  
            </td>
            <td colspan="2" style="text-align:left;">
                <div style="display:none;"><asp:FileUpload Width="150" ID="uplCotPrev" runat="server" onchange="this.form.submit()" /></div>
                <asp:TextBox style="margin-right:20px;" Width="500" ID="txtCotPrevPath" runat="server" Enabled="false" ></asp:TextBox> 
                <asp:ImageButton style="margin-right:10px;" ImageUrl="~/Images/deletePath16.jpg" ID="imgSelectCotPrev" runat="server" OnClick="imgSelectCotPrev_Click"  />  
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 105px; height: 23px; margin-top:0px; margin-bottom: 3px;" ID="cmdSelectCotPrev" runat="server" Text="Sélectionner" />                  
            </td>            
        </tr>

        <tr style="text-align:center">
            <td style="text-align:left;">
                <label>Provisions à la clôture : </label>                  
            </td>
            <td colspan="2" style="text-align:left;">
                <div style="display:none;"><asp:FileUpload Width="150" ID="uplProv" runat="server" onchange="this.form.submit()" /></div>
                <asp:TextBox style="margin-right:20px;" Width="500" ID="txtProvPath" runat="server" Enabled="false" ></asp:TextBox> 
                <asp:ImageButton style="margin-right:10px;" ImageUrl="~/Images/deletePath16.jpg" ID="ImageButton2" runat="server" OnClick="imgSelectProv_Click"  />  
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 105px; height: 23px; margin-top:0px; margin-bottom: 3px;" ID="cmdSelectProv" runat="server" Text="Sélectionner" />                  
            </td>            
        </tr>

        <tr style="text-align:center">
            <td style="text-align:left;">
                <label>Provisions à l'ouverture : </label>                  
            </td>            
            <td colspan="2" style="text-align:left;">
                <div style="display:none;"><asp:FileUpload Width="150" ID="uplProvOuverture" runat="server" onchange="this.form.submit()" /></div>
                <asp:TextBox runat="server" ID="txtProvOuvertureDate" TextMode="Date" Width="200"   />
                <asp:TextBox style="margin-right:20px;" Width="296" ID="txtProvOuverturePath" runat="server" Enabled="false" ></asp:TextBox> 
                
                <asp:ImageButton style="margin-right:10px;" ImageUrl="~/Images/deletePath16.jpg" ID="ImageButton3" runat="server" OnClick="imgSelectProvOuverture_Click"  />  
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 105px; height: 23px; margin-top:0px; margin-bottom: 3px;" 
                    ID="cmdSelectProvOuverture" runat="server" Text="Sélectionner" />                  
            </td>            
        </tr>

        <%--deactivate display of Experience Import 2/7/21--%>        
        <tr style="display:none; height:50px; text-align:left; vertical-align:text-top">
            <td colspan="2"> 
                <h1><asp:Literal  ID="Literal3" runat="server">Import Expérience</asp:Literal> </h1>                                 
            </td>            
        </tr>

        <tr style="display:none; text-align:center">
            <td style="text-align:left; width: 160px;">
                <label>Données expérience : </label>                  
            </td>
            <td style="text-align:left;">
                <div style="display:none;"><asp:FileUpload Width="150" ID="uplExp" runat="server" onchange="this.form.submit()" /></div>
                <asp:TextBox style="margin-right:20px;" Width="500" ID="txtExpPath" runat="server" Enabled="false" ></asp:TextBox>
                <asp:ImageButton style="margin-right:10px;" ImageUrl="~/Images/deletePath16.jpg" ID="ImageButton1" runat="server" OnClick="imgSelectExp_Click" />   
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 105px; height: 23px; margin-top:0px; margin-bottom: 3px;" ID="cmdSelectExp" runat="server" Text="Sélectionner" />                  
            </td>            
        </tr>
        
        <tr style="height:50px;"><td colspan="2"><hr /></td></tr>

        <tr style="text-align:center">
            <td style="text-align:left;">
                <label>Nom Import : </label>                  
            </td>
            <td style="text-align:left;">                
                <asp:TextBox style="margin-right:49px;" Width="500" ID="txtNomImport" runat="server" ></asp:TextBox> 
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 105px; height: 23px; margin-top:0px; margin-bottom: 3px;" ID="cmdImport" runat="server" Text="Importer" OnClick="cmdImport_Click" ClientIDMode="Static" />                              
            </td>            
        </tr>

        <tr style="height:20px; text-align:left">
            <td colspan="2"> 
                <asp:ValidationSummary ForeColor="Red" ID="ValSummary" runat="server" />                                    
            </td>            
        </tr>

        <tr style="height:50px; text-align:left; vertical-align:text-top">
            <td colspan="2"> 
                <h1><asp:Literal  ID="Literal1" runat="server">Gestion des données importées</asp:Literal> </h1>                                 
            </td>            
        </tr>

        <tr>
            <td colspan="2">
                <asp:ObjectDataSource DataObjectTypeName="CompteResultat.DAL.Import" ID="odsImport" runat="server" TypeName="CompteResultat.DAL.Import" 
                    SelectMethod="GetImportsWithoutArchive" SortParameterName="sortExpression" ></asp:ObjectDataSource>

                <asp:GridView CellPadding="5" ID="gvImport" runat="server" DataSourceID="odsImport" DataKeyNames="Id"  AutoGenerateColumns="False" OnRowDataBound="gvImport_RowDataBound" 
                    OnRowCommand="gvImport_RowCommand" AllowSorting="true" AllowPaging="true" PageSize="10"  >
                    <AlternatingRowStyle BackColor="White" />
                    <Columns>            
                        <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" />
                        <asp:BoundField DataField="Name" HeaderText="Nom du lot" SortExpression="Name" />
                        <asp:BoundField DataField="Date" DataFormatString="{0:d}" HeaderText="Date Import" SortExpression="Date" />
                        <asp:BoundField DataField="UserName" HeaderText="Utilisateur" SortExpression="UserName" />
                        <asp:BoundField DataField="ImportPath" HeaderText="Dossier Import" SortExpression="ImportPath" ItemStyle-Wrap="True" HeaderStyle-Width="500px" />

                        <asp:TemplateField HeaderText="">
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
            </td>
        </tr>

        <tr style="height:20px;"><td></td></tr>

    </table>

    </div>
    
    <div id="divImportSante" style="margin-top: 10px; margin-left: 30px; background-color:#D0EFEE; float:left"     >
        <table  border="0"  >
            <tr >
                <td style="width: 30px;">
                <td style="text-align:left; width: 400px;">
                    <label>OPTIONS DE L'IMPORT </label>                  
                </td>                      
            </tr>
            <tr >
                <td >
                <td style="">
                    <asp:CheckBox Visible="false" Checked="false" ID="chkForceCompSubsid" runat="server" Text="&nbsp;REGROUPER PAR PRODUIT sans entreprise et filliale " Font-Size="Small" />              
                </td>                      
            </tr>
            <tr >
                <td >
                <td style="">
                    <asp:CheckBox Checked="true" ID="chkExp" runat="server" Text="&nbsp;METTRE A JOUR les Prestations EXPERIENCE SANTE " Font-Size="Small" />              
                </td>                      
            </tr>
            <tr >
                <td >
                <td style="">
                    <asp:CheckBox Checked="true" ID="chkGroupes" runat="server" Text="&nbsp;METTRE A JOUR les GROUPES et GARANTIES SANTE " Font-Size="Small" />              
                </td>                      
            </tr>
            <tr >
                <td >
                <td style="">
                    <asp:CheckBox Checked="true" ID="chkCad" runat="server" Text="&nbsp;METTRE A JOUR le Cadencier " Font-Size="Small" />              
                </td>                      
            </tr>
        </table>
    </div>

    <div runat="server" id="divLoading"  style="display:none" ClientIDMode="Static" >
        <img width="100px" height="100px" style="margin: 70px 50px 10px 50px;" src="Images/ajax-loader.gif" />
    </div>


</asp:Content>
