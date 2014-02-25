<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Pager.ascx.cs" Inherits="CprBroker.Web.Controls.Pager" %>
<table>
    <tr>
        <td style="padding-right: 20px;">
            <b style="vertical-align: middle;">Go to page</b>
        </td>
        <td nowrap="nowrap">
            <asp:DataPager runat="server" ID="pager" QueryStringField="Page" PageSize="20">
                <Fields>
                    <asp:NextPreviousPagerField FirstPageText="&nbsp;&nbsp;&lt;&lt; First&nbsp;&nbsp;...&nbsp;&nbsp;"
                        ShowFirstPageButton="True" ShowPreviousPageButton="false" ShowNextPageButton="false"
                        ButtonType="Link" ButtonCssClass="PagerPage" RenderDisabledButtonsAsLabels="false" />
                    <asp:NumericPagerField ButtonType="Image" CurrentPageLabelCssClass="PagerCurrent"
                        NumericButtonCssClass="PagerPage" NextPreviousButtonCssClass="PagerPage" PreviousPageText="<"
                        NextPageText=">" ButtonCount="10" />
                    <asp:NextPreviousPagerField LastPageText="&nbsp;&nbsp;...&nbsp;Last&nbsp;&gt;&gt;&nbsp;&nbsp;"
                        ShowLastPageButton="True" ShowPreviousPageButton="false" ShowNextPageButton="false"
                        ButtonType="Link" ButtonCssClass="PagerPage" />
                </Fields>
            </asp:DataPager>
        </td>
    </tr>
</table>
