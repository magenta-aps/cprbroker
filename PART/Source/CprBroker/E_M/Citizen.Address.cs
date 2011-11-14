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
                AddressComplete = this.ToAddressCompleteType(),
                // No address point for persons
                AddressPoint = null,
                // No address note
                NoteTekst = null,
                // No political districts
                PolitiDistriktTekst = null,
                // Will be set later in this method
                PostDistriktTekst = null,
                // No school district
                SkoleDistriktTekst = null,
                // No social disrict
                SocialDistriktTekst = null,
                // No church district
                SogneDistriktTekst = null,
                // Assumed as high road code
                SpecielVejkodeIndikator = ToSpecielVejkodeIndikator(),
                // Always true because SpecielVejkodeIndikator is always set
                SpecielVejkodeIndikatorSpecified = true,
                // Always false
                UkendtAdresseIndikator = false,
                // No election district
                ValgkredsDistriktTekst = null
            };
            // TODO: Find a way to get the postal code if not found in the current way
            if (this.HousePostCode != null)
            {
                ret.PostDistriktTekst = this.HousePostCode.PostDistrict;
            }
            return ret;
        }

        public bool ToSpecielVejkodeIndikator()
        {
            if (RoadCode >= 1 && RoadCode <= 9999)
            {
                return this.RoadCode >= 9900;
            }
            else
            {
                throw new ArgumentException(string.Format("RoadCode <{0}> must be between 1 and 9999", RoadCode));
            }
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
