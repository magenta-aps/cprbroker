using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public partial class CitizenPotReadyAddress
    {
        //TODO: What is Building number(Citizen table) vs house number
        internal static AdresseType ToAdresseType(CitizenPotReadyAddress address)
        {
            return new AdresseType()
            {
                Item = new DanskAdresseType()
                {
                    AddressComplete = new CprBroker.Schemas.Part.AddressCompleteType()
                    {
                        AddressAccess = new CprBroker.Schemas.Part.AddressAccessType()
                        {
                            MunicipalityCode = Converters.ShortToString(address.MunicipalityCode),
                            StreetBuildingIdentifier = address.HouseNumber,
                            StreetCode = Converters.ShortToString(address.RoadCode)
                        },
                        AddressPostal = new CprBroker.Schemas.Part.AddressPostalType()
                        {
                            CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkCountryCode.ToString()),
                            DistrictName = address.PostDistrict,
                            DistrictSubdivisionIdentifier = null,
                            FloorIdentifier = address.Floor,
                            MailDeliverySublocationIdentifier = null,
                            PostCodeIdentifier = Converters.ShortToString(address.PostCode),
                            PostOfficeBoxIdentifier = null,
                            StreetBuildingIdentifier = address.HouseNumber,
                            StreetName = address.RoadName,
                            StreetNameForAddressingName = null,
                            SuiteIdentifier = address.Door,
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
    }
}
