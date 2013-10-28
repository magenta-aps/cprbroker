<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Applications.aspx.cs" Inherits="CprBroker.Web.Pages.Applications"
    MasterPageFile="~/Pages/Site.Master" Title="Applications" %>

<%@ MasterType VirtualPath="~/Pages/Site.Master" %>
<%@ Register Assembly="CprBroker.Web" Namespace="CprBroker.Web.Controls" TagPrefix="cpr" %>
<asp:Content runat="server" ContentPlaceHolderID="Contents">
    <h3>
        Admin application</h3>
    This is the root application that is pre-created in the system.It cannot be edited
    or deleted.
    <asp:LinqDataSource ID="baseApplicationLinqDataSource" runat="server" ContextTypeName="CprBroker.Data.Applications.ApplicationDataContext"
        TableName="Applications" OnSelecting="baseApplicationLinqDataSource_Selecting"
        EnableUpdate="True" Where="ApplicationId == @ApplicationId">
        <WhereParameters>
            <asp:Parameter DbType="Guid" Name="ApplicationId" />
        </WhereParameters>
    </asp:LinqDataSource>
    <asp:DetailsView ID="baseApplicationDetailsView" runat="server" AutoGenerateRows="False"
        DataKeyNames="ApplicationId" DataSourceID="baseApplicationLinqDataSource" Height="50px"
        Width="125px" OnItemUpdated="baseApplicationDetailsView_ItemUpdated" RowStyle-Wrap="false">
        <Fields>
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ReadOnly="true" />
            <asp:BoundField DataField="Token" HeaderText="Token" SortExpression="Token" ReadOnly="true" />
            <asp:BoundField DataField="RegistrationDate" HeaderText="Registration date" ReadOnly="True"
                SortExpression="RegistrationDate">
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
            <asp:BoundField DataField="ApprovedDate" HeaderText="Approved date" ReadOnly="True"
                SortExpression="ApprovedDate">
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:BoundField>
        </Fields>
    </asp:DetailsView>
    <h3>
        Applications</h3>
    These are new applications that can be used to access the system. You need a working
    application token to access the system's web services.
    <asp:LinqDataSource ID="applicationsLinqDataSource" runat="server" ContextTypeName="CprBroker.Data.Applications.ApplicationDataContext"
        TableName="Applications" EnableUpdate="True" OrderBy="RegistrationDate" OnSelecting="applicationsLinqDataSource_Selecting"
        Where="ApplicationId != @ApplicationId" OnUpdating="applicationsLinqDataSource_Updating"
        EnableInsert="True" EnableDelete="true" OnInserting="applicationsLinqDataSource_Inserting">
        <WhereParameters>
            <asp:Parameter DbType="Guid" Name="ApplicationId" />
        </WhereParameters>
    </asp:LinqDataSource>
    <asp:GridView ID="applicationsGridView" runat="server" AutoGenerateColumns="False"
        DataKeyNames="ApplicationId" DataSourceID="applicationsLinqDataSource" OnRowUpdated="applicationsGridView_RowUpdated"
        OnRowDeleted="applicationsGridView_RowDeleted">
        <Columns>
            <asp:BoundField DataField="ApplicationId" HeaderText="ApplicationId" ReadOnly="True"
                SortExpression="ApplicationId" Visible="False" />
            <asp:TemplateField HeaderText="Name" SortExpression="Name">
                <EditItemTemplate>
                    <cpr:SmartTextBox ID="TextBox1" runat="server" Text='<%# Bind("Name") %>' Required="True">
                    </cpr:SmartTextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Token" SortExpression="Token">
                <EditItemTemplate>
                    <cpr:SmartTextBox ID="TextBox2" runat="server" Text='<%# Bind("Token") %>' Required="True">
                    </cpr:SmartTextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label2" runat="server" Text='<%# Bind("Token") %>'></asp:Label>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
                <HeaderStyle Wrap="False" />
                <ItemStyle Wrap="False" />
            </asp:TemplateField>
            <asp:BoundField DataField="RegistrationDate" HeaderText="Registration date" SortExpression="RegistrationDate"
                ReadOnly="True" />
            <asp:CheckBoxField DataField="IsApproved" HeaderText="Approved" SortExpression="IsApproved" />
            <asp:BoundField DataField="ApprovedDate" HeaderText="Approved date" ReadOnly="True"
                SortExpression="ApprovedDate" />
            <asp:CommandField ShowEditButton="True" ControlStyle-CssClass="CommandButton" />
            <asp:CommandField ShowDeleteButton="True" ControlStyle-CssClass="CommandButton" />
        </Columns>
    </asp:GridView>
    <h3>
        New application</h3>
    <asp:DetailsView ID="newApplicationDetailsView" runat="server" AutoGenerateRows="False"
        DataKeyNames="ApplicationId" DataSourceID="applicationsLinqDataSource" DefaultMode="Insert"        >
        <Fields>
            <asp:TemplateField HeaderText="Name" SortExpression="Name">
                <InsertItemTemplate>
                    <cpr:SmartTextBox ID="TextBox1" runat="server" Text='<%# Bind("Name") %>' Required="True"
                        ValidationGroup="Insert"></cpr:SmartTextBox>
                </InsertItemTemplate>
            </asp:TemplateField>
            <asp:CheckBoxField DataField="IsApproved" HeaderText="Approved" SortExpression="IsApproved" />
            <asp:TemplateField ShowHeader="False" ControlStyle-CssClass="CommandButton">
                <InsertItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Insert"
                        Text="Insert" ValidationGroup="Insert"></asp:LinkButton>
                </InsertItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>
</asp:Content>
