<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExternalCalls.aspx.cs"
    Inherits="CprBroker.Web.Pages.ExternalCalls" MasterPageFile="~/Pages/Site.Master"
    Title="External calls" %>

<%@ Register Src="~/Controls/PeriodSelector.ascx" TagPrefix="uc" TagName="PeriodSelector" %>
<%@ Register Src="~/Controls/Pager.ascx" TagPrefix="uc" TagName="Pager" %>
<asp:Content runat="server" ContentPlaceHolderID="Contents">
    <table>
        <tr>
            <td class="LogCriteriaHeader">
                Date
            </td>
            <td>
                <uc:PeriodSelector runat="server" ID="periodSelector" />
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
                    <%# Eval("Cost") %>
                </td>
                <td>
                    <%# Eval("Success") %>
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>
