using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class CurrentDepartureDataType : IAddressSource
    {
        public AdresseType ToAdresseType()
        {
            return new AdresseType()
            {
                Item = new VerdenAdresseType()
            };
        }

        public VerdenAdresseType ToVerdenAdresseType()
        {
            return new VerdenAdresseType()
            {
                ForeignAddressStructure = this.ToForeignAddressStructureType(),
                NoteTekst = this.ToAddressNoteTekste(),
                // Address is always known
                UkendtAdresseIndikator = false
            };
        }

        public ForeignAddressStructureType ToForeignAddressStructureType()
        {
            return new ForeignAddressStructureType()
            {
                CountryIdentificationCode = ToCountryIdentificationCode(),
                // No location
                LocationDescriptionText = null,
                PostalAddressFirstLineText = this.ForeignAddress1,
                PostalAddressSecondLineText = this.ForeignAddress2,
                PostalAddressThirdLineText = this.ForeignAddress3,
                PostalAddressFourthLineText = this.ForeignAddress4,
                PostalAddressFifthLineText = this.ForeignAddress5

            };
        }

        public bool IsEmpty
        {
            get
            {
                var arr = new string[] { this.ForeignAddress1, this.ForeignAddress2, this.ForeignAddress3, this.ForeignAddress4, this.ForeignAddress5 };
                return string.IsNullOrEmpty(string.Join("", arr));
            }
        }

        public VirkningType[] ToVirkningTypeArray()
        {
            return new VirkningType[]{
                VirkningType.Create(
                Converters.ToDateTime(this.ExitDate, this.ExitDateUncertainty),
                null)};
        }

        public CountryIdentificationCodeType ToCountryIdentificationCode()
        {
            return CountryIdentificationCodeType.Create(_CountryIdentificationSchemeType.imk, Converters.DecimalToString(this.ExitCountryCode));
        }



        public string ToAddressNoteTekste()
        {
            return null;
        }
    }
}
