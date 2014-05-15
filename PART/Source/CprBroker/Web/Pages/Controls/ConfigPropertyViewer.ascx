<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfigPropertyViewer.ascx.cs"
    Inherits="CprBroker.Web.Controls.ConfigPropertyViewer" %>
<asp:Repeater ID="DataList1" runat="server" >
    <ItemTemplate>
        <b>
            <%# Eval("Name")%>:</b>
        <%# (bool)Eval("Confidential")?"********": Eval("Value")%>
    </ItemTemplate>
    <SeparatorTemplate>
        &nbsp;</SeparatorTemplate>
</asp:Repeater>