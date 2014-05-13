<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfigPropertyGridEditor.ascx.cs"
    Inherits="CprBroker.Web.Controls.ConfigPropertyGridEditor" %>
<%@ Register Assembly="CprBroker.Web" Namespace="CprBroker.Web.Controls" TagPrefix="cc1" %>
<asp:GridView runat="server" ID="grd" AutoGenerateColumns="false" DataKeyNames="Name"
    ShowFooter="true" OnRowCommand="grd_RowCommand" >
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
                    Confidential='<%# Eval("Confidential") %>' ValidationGroup="Add" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
