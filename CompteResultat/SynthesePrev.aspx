<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SynthesePrev.aspx.cs" Inherits="CompteResultat.SynthesePrev" EnableViewState="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
    <style>
        input[type="submit"]:disabled {background: #dddddd;}

        .lblHeader { margin-bottom: 15px; display: block; } 
        label.element { display: inline-block; margin-bottom: 20px; font-size: 16px; width:185px; } 
        input.element { display: inline-block;   width:250px; } 

        .grid-container { display: grid; grid-template-columns: 600px 800px; 
            padding: 5px; 
            grid-template-rows: auto;
            grid-template-areas: 
                "settings reports"
                "createSynthese createSynthese"
                "synthese synthese";
        }
       
        .itemSettings {grid-area: settings;}
        .itemReports {  grid-area: reports;}
        .itemCreateSynthese {  grid-area: createSynthese; }
        .itemSynthese {  grid-area: synthese; }

    </style>

    <script type="text/javascript">

        $(document).ready(function () { 
            $("#cmdCreate").click(function (evt) {
                //alert("eee")
                $("#divLoading").css("display", "block");
            });  
        }); 
     
   </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%--border-style: solid;--%>
    <div class="grid-container">

    <div class="itemSettings" >
        <h1><asp:Literal  ID="Literal9" runat="server">Synthese Prevoyance :</asp:Literal> </h1> 
        
        <div>
            <label class="element" style="margin-top:15px;">Assureur :</label>
            <asp:DropDownList ID="cmbAssureur" CssClass="element" style="width: 250px; margin-right:10px;" runat="server" ClientIDMode="Static" AutoPostBack="true" 
                SelectMethod="GetAssureurs" DataTextField="Name" DataValueField="Id" OnSelectedIndexChanged="cmbAssureur_SelectedIndexChanged" ></asp:DropDownList>                
        </div>

        <div >
            <label class="element">Date survenance début :</label>
            <%--OnTextChanged="txtStartPeriode_TextChanged" AutoPostBack="true"--%>
            <asp:TextBox runat="server" ID="txtStartPeriode" TextMode="Date" width="250"   />
        </div>
        <div>
            <label class="element">Date survenance fin :</label>
            <asp:TextBox runat="server" ID="txtEndPeriode" TextMode="Date" width="250"  />
        </div>
        <div>
            <label class="element">Arrêté des comptes :</label>
            <asp:TextBox runat="server" ID="txtArretCompte" TextMode="Date" width="250"  />
        </div>

        <div>         
            <asp:CheckBox Visible="false" ID="chkCalcProv" class="element" runat="server" Checked="true" style="display:inline-block;margin-bottom:15px; margin-top:15px;" Text="&nbsp;Calculer les provisions" /><br />
        </div>

    </div>

    <div class="itemReports" >
        <h1><asp:Literal  ID="Literal1" runat="server">Rapports prevoyance à générer :</asp:Literal> </h1> 

        <div>         
            <asp:CheckBox ID="chkSynthese"  runat="server" Checked="true" style="display:inline-block;margin-top:15px; " Text="&nbsp;Synthese&nbsp;- Top des gains/pertes pour les " />
            <asp:TextBox runat="server" AutoPostBack="True" ID="txtNumberEnt"  width="30"  OnTextChanged="txtNumberEnt_TextChanged"  />&nbsp;entreprises les plus significatives
            <br />
            <asp:Label style="display:inline;" CssClass="m-3 p-2 inline border-5" ID="lblNumbEntWarning" BackColor="Red" Font-Bold="true" ForeColor="White" Visible="false" runat="server" Text="Label"></asp:Label>
           
        </div>        
        <div>         
            <asp:CheckBox ID="chkGlobalEnt" class="element" runat="server" Checked="true" style="display:inline-block;margin-top:00px;" Text="&nbsp;Global société par entreprise" /><br />
        </div>      
               
        <div>         
            <asp:CheckBox ID="chk1An" class="element" runat="server" Checked="true" style="display:inline-block;margin-top:00px;" Text="&nbsp;Compte de résultats pour une année de survenance et pour tous les produits" /><br />
        </div>  

        <div style="margin-top: 20px; font-weight: 500; font-size: 20px;" >
            <label id="lblReportType" class="element" style="display:inline; margin-right:5px; vertical-align:top; font-size: 20px;" runat="server" >TYPE DE COMPTES : 
                <%--<asp:Image visible="false" style="display:inline; margin-left: 10px" ID="imgReport" runat="server" />--%>
            </label>
            <asp:RadioButtonList ValidateRequestMode="Disabled" AutoPostBack="false" style="display:inline; margin-top:10px" RepeatDirection="Horizontal" 
                ID="radioTypeComptes" runat="server">
                <asp:ListItem style="margin-right:10px" Selected>&nbsp;Survenance</asp:ListItem>
                <asp:ListItem style="margin-right:10px">&nbsp;Comptable</asp:ListItem>                                 
            </asp:RadioButtonList>
        </div>
        

    </div>
    
        
    <div class="itemCreateSynthese">
        <div style="margin-top:10px; float:left;">
            <span>
                <asp:Button CssClass="ButtonBigBlue ButtonInline" style="margin-right:0px; width:320px;" ID="cmdCreate" runat="server" Text="Créer Synthese" ClientIDMode="Static" OnClick="cmdcreate_Click" />  
                <asp:Label style="display:inline;" CssClass="m-3 p-2 inline border-5" ID="lblCadencierWarning" BackColor="Red" Font-Bold="true" ForeColor="White" Visible="false" runat="server" Text="Label">Attention, le cadencier n’est pas à jour !</asp:Label>
            </span>
        </div>

        <div runat="server" id="divLoading"  style="display:none" ClientIDMode="Static" >
            <img width="100px" height="100px" style="margin: 20px 5px 10px 140px;" src="Images/ajax-loader.gif" />
        </div>

        <asp:ValidationSummary style="margin-top:15px; float:left;" ForeColor="Red" ID="ValSummary" runat="server" />  
    </div>

    <div class="itemSynthese" style="display: none">
        <h1><asp:Label ID="lblHeaderSynthese" runat="server"></asp:Label> </h1> 

        <div class="RepeaterSynthese">             

        <asp:PlaceHolder ID="phHeader" Visible='false' runat="server" >
            <asp:Label ID="lblEmpty" runat="server" Text="Il n'y a pas des données disponibles !"> </asp:Label>   
        </asp:PlaceHolder>

        <asp:Repeater ID="rptSynthese" runat="server" ItemType="CompteResultat.DAL.Synthese" SelectMethod="GetSynthese" OnItemDataBound="rptSynthese_ItemDataBound" >
            <HeaderTemplate>      
                  <table>
                      <tr><th>Assureur</th><th>Entreprise&nbsp;&nbsp;&nbsp;&nbsp;</th><th>AnnéeSurv</th><th>Prestations</th><th>Provisions</th><th>Cot.Brutes</th><th>Chargements</th>
                          <th>Cot.Nettes</th><th>Ratio&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</th><th>Pertes/bénéfices</th><th>Coeff.Prov</th><th>Frais&nbsp;Réel</th><th>Remb.SS</th><th>Remb.Annexe</th><th>Arretee</th>
                      </tr>                    
            </HeaderTemplate>
            <FooterTemplate>
                </table>                
            </FooterTemplate>

            <ItemTemplate>
                <tr>
                    <td style="padding: 1px 8px;"><%#: Item.Assur %></td>
                    <td><%#: Item.Company %></td>
                    <td><%#: Item.Annee %></td>
                    <td><%#: Item.Prestations %></td>
                    <td><%#: Item.Provisions %></td>
                    <td><%#: Item.CotBrut %></td>
                    <td><%#: Item.Chargements %></td>
                    <td><%#: Item.CotNet %></td>
                    <td><%#: Item.Ratio %></td>
                    <td><%#: Item.GainLoss %></td>
                    <td><%#: Item.CoeffProv %></td>
                    <td><%#: Item.FR %></td>
                    <td><%#: Item.RSS %></td>
                    <td><%#: Item.RAnnexe %></td>
                    <td><%#: Item.DateArrete %></td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="alternate">
                    <td style="padding: 1px 8px;"><%#: Item.Assur %></td>
                    <td><%#: Item.Company %></td>
                    <td><%#: Item.Annee %></td>
                    <td><%#: Item.Prestations %></td>
                    <td><%#: Item.Provisions %></td>
                    <td><%#: Item.CotBrut %></td>
                    <td><%#: Item.Chargements %></td>
                    <td><%#: Item.CotNet %></td>
                    <td><%#: Item.Ratio %></td>
                    <td><%#: Item.GainLoss %></td>
                    <td><%#: Item.CoeffProv %></td>
                    <td><%#: Item.FR %></td>
                    <td><%#: Item.RSS %></td>
                    <td><%#: Item.RAnnexe %></td>
                    <td><%#: Item.DateArrete %></td>
                </tr>
            </AlternatingItemTemplate>
            
        </asp:Repeater>
            
        </div>    

    </div>

    </div>

</asp:Content>


