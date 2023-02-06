<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FMAnalyse.aspx.cs" Inherits="CompteResultat.FMAnalyse" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div style="float: left; width: 1100px;">
        <h1><asp:Literal   ID="Literal9" runat="server">Liste des controles :</asp:Literal></h1>         

        <dx:ASPxFileManager style="margin-top:20px;" ID="ASPxFileManager2" runat="server">
            
            <settings EnableMultiSelect="true" rootfolder="~/App_Data/Analyse" AllowedFileExtensions=".csv, .xlsx, .xlsm" />
            <SettingsFileList View="Details">
            </SettingsFileList>
            <SettingsEditing AllowDelete="True" AllowDownload="True" AllowRename="True" />
        </dx:ASPxFileManager>

    </div>

    </asp:Content>
