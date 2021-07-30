<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FileManager.aspx.cs" Inherits="CompteResultat.FileManager" %>

<%@ Register Assembly="DevExpress.Web.v17.1, Version=17.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>




<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div style="float: left; width: 800px;">
        <h1><asp:Literal   ID="Literal9" runat="server">Liste des comptes résultats :</asp:Literal></h1>         

        <dx:ASPxFileManager style="margin-top:20px;" ID="ASPxFileManager2" runat="server">
            <settings rootfolder="~/App_Data/ExcelCR" AllowedFileExtensions=".pptm, .xlsm" />
            <SettingsFileList View="Details">
            </SettingsFileList>
            <SettingsEditing AllowDelete="True" AllowDownload="True" AllowRename="True" />
        </dx:ASPxFileManager>

    </div>

    </asp:Content>
