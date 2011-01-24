<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataProviders.aspx.cs"
    Inherits="CprBroker.Web.Pages.DataProviders" MasterPageFile="~/Pages/Site.Master"
    Title="Data providers" %>

<%@ MasterType VirtualPath="~/Pages/Site.Master" %>
<%@ Register Assembly="CprBroker.Web" Namespace="CprBroker.Web.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Contents">
    <asp:LinqDataSource ID="dataProviderTypesLinqDataSource" runat="server" ContextTypeName="CprBroker.DAL.DataProviders.DataProvidersDataContext"
        TableName="DataProviderTypes" Where="IsExternal == @IsExternal">
        <WhereParameters>
            <asp:Parameter DefaultValue="True" Name="IsExternal" Type="Boolean" />
        </WhereParameters>
    </asp:LinqDataSource>
    <h3>
        Data provider types</h3>
    Possible types of data providers
    <asp:GridView ID="dataProviderTypesGridView" runat="server" AutoGenerateColumns="False"
        DataKeyNames="DataProviderTypeId" DataSourceID="dataProviderTypesLinqDataSource">
        <Columns>
            <asp:BoundField DataField="DataProviderTypeId" HeaderText="ID" InsertVisible="False"
                ReadOnly="True" SortExpression="DataProviderTypeId" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="TypeName" HeaderText="Type name" SortExpression="TypeName" />
        </Columns>
    </asp:GridView>
    <asp:LinqDataSource ID="dataProvidersLinqDataSource" runat="server" ContextTypeName="CprBroker.DAL.DataProviders.DataProvidersDataContext"
        OrderBy="DataProviderId" TableName="DataProviders" Where="DataProviderType.IsExternal == @IsExternal"
        EnableUpdate="True" EnableInsert="True" OnInserting="dataProvidersLinqDataSource_Inserting"
        EnableDelete="True" OnDeleted="dataProvidersLinqDataSource_Deleted" OnInserted="dataProvidersLinqDataSource_Inserted"
        OnUpdated="dataProvidersLinqDataSource_Updated" OnUpdating="dataProvidersLinqDataSource_Updating">
        <WhereParameters>
            <asp:Parameter DefaultValue="True" Name="IsExternal" Type="Boolean" />
        </WhereParameters>
    </asp:LinqDataSource>
    <h3>
        Data providers</h3>
    Available data providers. They will be used in the order listed here.
    <asp:GridView ID="dataProvidersGridView" runat="server" AutoGenerateColumns="False"
        DataKeyNames="DataProviderId" DataSourceID="dataProvidersLinqDataSource" EmptyDataText="(None)"
        OnRowCommand="dataProvidersGridView_RowCommand" OnRowUpdating="dataProvidersGridView_RowUpdating"
        OnRowUpdated="dataProvidersGridView_RowUpdated">
        <Columns>
            <asp:TemplateField HeaderText="Details">
                <ItemTemplate>
                    <asp:DataList ID="DataList1" runat="server" DataSource='<%# Eval("DataProviderProperties") %>'
                        RepeatDirection="Horizontal">
                        <ItemTemplate>
                            <b>
                                <%# Eval("Name")%>:</b>
                            <%# Eval("Value")%>
                        </ItemTemplate>
                    </asp:DataList>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:DataList ID="EditDataList" runat="server" DataSource='<%# Eval("DataProviderProperties") %>'
                        DataKeyField="Name" RepeatDirection="Horizontal">
                        <ItemTemplate>
                            <b>
                                <%# Eval("Name")%>:</b>
                            <cc1:SmartTextBox ID="SmartTextBox" runat="server" Text='<%# Bind("Value") %>' Required="True" />
                        </ItemTemplate>
                    </asp:DataList>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField Visible="false" HeaderText="Type" SortExpression="DataProviderTypeId">
                <EditItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("DataProviderType.Name") %>'></asp:Label>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField Visible="false" HeaderText="Address" SortExpression="Address">
                <EditItemTemplate>
                    <cc1:SmartTextBox ID="SmartTextBox1" runat="server" Text='<%# Bind("Address") %>'
                        Required="True" />
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" ControlStyle-CssClass="CommandButton" />
            <asp:CommandField ShowDeleteButton="True" ControlStyle-CssClass="CommandButton" />
            <asp:TemplateField ShowHeader="False" ControlStyle-CssClass="CommandButton">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton2" runat="server" CommandArgument='<%# Eval("DataProviderId") %>'
                        CommandName="Ping">Ping</asp:LinkButton>
                </ItemTemplate>
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
    <asp:GridView runat="server" ID="newDataProviderGridView" 
        AutoGenerateColumns="false" ShowFooter="true" 
        onrowcommand="newDataProviderGridView_RowCommand">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <%# Container.DataItem %>:
                </ItemTemplate>
                <FooterTemplate>
                    <asp:Button ID="InsertButton" runat="server" CommandName="Insert" Text="Insert" CssClass="CommandButton"
                        ValidationGroup="Add"></asp:Button>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <cc1:SmartTextBox ID="SmartTextBox" runat="server" Required="True" ValidationGroup="Add" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    
</asp:Content>
