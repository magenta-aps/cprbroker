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
                Item = this.ToDanskAdresseType()
            };
        }

        public DanskAdresseType ToDanskAdresseType()
        {
            var ret = new DanskAdresseType()
            {
                AddressComplete = null,
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
            if (this == null)
            {
                // TODO: See when to set this flag
                ret.UkendtAdresseIndikator = true;
            }
            else
            {
                ret.AddressComplete = this.ToAddressCompleteType();
                // TODO: Find a way to get the postal code if not found in the current way
                if (this.HousePostCode != null)
                {
                    ret.PostDistriktTekst = this.HousePostCode.PostDistrict;
                }
            }
            return ret;
        }

        public AddressCompleteType ToAddressCompleteType()
        {
            return new CprBroker.Schemas.Part.AddressCompleteType()
           {
               AddressAccess = this.ToAddressAccessType(),
               AddressPostal = this.ToAddressPostalType()
           };
        }

        public AddressAccessType ToAddressAccessType()
        {
            return new CprBroker.Schemas.Part.AddressAccessType()
           {
               MunicipalityCode = Converters.ShortToString(this.MunicipalityCode),
               StreetBuildingIdentifier = this.HouseNumber,
               StreetCode = Converters.ShortToString(this.RoadCode)
           };
        }

        public AddressPostalType ToAddressPostalType()
        {
            var ret = new CprBroker.Schemas.Part.AddressPostalType()
           {
               CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkCountryCode.ToString()),
               DistrictSubdivisionIdentifier = null,
               FloorIdentifier = this.Floor,
               MailDeliverySublocationIdentifier = null,
               PostCodeIdentifier = null,
               DistrictName = null,
               PostOfficeBoxIdentifier = null,
               StreetBuildingIdentifier = this.HouseNumber,
               StreetName = null,
               StreetNameForAddressingName = null,
               SuiteIdentifier = this.Door,
           };
            if (this.HousePostCode != null)
            {
                ret.PostCodeIdentifier = Converters.ShortToString(this.HousePostCode.PostCode);
                ret.DistrictName = this.HousePostCode.PostDistrict;
                ret.StreetName = this.HousePostCode.RoadName;
            }
            return ret;
        }

    }
}
