using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CvrDemo.Data;
using System.Net;
using System.Xml;
using System.Xml.XPath;
using System.Web.Script.Serialization;
using System.Globalization;

namespace CvrDemo.Pages
{
    public partial class Search : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            using (var dataContext = new CvrDataContext())
            {
                LegalUnit unit;
                Dictionary<String, String> nameAddressMap = new Dictionary<String, String>();
                List<Position> namePositionMap = null;
                decimal cvrNum;
                if (Decimal.TryParse(CvrSearchBox.Text, out cvrNum))
                {
                    unit = dataContext.Units.Where(u => u.LegalUnitIdentifier == cvrNum && u.ProductionUnitIdentifier == 0).FirstOrDefault() as LegalUnit;
                    if (unit != null)
                    {
                        nameAddressMap.Add(unit.Name,
                            "vejnavn=" + HttpUtility.UrlEncode(unit.AddressOfficialStreetName) +
                            "&husnr=" + HttpUtility.UrlEncode(unit.AddressOfficialStreetBuildingIdentifier) +
                            "&postnr=" + unit.AddressOfficialPostCodeIdentifier
                            );
                        var prod = unit.ProductionUnits.Where(pU => pU != null);
                        foreach (var pU in prod)
                        {
                            object o = pU.Name;
                            if (!nameAddressMap.ContainsKey(pU.Name))
                            {
                                nameAddressMap.Add(pU.Name,
                                    "vejnavn=" + HttpUtility.UrlEncode(pU.AddressOfficialStreetName) +
                                    "&husnr=" + HttpUtility.UrlEncode(pU.AddressOfficialStreetBuildingIdentifier) +
                                    "&postnr=" + pU.AddressOfficialPostCodeIdentifier
                                    );
                            }
                        }

                        var owner = unit.Owners.Where(o => o != null);
                        foreach (var ow in owner)
                        {
                            object o = ow.Ajourføringsmarkering;
                        }
                    }
                    else
                    {
                        unit = new LegalUnit();
                        unit.Name = " CVR-nummer ukendt";
                        unit.TelephoneNumberIdentifier = "";
                    }
                }
                else
                {
                    unit = new LegalUnit();
                    unit.Name = " Forkert input!";
                    unit.TelephoneNumberIdentifier = "";
                }
                frmLegalUnit.DataSource = new LegalUnit[] { unit };
                frmLegalUnit.DataBind();
                namePositionMap = getPositions(nameAddressMap);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                //this.ClientScript.RegisterClientScriptBlock(this.GetType(), "addresses", serializer.Serialize(nameAddressMap), true);
                /*
                if (namePositionMap.Count > 1)
                {
                    decimal[,] bounds = findBounds(namePositionMap);
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Call setBounds function", "setBounds(" + serializer.Serialize(bounds) + ");", true);
                }
                 */
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Call addPosiotn function", "addPositionData(" + serializer.Serialize(namePositionMap) + ");", true);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Call map function", "showPositionsOnMap();", true);
            }
        }
        private List<Position> getPositions(Dictionary<String, String> nameAddressMap)
        {
            Dictionary<decimal, decimal> addedPositions = new Dictionary<decimal, decimal>();
            List<Position> positionList = new List<Position>();
            /*
             * We call geo.oiorest.dk for each address and retrieves geo coordinates for them.
             */
            foreach (KeyValuePair<String,String> current in nameAddressMap)
            {
                String addressUrl = "http://geo.oiorest.dk/adresser.xml?" + current.Value;
                try
                {
                    HttpWebRequest request = WebRequest.Create(addressUrl) as HttpWebRequest;
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    XmlReader xmlr = XmlReader.Create(response.GetResponseStream());
                    XmlDocument doc = new XmlDocument();
                    doc.Load(xmlr);
                    XmlNodeList adresser = doc.SelectNodes("/adgangsadresser/adgangsadresse");
                    decimal width = 0;
                    decimal length = 0;
                    foreach (XmlNode xn in adresser)
                    {
                        Decimal.TryParse(xn.SelectSingleNode("wgs84koordinat/bredde").InnerText, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out width);
                        Decimal.TryParse(xn.SelectSingleNode("wgs84koordinat/længde").InnerText, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out length);
                    }
                    if (width > 0 && length > 0)
                    {
                        if (!addedPositions.ContainsKey(width))
                        {
                            addedPositions.Add(width, length);
                            positionList.Add(new Position(current.Key, width, length));
                        }
                        else
                        {
                            decimal l = addedPositions[width];
                            if (l != length)
                                positionList.Add(new Position(current.Key, width, length));
                        }
                    }
                }
                catch (Exception e)
                {
                    //positionList.Add(new Position(current.Key, 0, 0));
                }
                addedPositions.Clear();
            }
            return positionList;
        }

        private decimal[,] findBounds(List<Position> positions)
        {
            decimal[,] bounds = new decimal[2,2];
            decimal lowestLat = 0, highestLat = 0, lowestLon = 0, highestLon = 0;
            foreach(Position pos in positions)
            {
                if (lowestLat == 0)
                    lowestLat = pos.width;
                else
                {
                    if (pos.width < lowestLat)
                        lowestLat = pos.width;
                }
                if (lowestLon == 0)
                    lowestLon = pos.length;
                else
                {
                    if (pos.length < lowestLon)
                        lowestLon = pos.length;
                }
                if (highestLat == 0)
                    highestLat = pos.width;
                else
                {
                    if (pos.width > highestLat)
                        highestLat = pos.width;
                }
                if (highestLon == 0)
                    highestLon = pos.length;
                else
                {
                    if (pos.length > highestLon)
                        highestLon = pos.length;
                }
            }
            bounds[0, 0] = lowestLat;
            bounds[0, 1] = lowestLon;
            bounds[1, 0] = highestLat;
            bounds[1, 1] = highestLon;
            return bounds;
        }

        public class Position
        {
            public String name;
            public decimal width;
            public decimal length;
            public Position(String n, decimal w, decimal l)
            {
                name = n;
                width = w;
                length = l;
            }
        }
    }
}