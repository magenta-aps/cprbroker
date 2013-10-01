<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Extracts.aspx.cs" Inherits="CvrDemo.Pages.Extracts" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>CVR Demo - Extracts</title>
        <style type="text/css">
            div.panel_container {
                float: left;
                position: relative;
                left: 50%;
            }
            div.panel_box {
                padding: 5pt;
                background-color: #eeeeef;
                text-align: center;
                float: left;
                position: relative;
                left: -50%;
            }
            div.status_container {
                clear: both;
                float: left;
                position: relative;
                left: 50%;
            }
            div.status_box {
                padding: 5pt;
                background-color: #efefff;
                float: left;
                position: relative;
                top: 50pt;
                float: left;
                position: relative;
                left: -50%;
            }
        </style>
    </head>
    <body>
        <form id="ExtractsForm" runat="server">
            <div class="panel_container">
                <div class="panel_box">
                    <asp:Button Text="Opdatér" ID="UpdateButton" runat="server" OnClick="RefreshButton_Click" />
                </div>
            </div>
            <div>
                <asp:FormView
                    runat="server"
                    ID="frmUpdateDatabase"
                    width="100%">
                    <ItemTemplate>
                        <div>
                            <asp:Label ID="StatusText" runat="server"></asp:Label>
                        </div>
                    </ItemTemplate>
                </asp:FormView>
            </div>
        </form>
    </body>
</html>
