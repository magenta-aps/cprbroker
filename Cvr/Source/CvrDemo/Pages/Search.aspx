<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="CvrDemo.Pages.Search" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>CVR Demo - search</title>
    <style type="text/css">
        div.legal_unit_box {
            width: 50%;
            margin-left: auto;
            margin-right: auto;
            background-color: #efefff;
        }
        div.prod_unit_box {
            padding: 5pt;
            padding-left: 10%;
        }
        table.prod_unit_info {
            background-color: #eeeeee;
        }
        h2 {
            text-align: center;
        }
    </style>
</head>
    <body>
        <form id="SearchForm" runat="server">
            <div>
                <strong>CVR-nummer:</strong><asp:TextBox ID="CvrSearchBox" runat="server" Width="200px" /><asp:Button ID="SearchButton" Text="Søg" runat="server" OnClick="SearchButton_Click" />
            </div>
            <div>
                <asp:FormView
                    runat="server"
                    ID="frmLegalUnit"
                    width="100%">
                    <ItemTemplate>
                        <div class="legal_unit_box">
                            <h2><%# Eval("Name") %></h2>
                            <table>
                                <tr><td><strong>Selskabstype:</strong></td><td><%# Eval("BusinessFormatDescription") %></td></tr>
                                <tr><td><strong>Hovedbeskæftigelse:</strong></td><td><%# Eval("MainActivityActivityDescription") %></td></tr>
                                <tr><td valign="top"><strong>Adresse:</strong></td><td>
                                    <%# Eval("AddressOfficialStreetName") %> <%# Eval("AddressOfficialStreetBuildingIdentifier") %>, <%# Eval("AddressOfficialFloorIdentifier") %><%# Eval("AddressPostalSuiteIdentifier") %><br />
                                    <%# Eval("AddressOfficialPostCodeIdentifier") %> <%# Eval("AddressOfficialDistrictName") %>
                                </td></tr>
                                <tr><td><strong>Ejer:</strong></td><td><%# Eval("MainActivityActivityDescription") %></td></tr>
                            </table>
                            <h3>Produktionsenheder:</h3>
                            <asp:ListView ID="ListView1" runat="server" DataSource='<%# Eval("ProductionUnits") %>' >
                                <ItemTemplate>
                                    <div class="prod_unit_box">
                                        <table class="prod_unit_info">
                                            <tr><td><strong>Navn:</strong></td><td><%# Eval("Name") %></td></tr>
                                            <tr><td><strong>P-nummer:</strong></td><td><%# Eval("ProductionUnitIdentifier") %></td></tr>
                                            <tr><td><strong>Startdato:</strong></td><td><%# Eval("StartDate") %></td></tr>
                                            <tr><td><strong>Adresse:</strong></td><td>
                                                <%# Eval("AddressOfficialStreetName") %>
                                                <%# Eval("AddressOfficialFloorIdentifier") %>
                                                <%# Eval("AddressOfficialStreetBuildingIdentifier") %>
                                                                                 </td></tr>
                                        </table>
                                    </div>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                    </ItemTemplate>
                </asp:FormView>
            </div>
        </form>
    </body>
</html>
