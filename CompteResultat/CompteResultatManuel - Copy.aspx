<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CompteResultatManuel.aspx.cs" 
     Inherits="CompteResultat.CompteResultatManuel" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>

        .buttonYellow { display: block; margin-bottom: 20px;  width:290px; background-color:yellow;  } 

        .lblHeader { margin-bottom: 15px; display: block; } 
        label.element { display: inline-block; margin-bottom: 20px; font-size: 16px; width:155px; } 
        input.element { display: inline-block;   width:230px; } 

        label.element2 { display: inline-block; margin-bottom: 20px; font-size: 16px; width:120px; } 
        input.element2 { display: inline-block;   width:100px; } 

        button.element { display: block; margin-bottom: 20px;  width:290px; background-color:yellow;  } 
        
        #chkComptesConsol {  margin-top:20px; font-size:25px;  } 

        textarea { border: 1px solid #A9A9A9; font-family: "Times New Roman", Times, serif; background-color:#CECECE; }

        div.mainBlock { float:left; padding:10px; margin-right:50px; }
        div.lastBlock { float:left; padding:10px; margin-top:50px;  }

        

    </style>
    
    <script type="text/javascript">

        $(document).ready(function () {

            
            $('.NodeStyle a').unbind('click')


            $("#cmdCreateCR").click(function (evt) {
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
    <h1><label class="lblHeader" >Liste des entreprises et contrats :</label> </h1>
    
    <%--  <asp:TreeView CssClass="TreeView" ID="tvContracts" Width="350" Height="500" runat="server" ClientIDMode="Static" Enabled="true"
        OnTreeNodeCheckChanged="tvContracts_TreeNodeCheckChanged" OnSelectedNodeChanged="tvContracts_SelectedNodeChanged" OnLoad="tvContracts_Load" > 
        <asp:HiddenField ID="HiddenScrollLeft" runat="server" Value="0" />
          --%>

    
    <asp:HiddenField ID="HiddenScrollTop" runat="server" Value="0" ClientIDMode="Static" />

    <%-- wrap the treeview in a div that defines a scrollbar --%>
    <div id="tvDiv" style="height: 500px; width: 350px; overflow-y: scroll;overflow-x: hidden; border: 1px solid #A9A9A9; color:#3A4F63; text-decoration:none; ">
    <asp:TreeView  ID="tvContracts"  runat="server" ClientIDMode="Static" Enabled="true"
        OnTreeNodeCheckChanged="tvContracts_TreeNodeCheckChanged" OnSelectedNodeChanged="tvContracts_SelectedNodeChanged" OnLoad="tvContracts_Load" >

        <NodeStyle CssClass="NodeStyle" ></NodeStyle>

        <SelectedNodeStyle Font-Bold="true" ForeColor="BlueViolet" BackColor="GradientActiveCaption" />        
    </asp:TreeView>
    </div>
</div>

<div class="mainBlock" >
    <%-- ToolTip="Les comptes de résultats existants pour les contrats sélectionnées"        Comptes de résultats existants  --%>
    <h1><asp:Label ToolTip="" class="lblHeader" runat="server" >Détails :</asp:Label> </h1>       
    <%-- <asp:ListBox   ID="lbCRs" Width="350" Height="500" runat="server"></asp:ListBox> --%>
    <asp:TextBox TextMode="MultiLine"  ID="txtDetails" Width="350" Height="498" runat="server"></asp:TextBox>
</div>

<div class="lastBlock">
    <div >
        <label class="element">Période début :</label>
        <input id="txtStartPeriode" type="date" class="element" runat="server" />
    </div>
    <div>
        <label class="element">Période fin :</label>
        <input id="txtEndPeriode" type="date" class="element" runat="server" />
    </div>
    <div>
        <label class="element">Arrêté des comptes :</label>
        <input id="txtArretCompte" type="date" class="element" runat="server" />
    </div>
    <div style="display:none;">
        <label class="element">Collège :</label>        
        <asp:DropDownList ID="cmbCollege" Width="130" SelectMethod="GetColleges" DataTextField="Name" DataValueField="Id" runat="server"></asp:DropDownList>
    </div>
    <div>
        <label class="element">Détail du rapport :</label>        
        <asp:DropDownList ID="cmbDetailReport" Width="230" SelectMethod="GetReportTemplates" DataTextField="Name"  DataValueField="Id" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cmbDetailReport_SelectedIndexChanged"></asp:DropDownList>
    </div>
    <div>
        <label class="element">Nom du rapport :</label>
        <input id="txtNameReport" class="element" runat="server" />
    </div>
    <div>
         <asp:CheckBox Visible="false" class="element" ID="chkCalcProv" runat="server" Text="Calculer les provisions" Font-Size="Medium" />   
    </div>
    <div>
         <asp:CheckBox Visible="false" class="element" ID="chkComptesConsol" runat="server" Text="Comptes de résultats consolidées" Font-Size="Medium" />   
    </div>


    <%-- Taxes & Taux --%>
    <asp:PlaceHolder ID="taxControls" runat="server">
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

    <br />    
    
    <asp:Button CssClass="ButtonBigBlue" id="cmdCreateCR" Text="Créer comptes de résultats" runat="server" OnClick="cmdCreateCR_Click" ClientIDMode="Static" />

    <%--
    <asp:Button CssClass="btn-class" id="cmdDisplayReport" Text="Envoyer comptes de résultats" runat="server" OnClick="cmdDisplayReport_Click" />
    --%>

    <asp:Button CssClass="ButtonBigBlue" Visible="false" id="cmdStartExcel" Text="Télécharger le fichier Excel" runat="server" OnClick="cmdStartExcel_Click"  />
    <asp:Button CssClass="ButtonBigBlue" Visible="false" id="cmdStartPPT" Text="Télécharger le fichier Powerpoint" runat="server" OnClick="cmdStartPPT_Click"  />

    <asp:Button CssClass="ButtonBigRed" Visible="false" id="cmdDeleteCR" Text="Supprimer comptes de résultats" runat="server" 
        OnClientClick="return confirm('Est-ce que vous êtes sur de vouloir supprimer ce comptes des résultats ?');" OnClick="cmdDeleteCR_Click" />
    
    <asp:ValidationSummary Width="300" ForeColor="Red" ID="ValSummary" runat="server" />     
    
    <div runat="server" id="divLoading"  style="display:none" ClientIDMode="Static" >
        <img width="100px" height="100px" style="margin: 20px 50px 10px 100px;" src="Images/ajax-loader.gif" />
    </div>
    

    <%--
        BackColor="Red" ForeColor="White" 

    <button class="element" id="cmdCreateCR" value="CreateCR" runat="server"  >Créer comptes des résultats</button>
    <button class="element" name="cmdDisplayReport" value="SendCR" runat="server" >Envoyer comptes des résultats</button>
    <button class="element" style="background-color:red; color:white;" id="cmdDeleteCR" value="DeleteCR">Supprimer comptes des résultats</button>
        --%>

</div>


</asp:Content>
