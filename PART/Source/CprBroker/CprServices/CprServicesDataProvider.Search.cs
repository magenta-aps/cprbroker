using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices
{
    public partial class CprServicesDataProvider
    {
        public string[] CallADRSOG1(string token, SoegAttributListeType attributes)
        {
            var inp = Properties.Resources.ADRSOG1;
            var doc = new XmlDocument();
            doc.LoadXml(inp);

            AddSearchCriteria(doc, attributes);
            var resp = "";
            var kvit = Send(doc.OuterXml, ref token, out resp);
            return null;
        }

        public void AddSearchCriteria(XmlDocument doc, SoegAttributListeType attributes)
        {
            var keyNode = InitKeyNode(doc);

            // Add max records node - fixed by definition
            AddKeyField(keyNode, "MAXA", "20");

            if (attributes.SoegEgenskab != null)
            {
                foreach (var egen in attributes.SoegEgenskab.Where(e => e != null))
                {
                    if (egen.NavnStruktur != null)
                    {
                        // Names
                        List<string> names = new List<string>();
                        if (!string.IsNullOrEmpty(egen.NavnStruktur.PersonNameForAddressingName))
                        {
                            names.Add(egen.NavnStruktur.PersonNameForAddressingName);
                        }
                        if (egen.NavnStruktur.PersonNameStructure != null && egen.NavnStruktur.PersonNameStructure.IsEmpty)
                        {
                            if (!string.IsNullOrEmpty(egen.NavnStruktur.PersonNameStructure.PersonGivenName))
                            {
                                names.Add(egen.NavnStruktur.PersonNameStructure.PersonGivenName);
                            }
                            if (!string.IsNullOrEmpty(egen.NavnStruktur.PersonNameStructure.PersonMiddleName))
                            {
                                names.Add(egen.NavnStruktur.PersonNameStructure.PersonMiddleName);
                            }
                            if (!string.IsNullOrEmpty(egen.NavnStruktur.PersonNameStructure.PersonSurnameName))
                            {
                                names.Add(egen.NavnStruktur.PersonNameStructure.PersonSurnameName);
                            }
                        }
                        for (int i = 0; i < names.Count; i++)
                        {
                            var propName = "NVN";
                            if (i > 0)
                                propName += i;
                            AddKeyField(keyNode, propName, names[i]);
                        }
                        //egen.NavnStruktur.KaldenavnTekst
                        //egen.NavnStruktur.NoteTekst
                    }

                    // Gender
                    if (egen.PersonGenderCodeSpecified)
                    {
                        string gender = null;
                        switch (egen.PersonGenderCode)
                        {
                            case PersonGenderCodeType.female:
                                gender = "K";
                                break;
                            case PersonGenderCodeType.male:
                                gender = "M";
                                break;
                        }
                        AddKeyField(keyNode, "KON", gender);
                    }
                    // TODO: Unsupported
                    //egen.AndreAdresser
                    //egen.BirthDate 
                    //egen.FoedestedNavn
                    //egen.FoedselsregistreringMyndighedNavn
                    //egen.KontaktKanal                    
                    //egen.NaermestePaaroerende
                    //egen.SoegVirkning                    
                }
            }

            if (attributes.SoegRegisterOplysning != null)
            {
                foreach (var oplys in attributes.SoegRegisterOplysning.Where(r => r != null))
                {
                    if (oplys.Item is CprBorgerType)
                    {
                        var cpr = oplys.Item as CprBorgerType;
                        //cpr.AdresseNoteTekst
                        //cpr.FolkekirkeMedlemIndikator
                        if (cpr.FolkeregisterAdresse != null)
                        {
                            if (cpr.FolkeregisterAdresse.Item is DanskAdresseType)
                            {
                                var dansk = cpr.FolkeregisterAdresse.Item as DanskAdresseType;
                                if (dansk.AddressComplete != null)
                                {
                                    if (dansk.AddressComplete.AddressAccess != null)
                                    {
                                        AddKeyField(keyNode, "KOMK", dansk.AddressComplete.AddressAccess.MunicipalityCode);
                                        AddKeyField(keyNode, "VEJK", dansk.AddressComplete.AddressAccess.StreetBuildingIdentifier);
                                        AddKeyField(keyNode, "VEJK", dansk.AddressComplete.AddressAccess.StreetCode);
                                    }
                                    if (dansk.AddressComplete.AddressPostal != null)
                                    {
                                        //dansk.AddressComplete.AddressPostal.CountryIdentificationCode
                                        //dansk.AddressComplete.AddressPostal.DistrictName
                                        //dansk.AddressComplete.AddressPostal.DistrictSubdivisionIdentifier
                                        AddKeyField(keyNode, "ETAG", dansk.AddressComplete.AddressPostal.FloorIdentifier);
                                        //dansk.AddressComplete.AddressPostal.MailDeliverySublocationIdentifier
                                        AddKeyField(keyNode, "POST", dansk.AddressComplete.AddressPostal.PostCodeIdentifier);
                                        //dansk.AddressComplete.AddressPostal.PostOfficeBoxIdentifier
                                        AddKeyField(keyNode, "HNR", dansk.AddressComplete.AddressPostal.StreetBuildingIdentifier);
                                        AddKeyField(keyNode, "VNVN", dansk.AddressComplete.AddressPostal.StreetName);
                                        // TODO: is it OK to use this?
                                        AddKeyField(keyNode, "VNVN", dansk.AddressComplete.AddressPostal.StreetNameForAddressingName);
                                        AddKeyField(keyNode, "SIDO", dansk.AddressComplete.AddressPostal.SuiteIdentifier);
                                    }
                                }
                                //dansk.AddressPoint
                                //dansk.NoteTekst
                                //dansk.PolitiDistriktTekst
                                //dansk.PostDistriktTekst
                                //dansk.SkoleDistriktTekst
                                //dansk.SocialDistriktTekst
                                //dansk.SogneDistriktTekst
                                //dansk.SpecielVejkodeIndikator
                                //dansk.UkendtAdresseIndikator
                                //dansk.ValgkredsDistriktTekst

                            }
                            else if (cpr.FolkeregisterAdresse.Item is GroenlandAdresseType)
                            {
                                var groen = cpr.FolkeregisterAdresse.Item as GroenlandAdresseType;
                                if (groen.AddressCompleteGreenland != null)
                                {
                                    //groen.AddressCompleteGreenland.CountryIdentificationCode
                                    //groen.AddressCompleteGreenland.DistrictName
                                    //groen.AddressCompleteGreenland.DistrictSubdivisionIdentifier
                                    AddKeyField(keyNode, "ETAG", groen.AddressCompleteGreenland.FloorIdentifier);
                                    AddKeyField(keyNode, "BNR", groen.AddressCompleteGreenland.GreenlandBuildingIdentifier);
                                    //groen.AddressCompleteGreenland.MailDeliverySublocationIdentifier
                                    AddKeyField(keyNode, "KOMK", groen.AddressCompleteGreenland.MunicipalityCode);
                                    AddKeyField(keyNode, "POST", groen.AddressCompleteGreenland.PostCodeIdentifier);
                                    AddKeyField(keyNode, "HNR", groen.AddressCompleteGreenland.StreetBuildingIdentifier);
                                    AddKeyField(keyNode, "VEJK", groen.AddressCompleteGreenland.StreetCode);
                                    AddKeyField(keyNode, "VNVN", groen.AddressCompleteGreenland.StreetName);
                                    // TODO: Is this OK to add another VNVN
                                    AddKeyField(keyNode, "VNVN", groen.AddressCompleteGreenland.StreetNameForAddressingName);
                                    AddKeyField(keyNode, "SIDO", groen.AddressCompleteGreenland.SuiteIdentifier);
                                }
                                //groen.NoteTekst
                                //groen.SpecielVejkodeIndikator
                                //groen.UkendtAdresseIndikator
                            }
                            else if (cpr.FolkeregisterAdresse.Item is VerdenAdresseType)
                            {
                                // Unsupported
                            }
                        }
                        //cpr.ForskerBeskyttelseIndikator
                        //cpr.NavneAdresseBeskyttelseIndikator
                        //cpr.PersonCivilRegistrationIdentifier
                        if (cpr.PersonNationalityCode != null)
                        {
                            // TODO: Validate the scheme and convert value to the expected corresponding value
                            AddKeyField(keyNode, "STB", cpr.PersonNationalityCode.Value);
                        }
                        //cpr.PersonNummerGyldighedStatusIndikator
                        //cpr.TelefonNummerBeskyttelseIndikator                        
                    }
                    else if (oplys.Item is UdenlandskBorgerType)
                    {
                        // Not implemented
                    }
                    else if (oplys.Item is UkendtBorgerType)
                    {
                        // Not implemented
                    }
                }
            }
        }

        private static XmlNode InitKeyNode(XmlDocument doc)
        {
            var keyNode = doc.SelectSingleNode("//Key");
            keyNode.RemoveAll();
            return keyNode;
        }

        public void AddKeyField(XmlNode keyNode, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var fieldNode = keyNode.SelectSingleNode("//Field[@r='" + name + "']");
                if (fieldNode == null)
                {
                    fieldNode = keyNode.OwnerDocument.CreateNode(XmlNodeType.Element, "", "Field", "");
                    keyNode.AppendChild(fieldNode);

                    var rAttr = keyNode.OwnerDocument.CreateAttribute("r");
                    rAttr.Value = name;
                    fieldNode.Attributes.Append(rAttr);

                    var vAttr = keyNode.OwnerDocument.CreateAttribute("v");
                    vAttr.Value = value;
                    fieldNode.Attributes.Append(vAttr);

                }
                else
                {
                    fieldNode.Attributes["v"].Value = value;
                }
            }
        }

        public void CallADRESSE3(string token, string pnr)
        {
            var inp = Properties.Resources.ADRESSE3;
            var doc = new XmlDocument();
            doc.LoadXml(inp);
            var keyNode = InitKeyNode(doc);
            AddKeyField(keyNode, "PNR", pnr);
            var ret = "";
            var kvit = Send(doc.OuterXml, ref token, out ret);
            if (kvit.OK)
            {
                object ok = "";
            }
            object o = "";
        }
    }
}
