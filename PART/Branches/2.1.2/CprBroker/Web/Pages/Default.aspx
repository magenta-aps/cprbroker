<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CprBroker.Web.Pages.Default" MasterPageFile="~/Pages/Site.Master" Title="Home" %>

<asp:Content runat="server" ContentPlaceHolderID="Contents">
    <asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" ShowStartingNode="False" />
        <asp:Menu ID="Menu1" runat="server" DataSourceID="SiteMapDataSource1">
    </asp:Menu>        
</asp:Content>
