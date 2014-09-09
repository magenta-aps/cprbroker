<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dbr.aspx.cs" Inherits="CprBroker.Web.Pages.Dbr"
    MasterPageFile="~/Pages/Site.Master" %>

<%@ MasterType VirtualPath="~/Pages/Site.Master" %>
<%@ Register Assembly="CprBroker.Web" Namespace="CprBroker.Web.Controls" TagPrefix="cc1" %>
<%@ Register Src="~/Pages/Controls/ConfigPropertyViewer.ascx" TagPrefix="uc1" TagName="ConfigPropertyViewer" %>
<%@ Register Src="~/Pages/Controls/ConfigPropertyEditor.ascx" TagPrefix="uc1" TagName="ConfigPropertyEditor" %>
<%@ Register Src="~/Pages/Controls/ConfigPropertyGridEditor.ascx" TagPrefix="uc1"
    TagName="ConfigPropertyGridEditor" %>
<%@ Import Namespace="CprBroker.Engine" %>
<%@ Import Namespace="CprBroker.Data" %>
<%@ Import Namespace="CprBroker.Providers.CPRDirect" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Contents">
    <h3>
        DBR</h3>
    <h4>
        Sync targets</h4>
    <asp:LinqDataSource runat="server" ID="dsDbr" ContextTypeName="CprBroker.Data.Queues.QueueDataContext" OnSelecting="dsDbr_Selecting"
        TableName="Queues" Where="TypeId==@TypeId" EnableDelete="true" EnableUpdate="true">
        <WhereParameters>
            <asp:Parameter Name="TypeId" Type="Int32"/>
        </WhereParameters>
    </asp:LinqDataSource>
    <asp:GridView runat="server" ID="grdDbr" AutoGenerateColumns="false" OnRowCommand="grdDbr_RowCommand"        
        DataKeyNames="QueueId" DataSourceID="dsDbr">
        <EmptyDataTemplate>
            (None)</EmptyDataTemplate>
        <Columns>
            <cc1:ConfigPropertyField HeaderText="Details" OnObjectCreating="grdDbrPropertiesField_ObjectCreating" />
            <cc1:SmartTextField DataField="BatchSize" Type="Integer" Required="true" HeaderText="Batch Size" />
            <cc1:SmartTextField DataField="MaxRetry" Type="Integer" Required="true" HeaderText="Max retry" />            
            <asp:TemplateField ShowHeader="False" ControlStyle-CssClass="CommandButton">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton2" runat="server" CommandArgument='<%# Eval("QueueId") %>'
                        CommandName="Ping">Ping</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:CommandField ShowEditButton="True" ControlStyle-CssClass="CommandButton" />
            <asp:CommandField ShowDeleteButton="True" ControlStyle-CssClass="CommandButton" />
        </Columns>
    </asp:GridView>
    <h4>
        New sync target</h4>
    <uc1:ConfigPropertyGridEditor runat="server" ID="newDbr" OnDataBinding="newDbr_DataBinding"
        OnInsertCommand="newDbr_InsertCommand" />
</asp:Content>
