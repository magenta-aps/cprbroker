<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dbr.aspx.cs" Inherits="CprBroker.Web.Pages.Dbr"
    MasterPageFile="~/Pages/Site.Master" %>

<%@ MasterType VirtualPath="~/Pages/Site.Master" %>
<%@ Register Assembly="CprBroker.Web" Namespace="CprBroker.Web.Controls" TagPrefix="cc1" %>
<%@ Register Src="~/Pages/Controls/ConfigPropertyViewer.ascx" TagPrefix="uc1" TagName="ConfigPropertyViewer" %>
<%@ Register Src="~/Pages/Controls/ConfigPropertyEditor.ascx" TagPrefix="uc1" TagName="ConfigPropertyEditor" %>
<%@ Register Src="~/Pages/Controls/ConfigPropertyGridEditor.ascx" TagPrefix="uc1" TagName="ConfigPropertyGridEditor" %>
<%@ Import Namespace="CprBroker.Engine" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Contents">
    <h3>
        DBR</h3>
    <h4>
        Sync targets</h4>
    <asp:GridView runat="server" ID="grdDbr" OnDataBinding="grdDbr_DataBinding" AutoGenerateColumns="false"
        OnRowCommand="grdDbr_RowCommand" OnRowDeleting="grdDbr_RowDeleting" OnRowEditing="grdDbr_RowEditing"
        OnRowCancelingEdit="grdDbr_RowCancelingEdit" OnRowUpdating="grdDbr_RowUpdating"
        DataKeyNames="QueueId">
        <EmptyDataTemplate>
            (None)</EmptyDataTemplate>
        <Columns>
            <asp:TemplateField HeaderText="Details">
                <ItemTemplate>
                    <uc1:ConfigPropertyViewer ID="ConfigPropertyViewer1" runat="server" DataSource='<%# (Container.DataItem as IHasConfigurationProperties).ToDisplayableProperties() %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <uc1:ConfigPropertyEditor ID="configEditor" runat="server" DataSource='<%# (Container.DataItem as IHasConfigurationProperties).ToDisplayableProperties() %>' />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False" ControlStyle-CssClass="CommandButton">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton2" runat="server" CommandArgument='<%# Eval("Impl.QueueId") %>'
                        CommandName="Ping">Ping</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" ControlStyle-CssClass="CommandButton" />
            <asp:CommandField ShowDeleteButton="True" ControlStyle-CssClass="CommandButton" />
        </Columns>
    </asp:GridView>
    <h4>
        New sync target</h4>
        <uc1:ConfigPropertyGridEditor runat="server" id="newDbr" OnDataBinding="newDbr_DataBinding" OnInsertCommand="newDbr_InsertCommand" />
</asp:Content>
