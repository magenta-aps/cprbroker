<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dbr.aspx.cs" 
    Inherits="CprBroker.Web.Pages.Dbr"  MasterPageFile="~/Pages/Site.Master"%>

<%@ MasterType VirtualPath="~/Pages/Site.Master" %>
<%@ Register Assembly="CprBroker.Web" Namespace="CprBroker.Web.Controls" TagPrefix="cc1" %>
<%@ Import Namespace="CprBroker.Engine" %>
<asp:content id="Content1" runat="server" contentplaceholderid="Contents">
    <h3>DBR</h3>    
    <h4>Sync targets</h4>
    <asp:GridView runat="server" ID="grdDbr" OnDataBinding="grdDbr_DataBinding">    
    <EmptyDataTemplate>(None)</EmptyDataTemplate>
    <Columns>
        <asp:TemplateField HeaderText="Details">
                <ItemTemplate>
                    <asp:Repeater ID="DataList1" runat="server" DataSource='<%# (Container.DataItem as IHasConfigurationProperties).ToDisplayableProperties() %>'>
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
                    <asp:DataList ID="EditDataList" runat="server" DataSource='<%# (Container.DataItem as IHasConfigurationProperties).ToDisplayableProperties() %>' RepeatColumns="3"
                        DataKeyField="Name" RepeatDirection="Horizontal">
                        <ItemTemplate>
                            <b>
                                <%# Eval("Name")%>:</b>
                            <cc1:SmartTextBox ID="SmartTextBox" runat="server" Type='<%# Eval("Type") %>' Text='<%# Bind("Value") %>' Required='<%# Bind("Required") %>' Confidential='<%# Bind("Confidential") %>' />
                        </ItemTemplate>
                    </asp:DataList>
                </EditItemTemplate>
            </asp:TemplateField>
    </Columns>
    </asp:GridView>

    <h4>New sync target</h4>
    <asp:GridView runat="server" ID="newDbr" AutoGenerateColumns="false"
        DataKeyNames="Name" ShowFooter="true" OnRowCommand="newDataProviderGridView_RowCommand"
        OnDataBinding="newDbr_DataBinding">
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
</asp:content>
