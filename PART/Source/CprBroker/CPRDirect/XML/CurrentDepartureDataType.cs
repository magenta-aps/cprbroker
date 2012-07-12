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
                // Addres details
                ForeignAddressStructure = this.ToForeignAddressStructureType(),

                // Note - not implemented
                NoteTekst = this.ToAddressNoteTekste(),
                
                // Address is known if it has value !
                UkendtAdresseIndikator = this.IsEmpty
            };
        }

        public ForeignAddressStructureType ToForeignAddressStructureType()
        {
            return new ForeignAddressStructureType()
            {
                // Country
                CountryIdentificationCode = ToCountryIdentificationCode(),
                
                // No location
                LocationDescriptionText = null,
                
                // Address lines
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
