<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CPRNotificationServiceDemo.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label Text="Enter CPR to subscribe on:" runat="server"/>
        <asp:TextBox ID="CPRTextBox"  runat="server"/>
        <asp:Button ID="SubscribeButton" OnClick="SubscribeButton_OnClick" runat="server"  Text="Subscribe"/>
        <asp:Button ID="ShowNotificationsButton" OnClick="ShowNotificationsButton_OnClick" runat="server" Text="Show notifications" />
    </div>
    </form>
</body>
</html>
