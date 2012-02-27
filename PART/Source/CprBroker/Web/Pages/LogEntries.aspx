<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogEntries.aspx.cs" Inherits="CprBroker.Web.Pages.LogEntries"
    MasterPageFile="~/Pages/Site.Master" Title="Log" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Contents">
    <asp:LinqDataSource ID="logEntriesLinqDataSource" runat="server" ContextTypeName="CprBroker.Data.Applications.ApplicationDataContext"
        OrderBy="LogDate desc" TableName="LogEntries" AutoPage="False" 
        onselected="logEntriesLinqDataSource_Selected" 
        onselecting="logEntriesLinqDataSource_Selecting">
    </asp:LinqDataSource>
    <table>
        <tr>
            <td style="padding-right: 20px;">
                <b style="vertical-align: middle;">Go to page</b>
            </td>
            <td nowrap="nowrap">
                <asp:DataPager runat="server" ID="pager" QueryStringField="Page" PagedControlID="dlLogEntries"
                    PageSize="20">
                    <Fields>
                        <asp:NumericPagerField ButtonType="Image" CurrentPageLabelCssClass="PagerCurrent"
                            NumericButtonCssClass="PagerPage" NextPreviousButtonCssClass="PagerPage" PreviousPageText="<"
                            NextPageText=">" ButtonCount="10" />
                    </Fields>
                </asp:DataPager>
            </td>
        </tr>
    </table>
    <asp:ListView runat="server" ID="dlLogEntries" ExtractTemplateRows="true" DataSourceID="logEntriesLinqDataSource">
        <LayoutTemplate>
            <table id="Table1" runat="server" width="100%">
                <tr style="height: 50px;" class="GridHeader">
                    <th>
                        Date
                    </th>
                    <th>
                        Type
                    </th>
                    <th>
                        Application
                    </th>
                    <th>
                        User token
                    </th>
                    <th>
                        User Id
                    </th>
                    <th>
                        Method
                    </th>
                </tr>
                <tr runat="server" id="itemPlaceholder">
                </tr>
                <tr>
                    <td colspan="6">
                    </td>
                </tr>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr runat="server" class='<%# Container.DisplayIndex%2==1? "AlternateRow": "Row" %>'>
                <td>
                    <%# Eval("LogDate") %>
                </td>
                <td>
                    <%# Eval("LogType.Name") %>
                </td>
                <td>
                    <%# Eval("Application.Name") %>
                </td>
                <td>
                    <%# Eval("UserToken") %>
                </td>
                <td>
                    <%# Eval("UserId") %>
                </td>
                <td>
                    <%# Eval("MethodName") %>
                </td>
            </tr>
            <tr id="Tr1" runat="server" class='<%# Container.DisplayIndex%2==1? "AlternateRow": "Row" %>'>
                <td colspan="6">
                    <%# Eval("Text") %>
                </td>
            </tr>
            <tr id="Tr2" runat="server" class='<%# Container.DisplayIndex%2==1? "AlternateRow": "Row" %>'
                visible='<%# (Eval("DataObjectXML")!=null)%>'>
                <td>
                    <%# Eval("DataObjectType") %>
                </td>
                <td colspan="5">
                    <%# Eval("DataObjectXML") %>
                </td>
            </tr>
        </ItemTemplate>
        <ItemSeparatorTemplate>
            <tr runat="server">
                <td colspan="6" style="height: 10px;">
                </td>
            </tr>
        </ItemSeparatorTemplate>
    </asp:ListView>
</asp:Content>
