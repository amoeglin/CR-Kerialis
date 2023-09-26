<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ImportData.aspx.cs" Inherits="CompteResultat.ImportData" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
 
    <style>

        label1 
        {
            display: block;width:500px; margin-right:20px;
        }

    </style>

    <script src="scripts/jquery-3.1.1.min.js" type="text/javascript"></script>

    <script type="text/javascript">

        var selectedFiles;
        var uploadFile = "";

        $(document).ready(function () {  
            var box = document.getElementById("dropBox");
            box.addEventListener("dragenter", OnDragEnter, false);
            box.addEventListener("dragover", OnDragOver, false);
            box.addEventListener("dragleave", OnDragLeave, false);
            box.addEventListener("drop", OnDrop, false);

            var outerContainerBox = document.getElementById("outerContainer")
            outerContainerBox.addEventListener("drop", OnDropOB, false);
            outerContainerBox.addEventListener("dragover", OnDragOverOB, false);

            $("#cmdImport").click(function (evt) {
                $("#divLoading").css("display", "block");
            });            

            $("#uploadDropFile").click(function () {
                $("#divLoading").css("display", "block");
                var data = new FormData();
                for (var i = 0; i < selectedFiles.length; i++) {
                    data.append(selectedFiles[i].name, selectedFiles[i]);
                    console.log(selectedFiles[i])
                }
                $.ajax({
                    type: "POST",
                    url: "FileHandler.ashx",
                    contentType: false,
                    processData: false,
                    data: data,

                    success: function (result) {
                        console.log(result)
                        uploadFile = result
                        location.reload();
                    },
                    error: function () {
                        //alert("There was error uploading files!");
                    }
                });
            });
        }); 

        function OnDragOver(e) {
            e.stopPropagation();
            e.preventDefault();

            $("#dropBox").css({ 'background-color': '#FFFF74', 'font-size': '110%' });
            $("#dropIcon").attr("src", "/Images/drop2.png");
            $("#dropIcon2").attr("src", "/Images/drop2.png");
            e.dataTransfer.dropEffect = 'move';
            return false;
        }

        function OnDragLeave(e) {
            $("#dropBox").css({ 'background-color': '#D0EFEE', 'font-size': '100%' });
            $("#dropIcon").attr("src", "/Images/drop1.png");
            $("#dropIcon2").attr("src", "/Images/drop1.png");
        }

        function OnDragEnter(e) {
            e.stopPropagation();
            e.preventDefault();
        }

        function OnDrop(e) {
            e.stopPropagation();
            e.preventDefault();

            $("#dropBox").css({ 'background-color': '#D0EFEE', 'font-size': '100%' });
            $("#dropIcon").attr("src", "/Images/drop2.png");
            $("#dropIcon2").attr("src", "/Images/drop2.png");
            selectedFiles = e.dataTransfer.files;
            //console.log(e.dataTransfer.files[0].name)
            //$("#dropBox").text(selectedFiles[0].name);

            $("#uploadDropFile").trigger("click");
            return false;
        }

        function OnDropOB(e) {
            e.stopPropagation();
            e.preventDefault();
            console.log("drop")
            return false;
        }
        function OnDragOverOB(e) {
            e.stopPropagation();
            e.preventDefault();
            console.log("over")
            return false;
        }
     
    </script>
       
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<div id="outerContainer" style="width:1700px; height:700px;">
    
    <%-- 
    <div id="wrapper">
        <div style="float:left; margin-right:100px;  " >left</div>        
        <div style="" >right</div>

        <div style="margin-top:50px;  ">up</div>
        <div style="margin-top:50px;  ">down</div>
    </div> 
    --%>
    
    <div style="float:left; margin-top: 10px;">

        <table border="0">
        <tr style="height:40px; text-align:left; vertical-align:text-top; margin-bottom: 10px; margin-top:200px; ">
            <td style=" margin-top:200px; " colspan="2"> 
                <h1><asp:Literal  ID="Literal8" runat="server">Import Santé</asp:Literal> </h1>                                 
            </td>            
        </tr>

        <tr style="text-align:center; height:50px;">
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

        <tr style="text-align:center; height:50px;">
            <td style="text-align:left;margin-top:300px;">
                <label>Cotisations Santé : </label>                  
            </td>
            <td style="text-align:left;">
                <div style="display:none;"><asp:FileUpload Width="150" ID="uplCot" runat="server" onchange="this.form.submit()" /></div>
                <asp:TextBox style="margin-right:20px;" Width="500" ID="txtCotPath" runat="server" Enabled="false" ></asp:TextBox> 
                <asp:ImageButton style="margin-right:10px;" ImageUrl="~/Images/deletePath16.jpg" ID="imgSelectCot" runat="server" OnClick="imgSelectCot_Click"  />  
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 105px; height: 23px; margin-top:0px; margin-bottom: 3px;" ID="cmdSelectCot" runat="server" Text="Sélectionner" />                  
            </td>            
        </tr>

        <tr style="text-align:center; height:50px;">
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
            <tr style="height:20px;"><td></td></tr>
        </table>


        <table border="0">
        <tr style="height:50px; text-align:left; vertical-align:text-top; height:50px;">
            <td colspan="3" style="text-align:left; width: 180px;"> 
                <h1><asp:Literal  ID="Literal2" runat="server">Import Prévoyance</asp:Literal> </h1>                                 
            </td>             
        </tr>

         <tr style="text-align:center; height:50px;">
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

        <tr style="text-align:center; height:50px;">
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

        <tr style="text-align:center; height:50px;">
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

        <tr style="text-align:center; height:50px;">
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

        <tr style="text-align:center; height:50px;">
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

        <tr style="text-align:center;">
            <td style="text-align:left;">
                <label>Nom Import : </label>                  
            </td>
            <td style="text-align:left;">                
                <asp:TextBox style="margin-right:49px;" Width="500" ID="txtNomImport" runat="server" ></asp:TextBox> 
                <asp:Button CssClass="ButtonBigBlue" style="vertical-align:middle; display: inline; width: 105px; height: 23px; margin-top:0px; margin-bottom: 3px;" ID="cmdImport" runat="server" Text="Importer" OnClick="cmdImport_Click" ClientIDMode="Static" />                              
            </td>            
        </tr>
        <tr style="text-align:center; height:20px;"></tr>

        <tr style="height:60px; text-align:left; text-wrap:normal;">
            <td colspan="2"> 
                <asp:ValidationSummary ForeColor="Red" ID="ValSummary" runat="server" />                                    
            </td>            
        </tr>

        <tr style="height:50px; text-align:left; vertical-align:text-top; display:none">
            <td colspan="2"> 
                <h1><asp:Literal  ID="Literal1" runat="server">Gestion des données importées</asp:Literal> </h1>                                 
            </td>            
        </tr>

        <tr style="display:none">
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
    
    <div id="divImportSante" class="aaa" style="margin-top: 60px; margin-left: 70px; background-color:#D0EFEE; float:left; width:600px;"     >
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
            <tr >
                <td >
                <td style="">
                    <asp:CheckBox Checked="true" ID="chkAnalyse" runat="server" Text="&nbsp;CONTROLER les données importées dans la base " Font-Size="Small" />              
                </td>                      
            </tr>
        </table>
    </div>    

    <%--TODO 2 * none - block --%>
    <div id="dropBox" style="border: 4px solid #0099B1; text-align: center; border-radius: 16px; margin-top: 20px; margin-left: 70px; 
        background-color:#D0EFEE; float:left; width:600px; height:70px; display:none">
        <img id="dropIcon" src="/Images/drop1.png" style="width:40px; margin-top:10px; margin-right:20px;">
        <span style="display: inline-block; vertical-align: middle; margin-top:10px; font-size: 100%; font-weight:bold; ">Déposer le fichier d'import ici</span>
        <img id="dropIcon2" src="/Images/drop1.png" style="width:40px; margin-top:10px; margin-left:20px;">
        <%--<asp:Image runat="server" ID="Image1" Width="200" Height="200" ImageUrl="~/Images/folder.png" />--%>            
    </div>

    <%--TODO--%>
    <div style="display:none">        
        <input id="uploadDropFile" type="button" value="Importer"  />                              
    </div>

    <asp:Panel ID="pnlAnalyse" Visible="false" runat="server" style="border: 4px solid #0099B1; text-align: left; border-radius: 16px; margin-top: 20px; margin-left: 70px; background-color:#D0EFEE; 
        float:left; width:600px; height:300px;">
        <div id="cont1" runat="server" style="margin-left:20px; margin-top:10px;">
            <asp:Literal ID="litAnalyse" runat="server" Text=""  />            
        </div>        
    </asp:Panel>
    
    <div runat="server" id="divLoading"  style="display:none; position: absolute; left: 1250px; top: 470px;" ClientIDMode="Static" >
        <img width="100px" height="100px" style="margin: 70px 50px 10px 50px;" src="Images/ajax-loader.gif" />
    </div>

</div>

</asp:Content>


