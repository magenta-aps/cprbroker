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
        internal AdresseType ToAdresseType()
        {
            return new AdresseType()
            {
                Item = new DanskAdresseType()
                {
                    AddressComplete = new CprBroker.Schemas.Part.AddressCompleteType()
                    {
                        AddressAccess = new CprBroker.Schemas.Part.AddressAccessType()
                        {
                            MunicipalityCode = Converters.ShortToString(MunicipalityCode),
                            StreetBuildingIdentifier = HouseNumber,
                            StreetCode = Converters.ShortToString(RoadCode)
                        },
                        AddressPostal = new CprBroker.Schemas.Part.AddressPostalType()
                        {
                            CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkCountryCode.ToString()),
                            DistrictName = PostDistrict,
                            DistrictSubdivisionIdentifier = null,
                            FloorIdentifier = Floor,
                            MailDeliverySublocationIdentifier = null,
                            PostCodeIdentifier = Converters.ShortToString(PostCode),
                            PostOfficeBoxIdentifier = null,
                            StreetBuildingIdentifier = HouseNumber,
                            StreetName = RoadName,
                            StreetNameForAddressingName = null,
                            SuiteIdentifier = Door,
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
