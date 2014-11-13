using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices
{
    public class SearchMethodCall
    {
        public string Name;
        public List<KeyValuePair<string, string>> InputFields = new List<KeyValuePair<string, string>>();

        public SearchMethodCall()
        {

        }

        public SearchMethodCall(SearchMethod template, SearchRequest request)
        {
            Name = template.Name;

            InputFields.AddRange(
                from tf in template.InputFields
                from rf in request.CriteriaFields
                where tf.Name == rf.Key
                select rf
                );
        }

        public string ToRequestXml(string template)
        {
            var doc = new XmlDocument();
            doc.LoadXml(template);
            doc.SelectSingleNode("//Service").Attributes["r"].Value = Name;
            doc.SelectSingleNode("//CprServiceHeader").Attributes["r"].Value = Name;

            var keyNode = doc.SelectSingleNode("//Key");
            foreach (var f in InputFields)
            {
                var fieldNode = keyNode.OwnerDocument.CreateNode(XmlNodeType.Element, "", "Field", "");
                keyNode.AppendChild(fieldNode);

                var rAttr = keyNode.OwnerDocument.CreateAttribute("r");
                rAttr.Value = f.Key;
                fieldNode.Attributes.Append(rAttr);

                var vAttr = keyNode.OwnerDocument.CreateAttribute("v");
                vAttr.Value = f.Value;
                fieldNode.Attributes.Append(vAttr);
            }

            return doc.OuterXml;
        }

        public List<SearchPerson> ParseResponse(string responseXml, bool multi)
        {
            var doc = new XmlDocument();
            doc.LoadXml(responseXml);

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("c", Constants.XmlNamespace);

            string path;
            if (multi)// search method
                path = "//c:Rolle[@r='HovedRolle']/c:Table/c:Row[not(@u)]";
            else// lookup method
                path = "//c:Rolle[c:Field][@r='HovedRolle']";

            var elements = doc.SelectNodes(path, nsmgr).OfType<XmlElement>().ToArray();

            var ret = new List<SearchPerson>();
            foreach (var elm in elements)
            {
                var p = new SearchPerson();
                var name = GetFieldValue(elm, "CNVN_ADRNVN");
                if (!string.IsNullOrEmpty(name))
                    p.Name = NavnStrukturType.Create(name);
                p.PNR = GetFieldValue(elm, "PNR");

                #region Address
                var kom = GetFieldValue(elm, "KOMKOD");
                if (!string.IsNullOrEmpty(kom))
                {
                    var komK = decimal.Parse(kom);

                    p.Address = new AdresseType();

                    if (komK >= CprBroker.Schemas.Part.AddressConstants.GreenlandMunicipalCodeStart)
                    {
                        p.Address.Item = new GroenlandAdresseType()
                        {
                            AddressCompleteGreenland = new AddressCompleteGreenlandType()
                            {
                                CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkCountryCode.ToString()),
                                MunicipalityCode = kom,
                                StreetCode = GetFieldValue(elm, "VEJKOD"),

                                StreetBuildingIdentifier = GetFieldValue(elm, "HUSNR"),
                                GreenlandBuildingIdentifier = GetFieldValue(elm, "BNR"),
                                FloorIdentifier = GetFieldValue(elm, "ETAGE"),
                                SuiteIdentifier = GetFieldValue(elm, "SIDEDOER"),

                                // TODO: Lookup street name
                                StreetName = null,
                                StreetNameForAddressingName = null,

                                // TODO: Lookup post code and district
                                PostCodeIdentifier = null,
                                DistrictName = null,

                                DistrictSubdivisionIdentifier = null,
                                MailDeliverySublocationIdentifier = null,
                            },
                            SpecielVejkodeIndikator = true,
                            SpecielVejkodeIndikatorSpecified = true,
                            NoteTekst = null,
                            UkendtAdresseIndikator = false
                        };
                    }
                    else
                    {
                        // Danish
                        p.Address.Item = new DanskAdresseType()
                        {
                            AddressComplete = new AddressCompleteType()
                            {
                                AddressAccess = new AddressAccessType()
                                {
                                    MunicipalityCode = kom,
                                    StreetCode = GetFieldValue(elm, "VEJKOD"),
                                    StreetBuildingIdentifier = GetFieldValue(elm, "HUSNR")
                                },
                                AddressPostal = new AddressPostalType()
                                {
                                    CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkCountryCode.ToString()),

                                    FloorIdentifier = GetFieldValue(elm, "ETAGE"),
                                    StreetBuildingIdentifier = GetFieldValue(elm, "HUSNR"),
                                    SuiteIdentifier = GetFieldValue(elm, "SIDEDOER"),

                                    // TODO: Lookup
                                    PostCodeIdentifier = null,
                                    DistrictName = null,

                                    // TODO: lookup street name and addressing name
                                    StreetName = null,
                                    StreetNameForAddressingName = null,

                                    // Not implemented
                                    DistrictSubdivisionIdentifier = null,
                                    MailDeliverySublocationIdentifier = null,
                                    PostOfficeBoxIdentifier = null,
                                }
                            },
                            // TODO: Fill post district
                            PostDistriktTekst = null,
                            SpecielVejkodeIndikator = false,
                            SpecielVejkodeIndikatorSpecified = true,
                            UkendtAdresseIndikator = false,

                            // Not implemented
                            AddressPoint = null,
                            NoteTekst = null,
                            PolitiDistriktTekst = null,
                            SkoleDistriktTekst = null,
                            SogneDistriktTekst = null,
                            SocialDistriktTekst = null,
                            ValgkredsDistriktTekst = null,
                        };
                    }
                }
                else
                {
                    var streetAddress = GetFieldValue(elm, "STADR");
                    if (!string.IsNullOrEmpty(streetAddress))
                    {
                        // Parse the strings
                        var arr = streetAddress.Split(' ');
                        string streetName, houseNr;
                        if (arr.Length > 1)
                        {
                            streetName = string.Join(" ", arr.Take(arr.Length - 1).ToArray());
                            houseNr = arr.Last();
                        }
                        else
                        {
                            streetName = streetAddress;
                            houseNr = null;
                        }
                        var postCode = GetFieldValue(elm, "POSTNR");
                        var postDist = GetFieldText(elm, "POSTNR");

                        // Fill the object
                        p.Address = new AdresseType()
                        {
                            Item = new DanskAdresseType()
                            {
                                AddressComplete = new AddressCompleteType()
                                {
                                    // TODO: Can we fill Address access by lookup?
                                    AddressAccess = null,
                                    AddressPostal = new AddressPostalType()
                                    {
                                        CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkCountryCode.ToString()),
                                        StreetName = streetName,
                                        StreetBuildingIdentifier = houseNr,
                                        StreetNameForAddressingName = streetName,
                                        // TODO: See if we can get floor & suite
                                        FloorIdentifier = null,
                                        SuiteIdentifier = null,
                                        DistrictName = postDist,
                                        PostCodeIdentifier = postCode,

                                        // Not implemented
                                        MailDeliverySublocationIdentifier = null,
                                        PostOfficeBoxIdentifier = null,
                                        DistrictSubdivisionIdentifier = null,
                                    }
                                },
                                PostDistriktTekst = postDist,
                                SpecielVejkodeIndikator = false,
                                SpecielVejkodeIndikatorSpecified = false,
                                UkendtAdresseIndikator = false,

                                // Not implemented
                                NoteTekst = null,
                                AddressPoint = null,
                                PolitiDistriktTekst = null,
                                SkoleDistriktTekst = null,
                                SocialDistriktTekst = null,
                                SogneDistriktTekst = null,
                                ValgkredsDistriktTekst = null
                            }
                        };
                        //TODO" Parse address here

                    }
                    else
                    {
                        // TODO: Fill Foreign address
                    }
                }
                #endregion

                ret.Add(p);
            }
            return ret;
        }

        public string GetFieldValue(XmlElement elm, string name)
        {
            return GetFieldAttributeValue(elm, name, "v");
        }

        public string GetFieldText(XmlElement elm, string name)
        {
            return GetFieldAttributeValue(elm, name, "t");
        }

        public string GetFieldAttributeValue(XmlElement elm, string name, string attributeName)
        {
            var nsmgr = new XmlNamespaceManager(elm.OwnerDocument.NameTable);
            nsmgr.AddNamespace("c", Constants.XmlNamespace);

            var ndPath = string.Format("c:Field[@r='{0}']", name);
            var nd = elm.SelectSingleNode(ndPath, nsmgr);
            if (nd != null && nd.Attributes[attributeName] != null)
                return nd.Attributes[attributeName].Value.Trim();
            return null;
        }

    }
}
