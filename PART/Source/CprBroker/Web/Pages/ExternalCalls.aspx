<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExternalCalls.aspx.cs"
    Inherits="CprBroker.Web.Pages.ExternalCalls" MasterPageFile="~/Pages/Site.Master"
    Title="External calls" %>

<%@ Register Src="~/Controls/PeriodSelector.ascx" TagPrefix="uc" TagName="PeriodSelector" %>
<%@ Register Src="~/Controls/Pager.ascx" TagPrefix="uc" TagName="Pager" %>
<asp:Content runat="server" ContentPlaceHolderID="Contents">
    <table width="100%" border="0" cellpadding="3px">
        <tr>
            <td class="LogCriteriaHeader">
                <b>Date</b>
            </td>
            <td>
                <uc:PeriodSelector runat="server" ID="periodSelector" />
            </td>
            <td>
                <b>Provider</b>
            </td>
            <td>
                <asp:DataList runat="server" ID="providerListView" ExtractTemplateColumns="True"
                    OnDataBinding="providerListView_DataBinding" RepeatDirection="Horizontal">
                    <HeaderTemplate>
                        <a id="A1" runat="server" href='<%# CreateTypeLink("") %>' class='<%# string.IsNullOrEmpty(Request.Params["Type"])? "CurrentCriteriaButton": "CriteriaButton" %>'
                            style="width: 100%">All types</a>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <a href="<%# CreateTypeLink(Container.DataItem) %>" class='<%# Container.DataItem.ToString().Equals(Request.Params["Prov"], StringComparison.InvariantCultureIgnoreCase)? "CurrentCriteriaButton": "CriteriaButton" %>'
                            style="width: 100px">
                            <%# Container.DataItem %></a>
                    </ItemTemplate>
                </asp:DataList>
            </td>
        </tr>
    </table>
    <br />
    <table width="100%">
        <tr>
            <td align="left" style="width: 50%">
                <table runat="server" id="summaryTable">
                    <tr>
                        <td class="SummaryTitle">
                            Totals
                        </td>
                        <td class="SummaryName">
                            calls:
                        </td>
                        <td class="SummaryValue">
                            <%# this.RowCount %>
                        </td>
                        <td class="SummaryName">
                            cost:
                        </td>
                        <td class="SummaryValue">
                            <%# this.Cost %>
                        </td>
                    </tr>
                </table>
            </td>
            <td align="right">
                <uc:Pager runat="server" ID="pager" PagedControlID="grdCalls" />
            </td>
        </tr>
    </table>
    <asp:LinqDataSource ID="callsLinqDataSource" runat="server" ContextTypeName="CprBroker.Data.Applications.ApplicationDataContext"
        TableName="DataProviderCalls" AutoPage="False" OnSelecting="callsLinqDataSource_Selecting">
    </asp:LinqDataSource>
    <asp:ListView runat="server" ID="grdCalls" DataSourceID="callsLinqDataSource" AutoGenerateColumns="false"
        ExtractTemplateRows="true">
        <LayoutTemplate>
            <table id="Table1" runat="server" width="100%">
                <tr style="height: 50px;" class="GridHeader">
                    <th>
                        CallTime
                    </th>
                    <th>
                        DataProviderType
                    </th>
                    <th>
                        Operation
                    </th>
                    <th>
                        Input
                    </th>
                    <th>
                        Cost
                    </th>
                    <th>
                        Success
                    </th>
                </tr>
                <tr runat="server" id="itemPlaceholder">
                </tr>
                <tr>
                    <td colspan="5">
                    </td>
                </tr>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
        </ItemTemplate>
        <ItemTemplate>
            <tr id="Tr1" runat="server" class='<%# Container.DisplayIndex%2==1? "AlternateRow": "Row" %>'>
                <td>
                    <%# Eval("CallTime") %>
                </td>
                <td>
                    <%# Eval("DataProviderType") %>
                </td>
                <td>
                    <%# Eval("Operation") %>
                </td>
                <td>
                    <%# Eval("Input") %>
                </td>
                <td>
                    <%# Eval("Cost") %>
                </td>
                <td>
                    <%# Eval("Success") %>
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>
