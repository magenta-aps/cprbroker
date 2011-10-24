using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public partial class CitizenPotReadyAddress
    {
        public static AdresseType ToAdresseType(CitizenPotReadyAddress address)
        {
            if (address != null)
            {
                return new AdresseType()
                {
                    Item = ToDanskAdresseType(address)
                };
            }
            return null;
        }

        public static DanskAdresseType ToDanskAdresseType(CitizenPotReadyAddress address)
        {
            if (address != null)
            {
                return new DanskAdresseType()
                {
                    AddressComplete = ToAddressCompleteType(address),
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
                };
            }
            return null;
        }

        public static AddressCompleteType ToAddressCompleteType(CitizenPotReadyAddress address)
        {
            if (address != null)
            {
                return new CprBroker.Schemas.Part.AddressCompleteType()
               {
                   AddressAccess = ToAddressAccessType(address),
                   AddressPostal = ToAddressPostalType(address)
               };
            }
            return null;
        }

        //TODO: What is Building number(Citizen table) vs house number
        public static AddressAccessType ToAddressAccessType(CitizenPotReadyAddress address)
        {
            if (address != null)
            {
                return new CprBroker.Schemas.Part.AddressAccessType()
               {
                   MunicipalityCode = Converters.ShortToString(address.MunicipalityCode),
                   StreetBuildingIdentifier = address.HouseNumber,
                   StreetCode = Converters.ShortToString(address.RoadCode)
               };
            }
            return null;
        }

        public static AddressPostalType ToAddressPostalType(CitizenPotReadyAddress address)
        {
            if (address != null)
            {
                return new CprBroker.Schemas.Part.AddressPostalType()
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
               };
            }
            return null;
        }

    }
}
