<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PeriodSelector.ascx.cs"
    Inherits="CprBroker.Web.Controls.PeriodSelector" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<table>
    <tr>
        <td>
            in the last
        </td>
        <td>
            <asp:ListView runat="server" ID="lvPeriod" ExtractTemplateColumns="True" OnDataBinding="lvPeriod_DataBinding">
                <LayoutTemplate>
                    <table width="35%">
                        <tr>
                            <td runat="server" id="itemPlaceholder">
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <td id="Td1" runat="server">
                        <a href="<%# CreatePeriodLink(Container.DataItem) %>" class='<%# IsInPeriodMode && Container.DataItem.ToString().Equals(Enum.GetName(typeof(CprBroker.Web.LogPeriod), CurrentPeriod), StringComparison.InvariantCultureIgnoreCase)? "CurrentCriteriaButton": "CriteriaButton" %>'
                            style="width: 100px">
                            <%# Container.DataItem %></a>
                    </td>
                </ItemTemplate>
            </asp:ListView>
        </td>
    </tr>
    <tr>
        <td>
            between
        </td>
        <td>
            <asp:UpdatePanel ID="UpdatePanel1" RenderMode="Inline" runat="server">
                <ContentTemplate>
                    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" />
                    <asp:TextBox ID="txtFrom" runat="server" Width="75px" AutoPostBack="true" OnTextChanged="txtFrom_TextChanged"></asp:TextBox>
                    <asp:CalendarExtender ID="calexFrom" TargetControlID="txtFrom" runat="server" Format="d/M/yyyy" />
                    and
                    <asp:TextBox ID="txtTo" runat="server" Width="75px" AutoPostBack="true" OnTextChanged="txtTo_TextChanged"></asp:TextBox>
                    <asp:CalendarExtender ID="calexTo" runat="server" TargetControlID="txtTo" Format="d/M/yyyy" />
                    <a runat="server" id="lnkGoDate" name="lnkGoDate" class="CommandButton" visible="false">
                        GO</a>
                </ContentTemplate>
            </asp:UpdatePanel>
        </td>
    </tr>
</table>
