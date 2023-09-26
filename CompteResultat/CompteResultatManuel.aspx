<%@ Page  EnableEventValidation="false" Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CompteResultatManuel.aspx.cs" 
     Inherits="CompteResultat.CompteResultatManuel"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>

        .buttonYellow { display: block; margin-bottom: 20px;  width:290px; background-color:yellow;  } 
          
        .lblHeader { margin-bottom: 15px; display: block; } 
        label.element { display: inline-block; margin-bottom: 20px; font-size: 16px; width:185px; } 
        input.element { display: inline-block;   width:250px; } 

        label.element2 { display: inline-block; margin-bottom: 20px; font-size: 16px; width:120px; } 
        input.element2 { display: inline-block;   width:100px; } 

        button.element { display: block; margin-bottom: 20px;  width:290px; background-color:yellow;  } 
        
        #chkComptesConsol {  margin-top:20px; font-size:25px;  } 

        textarea { border: 1px solid #A9A9A9; font-family: "Times New Roman", Times, serif; background-color:#CECECE; }

        div.mainBlock { float:left; padding:10px; margin-right:30px; }
        div.lastBlock { float:left; padding:10px; margin-top:50px;  }           

    </style>
    
    <script type="text/javascript">

        $(document).ready(function () {

            $("#cmdCreateCR.manual").click(function (evt) {
                $("#divLoading").css("display", "block");
            });

            $('.NodeStyle a').unbind('click')                        

            //$("#cmdCreateCR").click(function (evt) {
            //    if (Page_ClientValidate()) { 
            //        $("#divValSummary").css("display", "block");
            //        $("#divLoading").css("display", "block");
            //    }
            //    else
            //    {
            //        $("#divValSummary").css("display", "none");
            //        $("#divLoading").css("display", "none");
            //    }
            //});

            $("#cmdSelectAll").click(function (evt) {
                $("#divLoading").css("display", "block");
            });

            $(".NodeStyle a").click(function (evt) {
                $("#divLoading").css("display", "block");

                //background-color:#CECECE;  tvContracts
                $("#tvContracts").css("background-color", "#EFEFEF");
                //$("body").css("cursor", "progress");
                //$('html,body,a').css('cursor', 'wait');
                $('html,body,a').css('cursor', 'none');

                $('.NodeStyle a').bind('click', function (e) {
                    e.preventDefault();
                })

            });


            //handle the checkbox changed event of the treeview control

            //$('#tvContracts input[type="checkbox"]').change(function () {
            //    //alert("The element with id " + this.id + " changed.");

            //    //event.preventDefault();
            //    //event.stopPropagation();
            //    //return false;
            //});

            //$('#tvContracts input[type="checkbox"]').click(function (event) {

            //    if ($(this).is(':checked')) {
            //        //alert('checked');
            //        //$(this).prop('checked', true);
            //    } else {                    
            //        //$(this).prop('checked', true);
            //    }

            //    //event.preventDefault();
            //    //event.stopPropagation();
            //    //return false;
            //});

           

        }); 

     // do a postback when a checkbox is clicked in the treeview
     function postBackByObject()
     {
         var o = window.event.srcElement;
         if (o.tagName == "INPUT" && o.type == "checkbox")
        {
           __doPostBack("","");
        } 
     }

     //handle treeview scroll - save last scroll position
     $(function () {
         //recover the scroll postion
         $("#tvDiv").scrollTop($("#HiddenScrollTop").val());                  
     })
     $(function () {
         //save the scroll position
         $("#tvDiv").scroll(function () {
             $("#HiddenScrollTop").val($(this).scrollTop());             
         });
     })
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="mainBlock" >
    <asp:ScriptManager ID="ScriptManger1" runat="Server"></asp:ScriptManager>
    <%--<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>--%>
    <asp:Timer runat="server" ID="Timer1" Interval="2000" Enabled="false" ontick="Timer1_Tick" />
        
    <h1><label class="lblHeader" >Liste des entreprises et contrats :</label> </h1>

    <div>
        <asp:RadioButtonList ID="radioMode" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" 
            style="background-color: #fff; margin-bottom: 5px;"   
            onselectedindexchanged="radioMode_SelectedIndexChanged">  
            <asp:ListItem style="margin-right:10px;" Selected>&nbsp;Mode automatique</asp:ListItem>  
            <asp:ListItem>&nbsp;Mode manuel</asp:ListItem> 
        </asp:RadioButtonList>  
    </div>

    <div>
        <asp:Panel ID="panelAuto" runat="server"  Width="350px" style="background-color: #fff; margin-bottom: 20px;">
            <%--<label class="element">Listes :</label>    --%>    
            <asp:DropDownList Visible="false" ID="cmbListes" Width="250" DataTextField="Name"  DataValueField="Id" runat="server" AutoPostBack="True" 
                OnSelectedIndexChanged="cmbDetailReport_SelectedIndexChanged">            
            </asp:DropDownList>

            <div style="margin-top:10px; margin-bottom:10px;">
                Générer des CR pour :&nbsp;
                <asp:CheckBox Checked="true" Visible="true" class="element" ID="chkGroups" runat="server" Text="&nbsp;Groupes" Font-Size="Medium" />&nbsp;&nbsp;
                <asp:CheckBox Checked="true" Visible="true" class="element" ID="chkEnterprises" runat="server" Text="&nbsp;Entreprises" Font-Size="Medium" />   
            </div>
            
            <%--DataTextField="Name" DataValueField="Id" OnSelectedIndexChanged="lbListes_SelectedIndexChanged" 
                OnDataBound="lbListes_DataBound"   SelectMethod="GetLists"   EnableViewState="false"   --%>
            <asp:ListBox BackColor="#D0EFEE" ID="lbListes"  runat="server" DataTextField="Name" DataValueField="Id" Height="463px" Width="100%" 
                AutoPostBack="true" OnSelectedIndexChanged="lbListes_SelectedIndexChanged"  CssClass="groupListbox" >            
            </asp:ListBox>


         </asp:Panel>
    </div>

    <div>
        <asp:Panel ID="panelManuel" runat="server"  Width="350px" style="background-color: #fff;">

        <div>
            <label class="element" style="margin-right:5px; width: 81px;">Entreprise :</label>
            <asp:TextBox ID="txtCompanyFilter" CssClass="element" style="width: 140px; margin-right:10px;" runat="server" ClientIDMode="Static" AutoPostBack="true" OnTextChanged="txtFilterCompany_Change" ></asp:TextBox> 
            
            <asp:Button CssClass="ButtonBigBlue" style="width:100px; display:inline;" id="cmdSearch" Text="Rechercher" runat="server" OnClick="cmdSearch_Click" ClientIDMode="Static" /> &nbsp;                  
        </div>

        <div>
            <%--display:inline;--%>
            <asp:Button CssClass="ButtonBigBlue" style="width:350px; display:inline;" id="cmdSelectAll" Text="Sélectionner tous les nœuds (très lent)" runat="server" OnClick="cmdSelectAll_Click" ClientIDMode="Static" /> 
        </div>           
    
        <%--  <asp:TreeView CssClass="TreeView" ID="tvContracts" Width="350" Height="500" runat="server" ClientIDMode="Static" Enabled="true"
            OnTreeNodeCheckChanged="tvContracts_TreeNodeCheckChanged" OnSelectedNodeChanged="tvContracts_SelectedNodeChanged" OnLoad="tvContracts_Load" > 
            <asp:HiddenField ID="HiddenScrollLeft" runat="server" Value="0" />
              --%>
    
        <asp:HiddenField ID="HiddenScrollTop" runat="server" Value="0" ClientIDMode="Static" />

        <%-- wrap the treeview in a div that defines a scrollbar height:500px --%>
        <div id="tvDiv" style="height: 350px; width: 350px; overflow-y: scroll;overflow-x: hidden; border: 1px solid #A9A9A9; color:#3A4F63; text-decoration:none; ">
    <asp:TreeView NodeStyle-NodeSpacing="2px" NodeStyle-HorizontalPadding="5px" ID="tvContracts"  runat="server" ClientIDMode="Static" Enabled="true"
        OnTreeNodeCheckChanged="tvContracts_TreeNodeCheckChanged" OnSelectedNodeChanged="tvContracts_SelectedNodeChanged" OnLoad="tvContracts_Load" >

        <NodeStyle CssClass="NodeStyle" ></NodeStyle>

        <SelectedNodeStyle Font-Bold="true" ForeColor="BlueViolet" BackColor="GradientActiveCaption" />        
    </asp:TreeView>
    </div>

    </asp:Panel>
    </div>
</div>

<div class="mainBlock" >
    <%-- ToolTip="Les comptes de résultats existants pour les contrats sélectionnées"        Comptes de résultats existants  --%>
    <h1><asp:Label ToolTip="" class="lblHeader" runat="server" >Détails :</asp:Label> </h1>       
    <%-- <asp:ListBox   ID="lbCRs" Width="350" Height="500" runat="server"></asp:ListBox> --%>
    
    <asp:Panel ID="panelAuto2" runat="server"  Width="100%" style="background-color: #fff; margin-bottom: 20px;">
        <asp:Button runat="server" Visible="false"  Text="Test" OnClick="TEST_Click" />
        <div style="float: left; margin-left:0px; "> 
            <h1><asp:Literal  ID="Literal1" runat="server">Contenu de la liste sélectionnée :</asp:Literal> </h1> 
        
            <div class="RepeaterCR1" style="background-color:#D0EFEE;"> 
                <asp:PlaceHolder ID="phHeader" Visible='false' runat="server">
                    <asp:Label ID="lblEmpty" runat="server" Text="Cette liste est vide!"> </asp:Label>   
                </asp:PlaceHolder>

                <asp:Repeater ItemType="CompteResultat.DAL.CRGenListComp" SelectMethod="GetGroupEntreprise" ID="rptListe" runat="server" 
                    OnItemDataBound="rptListe_ItemDataBound"  >
                    <HeaderTemplate>      
                          <table >
                              <tr style="font-size: 12px; height:25px;margin-bottom:45px;padding-bottom:50px;background-color:#FFFF74; border-bottom: 2px solid #00A8BC;">
                                  <th></th><th>Groupe</th><th>Entreprise</th><th>Raison Sociale</th><th>Structure Cot.</th></tr>                    
                    </HeaderTemplate>
                    <FooterTemplate>
                        </table>                
                    </FooterTemplate>

                    <ItemTemplate>
                        <tr class="standardCR" style="height:20px;">                            
                            <td style="width:30px;"><asp:CheckBox Checked="true" runat="server" ID="chk1" ClientIDMode="Static" /></td>
                            <td><asp:Label Width="90px" runat="server" ID="lblGroupe" ClientIDMode="Static" Text="<%#: Item.GroupName %>" /></td>
                            <td><asp:Label Width="90px" runat="server" ID="lblEnterprise" ClientIDMode="Static" Text="<%#: Item.Enterprise %>" /></td>
                            <td><asp:Label Width="90px" runat="server" ID="lblRaison" ClientIDMode="Static" Text="<%#: Item.RaisonSociale %>" /></td>
                            <td><asp:Label Width="100px" runat="server" ID="lblStructure" ClientIDMode="Static" Text="<%#: Item.StructureCotisation %>" /></td>
                            <%--<td><%#: Item.GroupName %></td>
                            <td><%#: Item.Enterprise %></td>  --%>                  
                        </tr>
                    </ItemTemplate>
                    <%--style="background-color:#FFFF74;"--%>
                    <AlternatingItemTemplate>                        
                        <tr class="alternateCR" style="background-color:#0ABFCC;" >
                            <td style="width:30px;"><asp:CheckBox Checked="true" runat="server" ID="chk1" ClientIDMode="Static" /></td>
                            <td><asp:Label runat="server" ID="lblGroupe" ClientIDMode="Static" Text="<%#: Item.GroupName %>" /></td>
                            <td><asp:Label runat="server" ID="lblEnterprise" ClientIDMode="Static" Text="<%#: Item.Enterprise %>" /></td>
                            <td><asp:Label runat="server" ID="lblRaison" ClientIDMode="Static" Text="<%#: Item.RaisonSociale %>" /></td>
                            <td><asp:Label runat="server" ID="lblStructure" ClientIDMode="Static" Text="<%#: Item.StructureCotisation %>" /></td>
                            
                        </tr>
                    </AlternatingItemTemplate>            
        </asp:Repeater>            
            </div>     

        </div>
    </asp:Panel>

    <asp:Panel ID="panelManuel2" runat="server"  Width="100%" style="background-color: #fff; margin-bottom: 20px;">
        <asp:TextBox TextMode="MultiLine" Enabled="false"  ID="txtDetails" Width="350" Height="600" runat="server"></asp:TextBox>
    </asp:Panel>
    
</div>

<div class="lastBlock">
    <div >
        <label runat="server" id="lblDateDebut" class="element">Date survenance début :</label>
        <%--<input id="txtStartPeriode2" type="date" class="element" runat="server" />--%>
        <asp:TextBox runat="server" ID="txtStartPeriode" TextMode="Date" width="250"  />
    </div>
    <div>
        <label runat="server" id="lblDateFin" class="element">Date survenance fin :</label>
        <%--<input id="txtEndPeriode" type="date" class="element" runat="server" />--%>
        <asp:TextBox runat="server" ID="txtEndPeriode" TextMode="Date" width="250"  />
    </div>
    <div runat="server" id="dateArreteCompte">
        <label class="element">Arrêté des comptes :</label>
        <%--<input id="txtArretCompte" type="date" class="element" runat="server" />--%>
        <asp:TextBox runat="server" ID="txtArretCompte" TextMode="Date" width="250"  />
    </div>
    <div style="display:none;">
        <label class="element">Collège :</label>        
        <asp:DropDownList ID="cmbCollege" Width="130" SelectMethod="GetColleges" DataTextField="Name" DataValueField="Id" runat="server"></asp:DropDownList>
    </div>
    <div>
        <label class="element">Détail du rapport :</label>        
        <asp:DropDownList ID="cmbDetailReport" Width="250" SelectMethod="GetReportTemplates" DataTextField="Name"  DataValueField="Id" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbDetailReport_SelectedIndexChanged"></asp:DropDownList>
    </div>
    <div>
        <label class="element">Nom du rapport :</label>
        <input id="txtNameReport" class="element" runat="server" />
        <asp:Label runat="server" id="validateReportName" Visible="false" ForeColor="Red">Le nom du rapport est obligatoire !</asp:Label>
        <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="ReportSubmissionGroup" runat="server" ErrorMessage="" 
            ControlToValidate="txtNameReport" ForeColor="Red"></asp:RequiredFieldValidator> --%>
    </div>
    <div>
         <asp:CheckBox Checked="true" Visible="false" class="element" ID="chkCalcProv" runat="server" Text="&nbsp;Calculer les provisions" Font-Size="Medium" />   
    </div>

    <div>
         <asp:CheckBox Visible="false" class="element" ID="chkComptesConsol" runat="server" Text="Comptes de résultats consolidées" Font-Size="Medium" />   
    </div>    


    <%-- Taxes & Taux --%>
    <div runat="server" id="div1"  style="display:none" ClientIDMode="Static" >
        <asp:PlaceHolder ID="taxControls" runat="server" >
            <div>
                <label id="lblTaxDef" class="element">Taxe par défaut :</label>
                <input id="taxDef" class="element" runat="server" />
            </div>
            <div>
                <label id="lblTaxAct" class="element">Taux actifs :</label>
                <input id="taxAct" class="element" runat="server" />
            </div>
            <div>
                <label id="lblTaxPer" class="element">Taux périphériques :</label>
                <input id="taxPer" class="element" runat="server" />
            </div>        
        </asp:PlaceHolder>
    </div> 

    
    <%-- Report Type --%>
    <asp:PlaceHolder ID="PlaceHolderReportType" runat="server">      
        <div >
            <label id="lblReportType" class="element" style="display:inline; margin-right:5px; vertical-align:top; " runat="server" >Type du rapport : 
                <asp:Image visible="false" style="display:inline; margin-left: 10px" ID="imgReport" runat="server" />
            </label>
            <asp:RadioButtonList ValidateRequestMode="Disabled" AutoPostBack="true" style="display:inline; margin-top:10px" RepeatDirection="Horizontal" ID="radioReportType" runat="server" OnSelectedIndexChanged="radioReportType_SelectedIndexChanged">
                <asp:ListItem style="margin-right:10px" Selected><img style="margin-left:3px" src = "./Images/report1.png" > Standard</asp:ListItem>
                <asp:ListItem style="margin-right:10px"><img  style="margin-left:3px" src = "./Images/report2.png" > Global Société</asp:ListItem>
                <asp:ListItem style="margin-right:10px; display:none;"><img  style="margin-left:3px" src = "./Images/report3.png" > Global Filiale</asp:ListItem>                    
            </asp:RadioButtonList>
        </div>
        <div style="margin-top: 15px;">
            <asp:CheckBox style="margin-right: 20px" Visible="true" class="element" ID="chkSelectAll" runat="server" Text="Sélectionner tous les entreprises et filiales" Font-Size="Medium"   />  
            <asp:CheckBox Enabled="false" Visible="true" class="element" ID="chkPrev" runat="server" Text="Prevoyance" Font-Size="Medium" /> 
        </div>
     </asp:PlaceHolder>    

    <div style="margin-top: 20px; font-weight: 500; font-size: 20px;" >
        <label id="Label1" class="element" style="display:inline; margin-right:5px; vertical-align:top; font-size: 20px;" runat="server" >TYPE DE COMPTES : 
            <%--<asp:Image visible="false" style="display:inline; margin-left: 10px" ID="imgReport" runat="server" />--%>
        </label>
        <asp:RadioButtonList ValidateRequestMode="Disabled" AutoPostBack="true" style="display:inline; margin-top:10px" RepeatDirection="Horizontal" 
            ID="radioTypeComptes" runat="server" OnSelectedIndexChanged="radioTypeComptes_SelectedIndexChanged">
            <asp:ListItem style="margin-right:10px" Selected>&nbsp;Survenance</asp:ListItem>
            <asp:ListItem style="margin-right:10px">&nbsp;Comptable</asp:ListItem>                                 
        </asp:RadioButtonList>
    </div>

    <br />
    
    <asp:Button Visible="false" id="cmdTest" Text="TEST" runat="server" OnClick="cmdTest_Click" ClientIDMode="Static" />
    
    <div>
        <span>
            <asp:Button style="display:inline;" CssClass="ButtonBigBlue inline" id="cmdCreateCR" Text="Créer comptes de résultats" runat="server" OnClick="cmdCreateCR_Click" ClientIDMode="Static" />
            <asp:Label style="display:inline;" CssClass="m-3 p-2 inline border-5" ID="lblCadencierWarning" BackColor="Red" Font-Bold="true" ForeColor="White" Visible="false" runat="server" Text="Label">Attention, le cadencier n’est pas à jour !</asp:Label>
        </span>        
    </div>
    
    <%--
    <asp:Button CssClass="btn-class" id="cmdDisplayReport" Text="Envoyer comptes de résultats" runat="server" OnClick="cmdDisplayReport_Click" />
    --%>

    <asp:Button CssClass="ButtonBigBlue" Visible="false" id="cmdStartExcel" Text="Télécharger le fichier Excel" runat="server" OnClick="cmdStartExcel_Click"  />
    <asp:Button CssClass="ButtonBigBlue" Visible="false" id="cmdStartPPT" Text="Télécharger le fichier Powerpoint" runat="server" OnClick="cmdStartPPT_Click"  />

    <asp:Button CssClass="ButtonBigRed" Visible="false" id="cmdDeleteCR" Text="Supprimer comptes de résultats" runat="server" 
        OnClientClick="return confirm('Est-ce que vous êtes sur de vouloir supprimer ce comptes des résultats ?');" OnClick="cmdDeleteCR_Click" />    
    
    <div runat="server" id="divValSummary"  style="display:block" ClientIDMode="Static" >
        <asp:ValidationSummary Width="300" ForeColor="Red" ID="ValSummary" runat="server"  /> 
    </div>

    <%--TEST--%>
    <asp:UpdatePanel style="margin-top:20px;  " ID="UpdatePanel1" runat="server" UpdateMode="Always">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
        </Triggers>
        <ContentTemplate>
            <%--style="border:2px solid #00A8BC"--%>

            <asp:Label ForeColor="#0075FF" Font-Size="Large" Font-Bold="true" Height="50px" Width="500px" runat="server" ID="lblProgress" Text=""  />
            <asp:Panel runat="server" ID="spinnerPanel" Visible="false" >                
                <div class='loading'></div>             
            </asp:Panel>

        </ContentTemplate>
    </asp:UpdatePanel>
    
    <div runat="server" id="divLoading" class=""  style="display:none" ClientIDMode="Static" >
        <img width="100px" height="100px" style="margin: 20px 50px 10px 100px;" src="Images/ajax-loader.gif" />
    </div>
    

    <%--
        BackColor="Red" ForeColor="White" 

    <button class="element" id="cmdCreateCR" value="CreateCR" runat="server"  >Créer comptes des résultats</button>
    <button class="element" name="cmdDisplayReport" value="SendCR" runat="server" >Envoyer comptes des résultats</button>
    <button class="element" style="background-color:red; color:white;" id="cmdDeleteCR" value="DeleteCR">Supprimer comptes des résultats</button>
        --%>

</div>

     <!-- Bootstrap Modal Dialog -->    
    <div class="modal fade" id="modalFileOpen" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="modal-content text-red" style="background-color: #00A8BC; font-size:18px; font-weight: bold; color:red;">
                        <div class="modal-header">
                            <h3 class="modal-title text-white" style="font-size:20px; color:white;" id="exampleModalLabel">Fichier ouvert !</h3>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                              <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <asp:Label ID="lblModalBody" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>
