<%@ Page Language="C#" MasterPageFile="~/Pages/Site.Master" AutoEventWireup="true"
    CodeBehind="Budget.aspx.cs" Inherits="CprBroker.Web.Pages.Budget" Title="Budget limits" %>

<%@ Import Namespace="CprBroker.Data.DataProviders" %>
<%@ Register Assembly="CprBroker.Web" Namespace="CprBroker.Web.Controls" TagPrefix="cc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Contents" runat="server">
    <h3>
        Budget limits</h3>
    <asp:LinqDataSource runat="server" ID="intervalsDataSource" ContextTypeName="CprBroker.Data.DataProviders.DataProvidersDataContext"
        TableName="BudgetIntervals" OrderBy="IntervalMilliseconds" EnableUpdate="true" />
    <asp:GridView runat="server" ID="intervalsGrid" DataSourceID="intervalsDataSource"
        AutoGenerateColumns="false" DataKeyNames="IntervalMilliseconds" HeaderStyle-HorizontalAlign="Center">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Period" ReadOnly="true" ItemStyle-Width="100"
                ItemStyle-Font-Bold="true" ItemStyle-HorizontalAlign="Center" />
            <cc:SmartTextField HeaderText="Max calls" DataField="CallThreshold" Type="Integer"
                ItemStyle-Width="150" />
            <cc:SmartTextField HeaderText="Max cost" DataField="CostThreshold" Type="Decimal"
                ItemStyle-Width="150" />
            <asp:BoundField DataField="LastChecked" HeaderText="Last check" ReadOnly="true" ItemStyle-Width="150" />
            <asp:CommandField ButtonType="Link" EditText="Edit" ShowEditButton="true" ControlStyle-CssClass="CommandButton" />
        </Columns>
    </asp:GridView>
</asp:Content>
