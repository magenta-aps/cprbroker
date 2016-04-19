<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfigPropertyEditor.ascx.cs"
    Inherits="CprBroker.Web.Controls.ConfigPropertyEditor" %>
<%@ Register Assembly="CprBroker.Web" Namespace="CprBroker.Web.Controls" TagPrefix="cc1" %>
<asp:DataList ID="EditDataList" runat="server" RepeatColumns="3" DataKeyField="Name" RepeatDirection="Horizontal">
    <ItemTemplate>
        <b>
            <%# Eval("Name")%>:</b>
        <cc1:SmartTextBox ID="SmartTextBox" runat="server" Type='<%# Eval("Type") %>' EnumType='<%# Eval("EnumType") %>' Text='<%# Bind("Value") %>'
            Required='<%# Bind("Required") %>' Confidential='<%# Bind("Confidential") %>' />
    </ItemTemplate>
</asp:DataList>