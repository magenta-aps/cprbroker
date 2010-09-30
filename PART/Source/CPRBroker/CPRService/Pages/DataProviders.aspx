<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataProviders.aspx.cs"
    Inherits="CPRService.Pages.DataProviders" MasterPageFile="~/Pages/Site.Master"
    Title="Data providers" %>

<%@ MasterType VirtualPath="~/Pages/Site.Master" %>
<%@ Register Assembly="CPRService" Namespace="CPRService.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Contents">
    <asp:LinqDataSource ID="dataProviderTypesLinqDataSource" runat="server" ContextTypeName="CPRBroker.DAL.CPRBrokerDALDataContext"
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
    <asp:LinqDataSource ID="dataProvidersLinqDataSource" runat="server" ContextTypeName="CPRBroker.DAL.CPRBrokerDALDataContext"
        OrderBy="DataProviderId" TableName="DataProviders" Where="DataProviderType.IsExternal == @IsExternal"
        EnableUpdate="True" EnableInsert="True" OnInserting="dataProvidersLinqDataSource_Inserting"
        EnableDelete="True" OnDeleted="dataProvidersLinqDataSource_Deleted" OnInserted="dataProvidersLinqDataSource_Inserted"
        OnUpdated="dataProvidersLinqDataSource_Updated">
        <WhereParameters>
            <asp:Parameter DefaultValue="True" Name="IsExternal" Type="Boolean" />
        </WhereParameters>
    </asp:LinqDataSource>
    <h3>
        Data providers</h3>
        Available data providers. They will be used in the order listed here.
    <asp:GridView ID="dataProvidersGridView" runat="server" AutoGenerateColumns="False"
        DataKeyNames="DataProviderId" DataSourceID="dataProvidersLinqDataSource" EmptyDataText="(None)"
        OnRowCommand="dataProvidersGridView_RowCommand">
        <Columns>
            <asp:BoundField DataField="DataProviderId" HeaderText="ID" InsertVisible="False"
                ReadOnly="True" SortExpression="DataProviderId" />
            <asp:TemplateField HeaderText="Type" SortExpression="DataProviderTypeId">
                <EditItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("DataProviderType.Name") %>'></asp:Label>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("DataProviderType.Name") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Address" SortExpression="Address">
                <EditItemTemplate>
                    <cc1:SmartTextBox ID="SmartTextBox1" runat="server" Text='<%# Bind("Address") %>'
                        Required="True" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Address") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Port" SortExpression="Port">
                <EditItemTemplate>
                    <cc1:SmartTextBox ID="SmartTextBox2" runat="server" Required="True" ValidationExpression="\d+"
                        Text='<%# Bind("Port") %>' Visible='<%# (int)Eval("DataProviderTypeId")== (int)CPRBroker.Schemas.DataProviderTypes.DPR %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label3" runat="server" Text='<%# Bind("Port") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Connection string" SortExpression="ConnectionString">
                <EditItemTemplate>
                    <cc1:SmartTextBox ID="SmartTextBox3" runat="server" Required="True" Text='<%# Bind("ConnectionString") %>'
                        Visible='<%# (int)Eval("DataProviderTypeId")== (int)CPRBroker.Schemas.DataProviderTypes.DPR %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("ConnectionString") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="User name" SortExpression="UserName">
                <EditItemTemplate>
                    <cc1:SmartTextBox ID="SmartTextBox4" runat="server" Required="True" Text='<%# Bind("UserName") %>'
                        Visible='<%# (int)Eval("DataProviderTypeId")== (int)CPRBroker.Schemas.DataProviderTypes.KMD %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label5" runat="server" Text='<%# Bind("UserName") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Password" SortExpression="Password">
                <EditItemTemplate>
                    <cc1:SmartTextBox ID="SmartTextBox5" runat="server" Required="True" Text='<%# Bind("Password") %>'
                        Visible='<%# (int)Eval("DataProviderTypeId")== (int)CPRBroker.Schemas.DataProviderTypes.KMD %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label6" runat="server" Text='<%# Bind("Password") %>'></asp:Label>
                </ItemTemplate>
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
        DataSourceID="dataProviderTypesLinqDataSource" DataTextField="Name" DataValueField="DataProviderTypeId"
        OnSelectedIndexChanged="newDataProviderDropDownList_SelectedIndexChanged">
    </asp:DropDownList>
    <asp:DetailsView ID="newDataProviderDetailsView" runat="server" DefaultMode="Insert"
        AutoGenerateRows="False" DataKeyNames="DataProviderId" DataSourceID="dataProvidersLinqDataSource"
        OnItemInserted="newDataProviderDetailsView_ItemInserted" CssClass="detailsView">
        <Fields>
            <asp:TemplateField HeaderText="Address" SortExpression="Address">
                <InsertItemTemplate>
                    <cc1:SmartTextBox ID="SmartTextBox1" runat="server" Text='<%# Bind("Address") %>'
                        Required="True" ValidationGroup="Add" />
                </InsertItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Port" SortExpression="Port">
                <InsertItemTemplate>
                    <cc1:SmartTextBox ID="SmartTextBox2" runat="server" Required="True" ValidationExpression="\d+"
                        Text='<%# Bind("Port") %>' Visible='<%# int.Parse(newDataProviderDropDownList.SelectedValue)== (int)CPRBroker.Schemas.DataProviderTypes.DPR %>'
                        ValidationGroup="Add" />
                </InsertItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Connection string" SortExpression="ConnectionString">
                <InsertItemTemplate>
                    <cc1:SmartTextBox ID="SmartTextBox3" runat="server" Required="True" Text='<%# Bind("ConnectionString") %>'
                        Visible='<%# int.Parse(newDataProviderDropDownList.SelectedValue)== (int)CPRBroker.Schemas.DataProviderTypes.DPR %>'
                        ValidationGroup="Add" />
                </InsertItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="User name" SortExpression="UserName">
                <InsertItemTemplate>
                    <cc1:SmartTextBox ID="SmartTextBox4" runat="server" Required="True" Text='<%# Bind("UserName") %>'
                        Visible='<%# int.Parse(newDataProviderDropDownList.SelectedValue)== (int)CPRBroker.Schemas.DataProviderTypes.KMD %>'
                        ValidationGroup="Add" />
                </InsertItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Password" SortExpression="Password">
                <InsertItemTemplate>
                    <cc1:SmartTextBox ID="SmartTextBox5" runat="server" Required="True" Text='<%# Bind("Password") %>'
                        Visible='<%# int.Parse(newDataProviderDropDownList.SelectedValue)== (int)CPRBroker.Schemas.DataProviderTypes.KMD %>'
                        ValidationGroup="Add" />
                </InsertItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False" ControlStyle-CssClass="CommandButton">
                <InsertItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CommandName="Insert" Text="Insert"
                        ValidationGroup="Add"></asp:LinkButton>
                </InsertItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>
</asp:Content>
