<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="CvrDemo.Pages.Search" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>CVR Demo - search</title>
        <link rel="stylesheet" href="leaflet-0.6.4/leaflet.css" />
        <!--[if lte IE 8]>
            <link rel="stylesheet" href="leaflet-0.6.4/leaflet.css" />
        <![endif]-->
        <script src="leaflet-0.6.4/leaflet.js" type="text/javascript"></script>
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
                height: 300px;
			}
			table.legal_unit_info td{
				padding:.25em;
				vertical-align:top;
			}
			table.legal_unit_info td:first-child{
				text-align:right;
			}
            div.prod_unit_box {
            	width:33.333%;
                float:left;
			}
            table.prod_unit_info {
				width:100%;
				position:relative;
            }
			table.prod_unit_info tbody {
                background-color: #ccf;
				display:block;
				margin:.25em;
				height:16.625em;
				position:static;
			}
			table.prod_unit_info td {
				padding:.5em;
				vertical-align:top;
				position:relative;
				z-index:10;
			}
			table.prod_unit_info td:first-child {
				width:6em;
				text-align:right;
			}
            h2 {
				margin:0 auto 1em;
            }
            #map {
                height: 100%;
                width: 100%;
            }
            div.legat_unit_map_wrapper {
                height: 300px;
            }
            div.legal_unit_info_table {
            	width:33.333%;
                float:left;
            }
            div.map_container {
            	width:66.666%;
                height: 100%;
                float:left;
            }
        </style>
    </head>
    <body>
        <form id="SearchForm" runat="server">
            <input type="hidden" id="nameAddressMap" />
            <input type="hidden" id="namePositionMap" />
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
                        <div style="visibility: <%# Eval("Name").ToString().Substring(0,1).Equals(" ") ? "hidden" : "visible" %>">
                            <div class="legat_unit_map_wrapper">
                                <div class="legal_unit_info_table">
                                    <table class="legal_unit_info">
                                        <tr><td><strong>Selskabstype:</strong></td><td><%# Eval("BusinessFormatDescription") %></td></tr>
                                        <tr><td><strong>Hovedbeskæftigelse:</strong></td><td><%# Eval("MainActivityActivityDescription") %></td></tr>
                                        <tr><td valign="top"><strong>Adresse:</strong></td><td>
                                            <%# Eval("AddressOfficialStreetName") %> <%# Eval("AddressOfficialStreetBuildingIdentifier") %>, <%# Eval("AddressOfficialFloorIdentifier") %><%# Eval("AddressPostalSuiteIdentifier") %><br />
                                            <%# Eval("AddressOfficialPostCodeIdentifier") %> <%# Eval("AddressOfficialDistrictName") %><br />
                                        </td></tr>
                                        <tr><td><strong>Telefon:</strong></td><td><%# Eval("TelephoneNumberIdentifier").ToString().Equals("") ? "Ukendt" : Eval("TelephoneNumberIdentifier") %></td></tr>
                                    </table>
                                </div>
                                <div class="map_container">
                                    <div id="map"></div>
                                </div>
                            </div>
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
                                                </td></tr>
                                                <tr><td><strong>Telefon:</strong></td><td><%# Eval("TelephoneNumberIdentifier").ToString().Equals("") ? "Ukendt" : Eval("TelephoneNumberIdentifier") %></td></tr>
                                            </table>
                                        </div>
                                    </ItemTemplate>
                                </asp:ListView>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:FormView>
            <script type="text/javascript">
                var positions;
                function addPositionData(data) {
                    positions = data;
                }
                function showPositionsOnMap() {
                    var boundsArray = [];
                    var namePositionMap = positions;
                    if (namePositionMap.length >= 2) {
                        for (var i = 0; i < namePositionMap.length; i++) {
                            boundsArray.push([namePositionMap[i].width, namePositionMap[i].length]);
                        }
                        if (boundsArray.length > 1)
                            var map = L.map('map').fitBounds(boundsArray);
                        else
                            var map = L.map('map', { center: [boundsArray[0].width, boundsArray[0].length], zoom: 18 });
                    } else
                        var map = L.map('map', { center: [namePositionMap[0].width, namePositionMap[0].length], zoom: 18 });
                    L.tileLayer('http://{s}.tile.osm.org/{z}/{x}/{y}.png', {
                        maxZoom: 18,
                        attribution: 'Map data &copy; <a href="http://openstreetmap.org">OpenStreetMap</a> contributors, <a href="http://creativecommons.org/licenses/by-sa/2.0/">CC-BY-SA</a>, Imagery © <a href="http://cloudmade.com">CloudMade</a>'
                    }).addTo(map);

                    var markers = new Array();
                    for (var i = 0; i < namePositionMap.length; i++) {
                        markers.push(L.marker([namePositionMap[i].width, namePositionMap[i].length]).addTo(map));
                        markers[i].bindPopup("<b>" + namePositionMap[i].name + "</b><br>(" + namePositionMap[i].width + ", " + namePositionMap[i].length + ".");
                    }
                }
            </script>
        </form>
    </body>
</html>
