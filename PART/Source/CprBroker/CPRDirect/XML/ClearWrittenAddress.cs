using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class ClearWrittenAddressType
    {
        public AdresseType ToAdresseType()
        {
            return new AdresseType()
            {
                Item = ToDanskAdresseType()
            };
        }

        public bool IsEmpty
        {
            get
            {
                // this.AddressingName is ignored
                object[] parts = new object[] { this.BuildingNumber, this.CareOfName, this.CityName, this.Door, this.Floor, this.HouseNumber, this.LabelledAddress, this.Location, this.MunicipalityCode, this.PostCode, this.PostDistrictText, this.StreetAddressingName, this.StreetCode };
                var strArr = parts
                    .Select(o => o.ToString().Replace("0", "").Trim())
                    .ToArray();

                return string.IsNullOrEmpty(
                    string.Join("", strArr)
                    );
            }
        }

        public DanskAdresseType ToDanskAdresseType()
        {
            var ret = new DanskAdresseType()
            {
                AddressComplete = this.ToAddressCompleteType(),

                // No address point for persons
                AddressPoint = this.ToAddressPointType(),

                NoteTekst = ToAddressNoteTekste(),

                // No political districts
                PolitiDistriktTekst = null,

                PostDistriktTekst = this.PostDistrictText,

                // No school district
                SkoleDistriktTekst = null,

                // No social disrict
                SocialDistriktTekst = null,

                // No church district - checked
                SogneDistriktTekst = null,

                // Assuming this is the same as high road code - verified
                SpecielVejkodeIndikator = this.ToSpecielVejkodeIndikator(),

                // Always true because SpecielVejkodeIndikator is always set                
                SpecielVejkodeIndikatorSpecified = true,

                // Address is unknown if it is empty :)
                UkendtAdresseIndikator = IsEmpty,

                // No election district - checked
                ValgkredsDistriktTekst = null
            };
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

        public string ToAddressNoteTekste()
        {
            // No address note
            // I do not think it is the same as RelocationOrder.BEMAERK-FLYTTEPÅBUD                
            return null;
        }

        public AddressPointType ToAddressPointType()
        {
            // Not implemented
            return null;
        }

        public bool ToSpecielVejkodeIndikator()
        {
            return Schemas.Util.Converters.ToSpecielVejkodeIndikator(this.StreetCode);
        }

        public AddressAccessType ToAddressAccessType()
        {
            return new CprBroker.Schemas.Part.AddressAccessType()
            {
                MunicipalityCode = Converters.DecimalToString(this.MunicipalityCode),
                StreetBuildingIdentifier = this.HouseNumber,
                StreetCode = Converters.DecimalToString(this.StreetCode)
            };
        }

        /// <summary>
        /// Converts the current object to AddressPostalType object
        /// </summary>
        /// <returns></returns>
        public AddressPostalType ToAddressPostalType()
        {
            var ret = new CprBroker.Schemas.Part.AddressPostalType()
            {
                // Set country code
                CountryIdentificationCode = CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Constants.DenmarkCountryCode.ToString()),

                // DistrictSubdivisionIdentifier is not supported - checked
                DistrictSubdivisionIdentifier = null,

                // Set floor
                FloorIdentifier = this.Floor,

                // MailDeliverySublocationIdentifier is not supported - checked
                MailDeliverySublocationIdentifier = null,

                // Set post code
                PostCodeIdentifier = Converters.DecimalToString(this.PostCode),

                // Set post district
                DistrictName = this.PostDistrictText,

                // PostOfficeBoxIdentifier is not supported
                PostOfficeBoxIdentifier = null,

                // Set building identifier
                StreetBuildingIdentifier = this.HouseNumber,

                // Set street name
                StreetName = this.StreetAddressingName,

                // Set street addressing name
                StreetNameForAddressingName = this.StreetAddressingName,

                // Set suite identifier
                SuiteIdentifier = this.Door,
            };
            return ret;
        }

    }
}
