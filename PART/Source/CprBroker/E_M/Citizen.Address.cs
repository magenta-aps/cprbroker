using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public partial class Citizen
    {
        public virtual AdresseType ToAdresseType()
        {
            return new AdresseType()
            {
                Item = ToDanskAdresseType(this)
            };
        }

        public static DanskAdresseType ToDanskAdresseType(Citizen citizen)
        {
            var ret = new DanskAdresseType()
            {
                AddressComplete = null,
                // TODO: Fill citizen point
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
            if (citizen == null)
            {
                ret.UkendtAdresseIndikator = true;
            }
            else
            {
                ret.AddressComplete = ToAddressCompleteType(citizen);
                if (citizen.HousePostCode != null)
                {
                    ret.PostDistriktTekst = citizen.HousePostCode.PostDistrict;
                }
            }
            return ret;
        }

        public static AddressCompleteType ToAddressCompleteType(Citizen citizen)
        {
            if (citizen != null)
            {
                return new CprBroker.Schemas.Part.AddressCompleteType()
               {
                   AddressAccess = ToAddressAccessType(citizen),
                   AddressPostal = ToAddressPostalType(citizen)
               };
            }
            return null;
        }

        public static AddressAccessType ToAddressAccessType(Citizen citizen)
        {
            if (citizen != null)
            {
                return new CprBroker.Schemas.Part.AddressAccessType()
               {
                   MunicipalityCode = Converters.ShortToString(citizen.MunicipalityCode),
                   StreetBuildingIdentifier = citizen.HouseNumber,
                   StreetCode = Converters.ShortToString(citizen.RoadCode)
               };
            }
            return null;
        }

        public static AddressPostalType ToAddressPostalType(Citizen citizen)
        {
            if (citizen != null)
            {
                var ret = new CprBroker.Schemas.Part.AddressPostalType()
               {
                   CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkCountryCode.ToString()),
                   DistrictSubdivisionIdentifier = null,
                   FloorIdentifier = citizen.Floor,
                   MailDeliverySublocationIdentifier = null,
                   PostCodeIdentifier = null,
                   DistrictName = null,
                   PostOfficeBoxIdentifier = null,
                   StreetBuildingIdentifier = citizen.HouseNumber,
                   StreetName = null,
                   StreetNameForAddressingName = null,
                   SuiteIdentifier = citizen.Door,
               };
                if (citizen.HousePostCode != null)
                {
                    ret.PostCodeIdentifier = Converters.ShortToString(citizen.HousePostCode.PostCode);
                    ret.DistrictName = citizen.HousePostCode.PostDistrict;
                    ret.StreetName = citizen.HousePostCode.RoadName;

                }
                return ret;
            }
            return null;
        }

    }
}
