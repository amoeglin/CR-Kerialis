<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionExperience.aspx.cs" Inherits="CompteResultat.GestionExperience" EnableViewState="false"%>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    
    <style>
        input[type="submit"]:disabled {background: #dddddd;}

    </style>
    <script type="text/javascript">

        $(document).ready(function () { 
            $("#cmdImport").click(function (evt) {
                $("#divLoading").css("display", "block");
            }); 

            $("#cmdRecreate").click(function (evt) {
                $("#divLoading").css("display", "block");
            });
        }); 
     
   </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div style="float: left; width: 400px;">
        <h1><asp:Literal ID="Literal9" runat="server">Liste des assureurs :</asp:Literal> </h1> 
        <div style="margin-bottom:15px; "></div>
        
        <%--
        <asp:CheckBox ID="chkAssur" runat="server" Checked="false" AutoPostBack="true"
              style="display:inline-block;margin-bottom:15px; margin-top:15px;" Text="Afficher uniquement les assureurs sans experience" /><br />  
            --%>

        <asp:ListBox ID="lbAssur" runat="server" SelectMethod="GetAssureurs" DataTextField="Name" DataValueField="Id" Height="250px" Width="100%" 
                AutoPostBack="true"  OnSelectedIndexChanged="lbAssur_SelectedIndexChanged" EnableViewState="false" OnDataBound="lbAssur_DataBound" >            
        </asp:ListBox>

        <div style="display:none;"> 
            <asp:FileUpload Width="150" ID="uploadExcel" runat="server" onchange="this.form.submit()" /> 
        </div>
        <div style="margin-top:15px; float:right;">
            <asp:Button CssClass="ButtonBigBlue ButtonInline" style="margin-right:20px; width:90px;" ID="cmdImport" runat="server" Text="Import" ClientIDMode="Static" />  
            <asp:Button CssClass="ButtonBigBlue ButtonInline" style="margin-right:20px; width:90px;" ID="cmdExport" runat="server" Text="Export" OnClick="cmdExport_Click"  /> 
            <asp:Button CssClass="ButtonBigRed ButtonInline" style="width:90px;" ID="cmdDelete" runat="server" Text="Supprimer" OnClick="cmdDelete_Click" ClientIDMode="Static"  /> 
                         
            <%-- <input type="submit" ID="cmdDelete" name="cmdDelete" class="ButtonBigRed ButtonInline" style="width:90px;" value="Supprimer" runat="server" />  --%>
        </div>

        <div style="margin-top:5px; float:right;">
            <asp:Button CssClass="ButtonBigBlue ButtonInline" style="margin-right:0px; width:320px;" ID="cmdRecreate" runat="server" Text="REGENERER à partir des PRESTATIONS" ClientIDMode="Static" OnClick="cmdRecreate_Click" />  
        </div>

        <div runat="server" id="divLoading"  style="display:none" ClientIDMode="Static" >
            <img width="100px" height="100px" style="margin: 20px 5px 10px 140px;" src="Images/ajax-loader.gif" />
        </div>

        <asp:ValidationSummary style="margin-top:15px; float:left;" ForeColor="Red" ID="ValSummary" runat="server" />  

    </div>


    <%-- Repeater --%>

    <div style="float: left; margin-left:80px; " > 

        <h1><asp:Literal  ID="Literal1" runat="server">Experience :</asp:Literal> </h1> 
        
        <div class="RepeaterExp"> 

        <asp:PlaceHolder ID="phHeader" Visible='false' runat="server">
            <asp:Label ID="lblEmpty" runat="server" Text="Il n'y a pas des données disponibles !"> </asp:Label>   
        </asp:PlaceHolder>

        <asp:Repeater ItemType="CompteResultat.DAL.C_TempExpData" SelectMethod="GetExperience" ID="rptExp" runat="server" OnItemDataBound="rptExp_ItemDataBound"    >
            <HeaderTemplate>      
                  <table>
                      <tr><th>Assureur</th><th>Au</th><th>Contrat</th><th>Code College</th><th>Année</th>
                          <th>Acte</th><th>Famille</th><th>CAS</th><th>Nombre Acte</th><th>Frais Réel</th>
                          <th>Remb SS</th><th>Remb Annexe</th><th>Remb Nous</th><th>Réseau</th>
                          <th>Min FR</th><th>Max FR</th><th>Min Nous</th><th>Max Nous</th>
                      </tr>                    
            </HeaderTemplate>
            <FooterTemplate>
                </table>                
            </FooterTemplate>

            <ItemTemplate>
                <tr>
                    <td><%#: Item.AssureurName %></td>
                    <td><%#: Item.Au.Value.ToShortDateString() %></td>
                    <td><%#: Item.Contrat %></td>
                    <td><%#: Item.CodCol %></td>
                    <td><%#: Item.AnneeExp %></td>
                    <td><%#: Item.LibActe %></td>
                    <td><%#: Item.LibFam %></td>
                    <td><%#: Item.TypeCas %></td>
                    <td><%#: Item.NombreActe %></td>
                    <td><%#: Item.Fraisreel.Value.ToString("F2") %></td>
                    <td><%#: Item.Rembss.Value.ToString("F2") %></td>
                    <td><%#: Item.RembAnnexe.Value.ToString("F2") %></td>
                    <td><%#: Item.RembNous.Value.ToString("F2") %></td>
                    <td><%#: Item.Reseau %></td>
                    <td><%#: Item.MinFr.Value.ToString("F2") %></td>
                    <td><%#: Item.MaxFr.Value.ToString("F2") %></td>
                    <td><%#: Item.MinNous.Value.ToString("F2") %></td>
                    <td><%#: Item.MaxNous.Value.ToString("F2") %></td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="alternate">
                    <td><%#: Item.AssureurName %></td>
                    <td><%#: Item.Au.Value.ToShortDateString() %></td>
                    <td><%#: Item.Contrat %></td>
                    <td><%#: Item.CodCol %></td>
                    <td><%#: Item.AnneeExp %></td>
                    <td><%#: Item.LibActe %></td>
                    <td><%#: Item.LibFam %></td>
                    <td><%#: Item.TypeCas %></td>
                    <td><%#: Item.NombreActe %></td>
                    <td><%#: Item.Fraisreel.Value.ToString("F2") %></td>
                    <td><%#: Item.Rembss.Value.ToString("F2") %></td>
                    <td><%#: Item.RembAnnexe.Value.ToString("F2") %></td>
                    <td><%#: Item.RembNous.Value.ToString("F2") %></td>
                    <td><%#: Item.Reseau %></td>
                    <td><%#: Item.MinFr.Value.ToString("F2") %></td>
                    <td><%#: Item.MaxFr.Value.ToString("F2") %></td>
                    <td><%#: Item.MinNous.Value.ToString("F2") %></td>
                    <td><%#: Item.MaxNous.Value.ToString("F2") %></td>
                </tr>
            </AlternatingItemTemplate>
            
        </asp:Repeater>
            
        </div>     

    </div>

</asp:Content>

