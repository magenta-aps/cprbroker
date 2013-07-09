<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataProviders.aspx.cs"
    Inherits="CprBroker.Web.Pages.DataProviders" MasterPageFile="~/Pages/Site.Master"
    Title="Data providers" %>

<%@ MasterType VirtualPath="~/Pages/Site.Master" %>
<%@ Register Assembly="CprBroker.Web" Namespace="CprBroker.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Contents">
    <h3>
        Data provider types</h3>
    Possible types of data providers
    <asp:GridView ID="dataProviderTypesGridView" runat="server" AutoGenerateColumns="False"
        DataKeyNames="AssemblyQualifiedName" OnDataBinding="dataProviderTypesGridView_DataBinding">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Name" />
            <asp:BoundField DataField="AssemblyQualifiedName" HeaderText="Assembly qualified name" />
        </Columns>
    </asp:GridView>
    <h3>
        Data providers</h3>
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
                    <asp:Repeater ID="DataList1" runat="server" DataSource='<%# GetAttributes(Container.DataItem) %>'>
                        <ItemTemplate>
                            <b>
                                <%# Eval("Name")%>:</b>
                            <%# (bool)Eval("Confidential")?"********": Eval("Value")%>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            &nbsp;</SeparatorTemplate>
                    </asp:Repeater>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DataList ID="EditDataList" runat="server" DataSource='<%# GetAttributes(Container.DataItem) %>' RepeatColumns="3"
                        DataKeyField="Name" RepeatDirection="Horizontal">
                        <ItemTemplate>
                            <b>
                                <%# Eval("Name")%>:</b>
                            <cc1:SmartTextBox ID="SmartTextBox" runat="server" Type='<%# Eval("Type") %>' Text='<%# Bind("Value") %>' Required='<%# Bind("Required") %>' Confidential='<%# Bind("Confidential") %>' />
                        </ItemTemplate>
                    </asp:DataList>
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
                            <td>
                            </td>
                            <td>
                                <asp:LinkButton ID="LinkButton1" runat="server" CommandName="Enable" CommandArgument='<%#Eval("DataProviderId") %>'
                                    Text='<%#(bool)Eval("IsEnabled")?"(Disable)" : "(Enable)" %>' CssClass="CommandButton" />
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
                <EditItemTemplate>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False" ControlStyle-CssClass="CommandButton">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton2" runat="server" CommandArgument='<%# Eval("DataProviderId") %>'
                        CommandName="Ping">Ping</asp:LinkButton>
                </ItemTemplate>
                <EditItemTemplate>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowDeleteButton="True" ControlStyle-CssClass="CommandButton" />
            <asp:TemplateField ControlStyle-CssClass="UpDownButton">
                <ItemTemplate>
                    <asp:ImageButton runat="server" ID="UpButton" ImageUrl="Images/Up.jpg" CommandName="Up"
                        CommandArgument='<%# Eval("DataProviderId") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ControlStyle-CssClass="UpDownButton">
                <ItemTemplate>
                    <asp:ImageButton runat="server" ID="DownButton" ImageUrl="Images/Down.jpg" CommandName="Down"
                        CommandArgument='<%# Eval("DataProviderId") %>' />
                </ItemTemplate>
                <EditItemTemplate>
                </EditItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <h3>
        New data provider</h3>
    Type:
    <asp:DropDownList ID="newDataProviderDropDownList" runat="server" AutoPostBack="True"
        DataTextField="FullName" DataValueField="AssemblyQualifiedName" OnSelectedIndexChanged="newDataProviderDropDownList_SelectedIndexChanged"
        OnDataBinding="newDataProviderDropDownList_DataBinding">
    </asp:DropDownList>
    <asp:GridView runat="server" ID="newDataProviderGridView" AutoGenerateColumns="false"
        DataKeyNames="Name" ShowFooter="true" OnRowCommand="newDataProviderGridView_RowCommand"
        OnDataBinding="newDataProviderGridView_DataBinding">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <%# Eval("Name") %>:
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Button ID="InsertButton" runat="server" CommandName="Insert" Text="Insert" CssClass="CommandButton"
                        ValidationGroup="Add"></asp:Button>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:SmartTextBox ID="SmartTextBox" runat="server" Type='<%# Eval("Type") %>' Required='<%# Eval("Required") %>'
                        Confidential='<%# Eval("Confidential") %>' ValidationGroup="Add"  />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
