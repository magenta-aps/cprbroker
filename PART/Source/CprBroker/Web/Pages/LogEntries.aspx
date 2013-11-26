<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogEntries.aspx.cs" Inherits="CprBroker.Web.Pages.LogEntries"
    MasterPageFile="~/Pages/Site.Master" Title="Log" %>

<%@ Register Assembly="CprBroker.Web" Namespace="CprBroker.Web.Controls" TagPrefix="cc" %>
<%@ Register Src="~/Controls/PeriodSelector.ascx" TagPrefix="uc" TagName="PeriodSelector" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Contents">
    <table width="100%" border="0" cellpadding="3px">
        <tr>
            <td rowspan="2" class="LogCriteriaHeader">
                <b>Date</b>
            </td>
            <td style="width: 25%" nowrap="nowrap">
                <uc:PeriodSelector runat="server" id="periodSelector" />
            </td>
            <td rowspan="2" class="LogCriteriaHeader">
                <b>Type</b>
            </td>
            <td rowspan="2">
                <asp:DataList runat="server" ID="lvType" ExtractTemplateColumns="True" OnDataBinding="lvType_DataBinding"
                    RepeatDirection="Horizontal">
                    <HeaderTemplate>
                        <a id="A1" runat="server" href='<%# CreateTypeLink("") %>' class='<%# string.IsNullOrEmpty(Request.Params["Type"])? "CurrentCriteriaButton": "CriteriaButton" %>'
                            style="width: 100%">All types</a>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <a href="<%# CreateTypeLink(Container.DataItem) %>" class='<%# Container.DataItem.ToString().Equals(Request.Params["Type"], StringComparison.InvariantCultureIgnoreCase)? "CurrentCriteriaButton": "CriteriaButton" %>'
                            style="width: 100px">
                            <%# Container.DataItem %></a>
                    </ItemTemplate>
                </asp:DataList>
            </td>
        </tr>
        
        <tr>
            <td colspan="4">
            </td>
        </tr>
        <tr>
            <td class="LogCriteriaHeader">
                <b>Application</b>
            </td>
            <td colspan="4">
                <asp:LinqDataSource ID="applicationsLinqDataSource" runat="server" ContextTypeName="CprBroker.Data.Applications.ApplicationDataContext"
                    TableName="Applications" OrderBy="RegistrationDate" EnableUpdate="False" EnableInsert="False"
                    EnableDelete="False">
                </asp:LinqDataSource>
                <asp:DataList runat="server" ID="lvApplications" DataSourceID="applicationsLinqDataSource"
                    RepeatDirection="Horizontal" RepeatColumns="5" Width="75%">
                    <ItemStyle Width="20%" Wrap="false" />
                    <HeaderTemplate>
                        <a href="<%# CreateAppLink("") %>" class='<%# string.IsNullOrEmpty(Request.Params["App"])? "CurrentCriteriaButton": "CriteriaButton" %>'
                            style="width: 100%">All applications</a>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <a href="<%# CreateAppLink(DataBinder.Eval(Container.DataItem,"Name")) %>" class='<%# DataBinder.Eval(Container.DataItem,"Name").ToString().Equals(Request.Params["App"], StringComparison.InvariantCultureIgnoreCase)? "CurrentCriteriaButton": "CriteriaButton" %>'
                            style="width: 100%">
                            <%# DataBinder.Eval(Container.DataItem, "Name")%></a>
                    </ItemTemplate>
                </asp:DataList>
            </td>
        </tr>
        <tr>
            <td colspan="5" style="height: 5px">
            </td>
        </tr>
    </table>
    <asp:LinqDataSource ID="logEntriesLinqDataSource" runat="server" ContextTypeName="CprBroker.Data.Applications.ApplicationDataContext"
        OrderBy="LogDate desc" TableName="LogEntries" AutoPage="False" OnSelected="logEntriesLinqDataSource_Selected"
        OnSelecting="logEntriesLinqDataSource_Selecting">
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
                        <asp:NextPreviousPagerField FirstPageText="&nbsp;&nbsp;&lt;&lt; First&nbsp;&nbsp;...&nbsp;&nbsp;" ShowFirstPageButton="True" ShowPreviousPageButton="false"
                            ShowNextPageButton="false" ButtonType="Link" ButtonCssClass="PagerPage" RenderDisabledButtonsAsLabels="false" />
                        <asp:NumericPagerField ButtonType="Image" CurrentPageLabelCssClass="PagerCurrent"
                            NumericButtonCssClass="PagerPage" NextPreviousButtonCssClass="PagerPage" PreviousPageText="<"
                            NextPageText=">" ButtonCount="10" />
                        <asp:NextPreviousPagerField LastPageText="&nbsp;&nbsp;...&nbsp;Last&nbsp;&gt;&gt;&nbsp;&nbsp;" ShowLastPageButton="True" ShowPreviousPageButton="false"
                            ShowNextPageButton="false" ButtonType="Link" ButtonCssClass="PagerPage" />
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
