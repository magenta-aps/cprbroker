using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.DPR
{
    public partial class PersonAddress
    {
        public AdresseType ToAdresseType(Street street)
        {
            return new AdresseType()
            {
                Item = new DanskAdresseType()
                {
                    AddressComplete = new CprBroker.Schemas.Part.AddressCompleteType()
                    {
                        AddressAccess = new CprBroker.Schemas.Part.AddressAccessType()
                        {
                            MunicipalityCode = MunicipalityCode.ToDecimalString(),
                            StreetBuildingIdentifier = HouseNumber,
                            StreetCode = StreetCode.ToDecimalString()
                        },
                        AddressPostal = new CprBroker.Schemas.Part.AddressPostalType()
                        {
                            CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkKmdCode),
                            DistrictName = Town,
                            DistrictSubdivisionIdentifier = null,
                            FloorIdentifier = Floor,
                            MailDeliverySublocationIdentifier = null,
                            PostCodeIdentifier = PostCode.ToDecimalString(),
                            PostOfficeBoxIdentifier = null,
                            StreetBuildingIdentifier = HouseNumber,
                            StreetName = street != null ? street.StreetAddressingName : null,
                            StreetNameForAddressingName = StreetAddressingName,
                            SuiteIdentifier = DoorNumber,
                        }
                    },
                    // No address point
                    AddressPoint = null,
                    NoteTekst = null,
                    PolitiDistriktTekst = null,
                    PostDistriktTekst = null,
                    SkoleDistriktTekst = null,
                    SocialDistriktTekst = null,
                    SogneDistriktTekst = null,
                    SpecielVejkodeIndikator = false,
                    SpecielVejkodeIndikatorSpecified = false,
                    UkendtAdresseIndikator = false,
                    ValgkredsDistriktTekst = null
                }
            };
        }

        public AdresseType ToForeignAddressFromSupplementary()
        {
            return new AdresseType()
            {
                Item = new VerdenAdresseType()
                {
                    ForeignAddressStructure = new ForeignAddressStructureType()
                    {
                        CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkKmdCode),
                        LocationDescriptionText = Location,
                        PostalAddressFirstLineText = AdditionalAddressLine1,
                        PostalAddressSecondLineText = AdditionalAddressLine2,
                        PostalAddressThirdLineText = AdditionalAddressLine3,
                        PostalAddressFourthLineText = AdditionalAddressLine4,
                        PostalAddressFifthLineText = AdditionalAddressLine5,
                    },
                    NoteTekst = null,
                    UkendtAdresseIndikator = false
                }
            };
        }

    }
}
