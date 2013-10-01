<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="CvrDemo.Pages.Search" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>CVR Demo - search</title>
        <style type="text/css">
			body {
                margin: 0;
                padding: 0;
                font-family: Helvetica, Arial, sans-serif;
			}
            div.search_box {
                padding: 1em;
                background-color: #dde;
                text-align: center;
                width:85%;
				margin:0 auto;
            }
			div.result_wrapper {
				margin:-.25em;
				overflow:hidden;
			}
            div.legal_unit_box {
                width:85%;
				margin:0 auto;
                background-color: #eef;
				padding:1em;
            }
			table.legal_unit_info {
				margin-left:-.25em;
			}
			table.legal_unit_info td{
				padding:.25em;
				vertical-align:top;
			}
			table.legal_unit_info td:first-child{
				text-align:right;
			}
            div.prod_unit_box {
            	width:33.333%;float:left;
			}
            table.prod_unit_info {
				width:100%;
				position:relative;
            }
			table.prod_unit_info tbody {
                background-color: #ccf;
				display:block;
				margin:.25em;
				height:13.625em;
				position:static;
			}
			table.prod_unit_info td {
				padding:.5em;
				vertical-align:top;
				position:relative;
				z-index:10;
			}
			table.prod_unit_info td:first-child {
				width:5.25em;
				text-align:right;
			}
            h2 {
				margin:0 auto 1em;
            }
        </style>
    </head>
    <body>
        <form id="SearchForm" runat="server">
            <div class="search_box">
                <strong>CVR-nummer:</strong><asp:TextBox ID="CvrSearchBox" runat="server" Width="200px" /><asp:Button ID="SearchButton" Text="Søg" runat="server" OnClick="SearchButton_Click" />
            </div>
            <asp:FormView
                runat="server"
                ID="frmLegalUnit"
                RenderOuterTable="false">
                <ItemTemplate>
                    <div class="legal_unit_box">
                        <h2><%# Eval("Name") %></h2>
                        <table class="legal_unit_info">
                            <tr><td><strong>Selskabstype:</strong></td><td><%# Eval("BusinessFormatDescription") %></td></tr>
                            <tr><td><strong>Hovedbeskæftigelse:</strong></td><td><%# Eval("MainActivityActivityDescription") %></td></tr>
                            <tr><td valign="top"><strong>Adresse:</strong></td><td>
                                <%# Eval("AddressOfficialStreetName") %> <%# Eval("AddressOfficialStreetBuildingIdentifier") %>, <%# Eval("AddressOfficialFloorIdentifier") %><%# Eval("AddressPostalSuiteIdentifier") %><br />
                                <%# Eval("AddressOfficialPostCodeIdentifier") %> <%# Eval("AddressOfficialDistrictName") %><br />
                                <a
                                    href="http://maps.google.com/?q=<%# Eval("AddressOfficialStreetName") %> <%# Eval("AddressOfficialStreetBuildingIdentifier") %>, <%# Eval("AddressOfficialFloorIdentifier") %><%# Eval("AddressPostalSuiteIdentifier") %>, <%# Eval("AddressOfficialPostCodeIdentifier") %> <%# Eval("AddressOfficialDistrictName") %>"
                                    target="_blank">Se på kort</a>
                            </td></tr>
                        </table>
                        <h3>Produktionsenheder:</h3>
                        <div class="result_wrapper">
                            <asp:ListView ID="ProductionUnitsList" runat="server" DataSource='<%# Eval("ActualProductionUnits") %>' >
                                <ItemTemplate>
                                    <div class="prod_unit_box">
                                        <table class="prod_unit_info">
                                            <tr><td><strong>Navn:</strong></td><td><%# Eval("Name") %></td></tr>
                                            <tr><td><strong>P-nummer:</strong></td><td><%# Eval("ProductionUnitIdentifier") %></td></tr>
                                            <tr><td><strong>Startdato:</strong></td><td><%# Eval("StartDate") %></td></tr>
                                            <tr><td><strong>Adresse:</strong></td><td>
                                                <%# Eval("AddressOfficialStreetName") %> <%# Eval("AddressOfficialStreetBuildingIdentifier") %>, <%# Eval("AddressOfficialFloorIdentifier") %><%# Eval("AddressPostalSuiteIdentifier") %><br />
                                                <%# Eval("AddressOfficialPostCodeIdentifier") %> <%# Eval("AddressOfficialDistrictName") %><br />
                                                <a
                                                    href="http://maps.google.com/?q=<%# Eval("AddressOfficialStreetName") %> <%# Eval("AddressOfficialStreetBuildingIdentifier") %>, <%# Eval("AddressOfficialFloorIdentifier") %><%# Eval("AddressPostalSuiteIdentifier") %>, <%# Eval("AddressOfficialPostCodeIdentifier") %> <%# Eval("AddressOfficialDistrictName") %>"
                                                    target="_blank">Se på kort</a>
                                            </td></tr>
                                        </table>
                                    </div>
                                </ItemTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:FormView>
        </form>
    </body>
</html>
