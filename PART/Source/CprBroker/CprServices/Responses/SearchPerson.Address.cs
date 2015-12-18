using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CprBroker.Utilities;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CprServices.Responses
{
    public partial class SearchPerson
    {
        public AdresseType ToAdresseType()
        {
            var kom = GetFieldValue(_Node, "KOMKOD");
            var foreignCountryCode = GetFieldValue(_Node, "UDRLANDEKOD");
            if (!string.IsNullOrEmpty(kom))
            {
                var komK = decimal.Parse(kom);

                return new AdresseType()
                {
                    Item =
                        komK >= CprBroker.Schemas.Part.AddressConstants.GreenlandMunicipalCodeStart ?
                            ToGroenlandskAdresseType(_Node, kom) as AdresseBaseType
                            : ToDanskAdresseType(_Node, kom)
                };
            }
            else if (string.IsNullOrEmpty(foreignCountryCode))
            {
                return ToAdresseTypeFromString(_Node);
            }
            else
            {
                return new AdresseType()
                {
                    Item = ToVerdenAdresseType(_Node, foreignCountryCode)
                };
            }
        }

        public DanskAdresseType ToDanskAdresseType(XmlElement elm, string kom)
        {
            return new DanskAdresseType()
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

                        StreetBuildingIdentifier = GetFieldValue(elm, "HUSNR"),
                        FloorIdentifier = GetFieldValue(elm, "ETAGE"),
                        SuiteIdentifier = GetFieldValue(elm, "SIDEDOER"),

                        // Post code, district & town
                        PostCodeIdentifier = GetFieldValue(elm, "POSTNR"),
                        DistrictName = ToPostDistrictText(elm),
                        DistrictSubdivisionIdentifier = GetFieldValue(elm, "BYNVN"),

                        // TODO: This works only in Lookup metrhods. Lookup street name and addressing name in Search methods
                        StreetName = Strings.FirstNonEmpty(
                            GetFieldText(elm, "VEJKOD"), // ADRESSE3
                            GetFieldValue(elm, "VEJADRNVN")), // Stam+
                        StreetNameForAddressingName = Strings.FirstNonEmpty(
                             GetFieldText(elm, "VEJKOD"), // ADRESSE3
                             GetFieldValue(elm, "VEJADRNVN")), // Stam+

                        // Not implemented
                        MailDeliverySublocationIdentifier = null,
                        PostOfficeBoxIdentifier = null,
                    }
                },

                PostDistriktTekst = ToPostDistrictText(elm),
                SpecielVejkodeIndikator = ToSpecielVejkodeIndikator(elm),
                SpecielVejkodeIndikatorSpecified = true,
                UkendtAdresseIndikator = false,

                // District lookup
                AddressPoint = null,
                NoteTekst = null,
                PolitiDistriktTekst = null,
                SkoleDistriktTekst = null,
                SogneDistriktTekst = null,
                SocialDistriktTekst = null,
                ValgkredsDistriktTekst = null,
            };
        }

        public GroenlandAdresseType ToGroenlandskAdresseType(XmlElement elm, string kom)
        {
            return new GroenlandAdresseType()
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

                    StreetName = ToStreetName(elm),
                    StreetNameForAddressingName = ToStreetName(elm),

                    // Post code, district & town
                    PostCodeIdentifier = GetFieldValue(elm, "POSTNR"),
                    DistrictName = ToPostDistrictText(elm),
                    DistrictSubdivisionIdentifier = GetFieldValue(elm, "BYNVN"),

                    // Unsupported
                    MailDeliverySublocationIdentifier = null,
                },
                SpecielVejkodeIndikator = ToSpecielVejkodeIndikator(elm),
                SpecielVejkodeIndikatorSpecified = true,
                UkendtAdresseIndikator = false,

                // Unsupported
                NoteTekst = null,
            };
        }

        public string ToStreetName(XmlElement elm)
        {
            return Strings.FirstNonEmpty(
                GetFieldText(elm, "VEJKOD"), // ADRESSE3
                GetFieldValue(elm, "VEJADRNVN")// Stam+
            );
        }

        public string ToPostDistrictText(XmlElement elm)
        {
            // TODO: Fill post district in search methods
            return Strings.FirstNonEmpty(
                GetFieldValue(elm, "CPSN_POSTDISTTXT"),
                GetFieldText(elm, "POSTNR")
            );
        }

        public bool ToSpecielVejkodeIndikator(XmlElement elm)
        {
            return Schemas.Util.Converters.ToSpecielVejkodeIndikator(GetFieldValue(elm, "VEJKOD"));
        }

        public AdresseType ToAdresseTypeFromString(XmlElement elm)
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
                var postDist = ToPostDistrictText(elm);

                // Fill the object
                return new AdresseType()
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
            }
            return null;
        }

        public VerdenAdresseType ToVerdenAdresseType(XmlElement elm, string foreignCountryCode)
        {
            return new VerdenAdresseType()
            {
                ForeignAddressStructure = new ForeignAddressStructureType()
                {
                    CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, foreignCountryCode),
                    PostalAddressFirstLineText = GetFieldValue(elm, "UDLANDSADR1"),
                    PostalAddressSecondLineText = GetFieldValue(elm, "UDLANDSADR2"),
                    PostalAddressThirdLineText = GetFieldValue(elm, "UDLANDSADR3"),
                    PostalAddressFourthLineText = GetFieldValue(elm, "UDLANDSADR4"),
                    PostalAddressFifthLineText = GetFieldValue(elm, "UDLANDSADR5"),
                    LocationDescriptionText = null,
                }, 
                
                NoteTekst = null,
                UkendtAdresseIndikator = false
            };
        }

        public AdresseType ToContactAddress()
        {
            // TODO: Shall this be filled from ADRESSE4?
            return null;
        }
    }
}
