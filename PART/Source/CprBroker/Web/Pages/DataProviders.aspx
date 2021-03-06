﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataProviders.aspx.cs"
    Inherits="CprBroker.Web.Pages.DataProviders" MasterPageFile="~/Pages/Site.Master"
    Title="Data providers" %>

<%@ MasterType VirtualPath="~/Pages/Site.Master" %>
<%@ Register Assembly="CprBroker.Web" Namespace="CprBroker.Web.Controls" TagPrefix="cc1" %>
<%@ Register Src="~/Pages/Controls/ConfigPropertyViewer.ascx" TagPrefix="uc1" TagName="ConfigPropertyViewer" %>
<%@ Register Src="~/Pages/Controls/ConfigPropertyEditor.ascx" TagPrefix="uc1" TagName="ConfigPropertyEditor" %>
<%@ Register Src="~/Pages/Controls/ConfigPropertyGridEditor.ascx" TagPrefix="uc1"
    TagName="ConfigPropertyGridEditor" %>
<%@ Import Namespace="CprBroker.Data.DataProviders" %>
<%@ Import Namespace="CprBroker.Engine" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Contents">
    <h3>Data provider types</h3>
    Possible types of data providers
    <asp:GridView ID="dataProviderTypesGridView" runat="server" AutoGenerateColumns="False"
        DataKeyNames="AssemblyQualifiedName" OnDataBinding="dataProviderTypesGridView_DataBinding">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" />
            <asp:BoundField DataField="AssemblyQualifiedName" HeaderText="Assembly qualified name" />
        </Columns>
    </asp:GridView>
    <h3>Data providers</h3>
    Available data providers. They will be used in the order listed here.
    <asp:GridView ID="dataProvidersGridView" runat="server" AutoGenerateColumns="False"
        DataKeyNames="DataProviderId" EmptyDataText="(None)" OnRowCommand="dataProvidersGridView_RowCommand"
        OnRowUpdating="dataProvidersGridView_RowUpdating" OnDataBinding="dataProvidersGridView_DataBinding"
        OnRowDeleting="dataProvidersGridView_RowDeleting" OnRowCancelingEdit="dataProvidersGridView_RowCancelingEdit"
        OnRowEditing="dataProvidersGridView_RowEditing" OnDataBound="dataProvidersGridView_DataBound">
        <Columns>
            <asp:TemplateField HeaderText="Type">
                <ItemTemplate>
                    <%# GetShortTypeName(Eval("TypeName").ToString()) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Details">
                <ItemTemplate>
                    <uc1:ConfigPropertyViewer runat="server" DataSource='<%# new DataProviderFactory().CreateDataProvider(Container.DataItem as DataProvider).ToDisplayableProperties() %>' />
                </ItemTemplate>
                <EditItemTemplate>
                    <uc1:ConfigPropertyEditor ID="configEditor" runat="server" DataSource='<%# new DataProviderFactory().CreateDataProvider(Container.DataItem as DataProvider).ToDisplayableProperties() %>' />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" ControlStyle-CssClass="CommandButton" />
            <asp:TemplateField HeaderText="Enabled" ItemStyle-HorizontalAlign="Center" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <table>
                        <tr>
                            <td>
                                <%#(bool)Eval("IsEnabled")?"Yes":"No" %>
                            </td>
                            <td></td>
                            <td>
                                <asp:LinkButton ID="LinkButton1" runat="server" CommandName="Enable" CommandArgument='<%#Eval("DataProviderId") %>'
                                    Text='<%#(bool)Eval("IsEnabled")?"(Disable)" : "(Enable)" %>' CssClass="CommandButton" />
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False" ControlStyle-CssClass="CommandButton">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton2" runat="server" CommandArgument='<%# Eval("DataProviderId") %>'
                        CommandName="Ping">Ping</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False" ControlStyle-CssClass="CommandButton" HeaderText="Auto Update ready">
                <ItemTemplate>
                    <table style="text-align: center; width: 100%">
                        <tr>
                            <td><%# GetIsReady(Container.DataItem) %></td>
                            <td>
                                <asp:LinkButton ID="LinkButton3" runat="server" CommandArgument='<%# Eval("DataProviderId") %>' Visible='<%# (bool) typeof(IAutoUpdateDataProvider).IsAssignableFrom(Type.GetType(Eval("TypeName").ToString())) %>'
                                    CommandName="AutoUpdateHint" CausesValidation="false">(Script)</asp:LinkButton></td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Subscriptions">
                <ItemTemplate>
                    <button runat="server" type="button" class="CommandButton"
                        visible='<%# typeof(ISubscriptionManagerDataProvider).IsAssignableFrom(Type.GetType(Eval("TypeName").ToString())) %>'
                        name="openDataProviderModal" dataproviderid='<%#Eval("DataProviderId") %>'>
                        Manage
                    </button>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowDeleteButton="True" ControlStyle-CssClass="CommandButton" />
            <asp:TemplateField ControlStyle-CssClass="UpDownButton">
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="UpButton" CommandName="Up"
                        CommandArgument='<%# Eval("DataProviderId") %>' >
                        <span class="glyphicon glyphicon-chevron-up"></span>
                    </asp:LinkButton>
                </ItemTemplate>
                <EditItemTemplate>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ControlStyle-CssClass="UpDownButton">
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="DownButton" CommandName="Down"
                        CommandArgument='<%# Eval("DataProviderId") %>' >
                        <span class="glyphicon glyphicon-chevron-down"></span>
                    </asp:LinkButton>
                </ItemTemplate>
                <EditItemTemplate>
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <h3>New data provider</h3>
    Type:
    <asp:DropDownList ID="newDataProviderDropDownList" runat="server" AutoPostBack="True"
        DataTextField="FullName" DataValueField="AssemblyQualifiedName" OnSelectedIndexChanged="newDataProviderDropDownList_SelectedIndexChanged"
        OnDataBinding="newDataProviderDropDownList_DataBinding">
    </asp:DropDownList>
    <uc1:ConfigPropertyGridEditor runat="server" ID="newDataProvider" OnDataBinding="newDataProvider_DataBinding"
        OnInsertCommand="newDataProvider_InsertCommand" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="Footer">
    <!-- Modal dialog box to show individual subscriptions - hidden by default-->
    <div id="dataProviderModal" class="modal static">

        <!-- Modal HTML -->
        <div class="modal-dialog  modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Subscriptions</h4>
                </div>
                <div class="modal-body">
                    <span id="dataProviderPopup"></span>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript" language="javascript">

        $('[name="openDataProviderModal"]').unbind('click');

        $('[name="openDataProviderModal"]').click(function () {

            var dataProviderId = this.getAttribute('dataProviderId');
            var url = '/mvc/pages/dataproviders/' + dataProviderId;

            $.get(url)
                .done(function (data, textStatus, jqXHR) {
                    $('#dataProviderPopup').html(data);
                    setDataProviderButtonEvents();
                    $('#dataProviderModal').modal('show');
                })
            .fail(function () {
                alert('failed');
            });
        });

    </script>
</asp:Content>
