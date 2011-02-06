using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class ForeignAddress
    {
        public VerdenAdresseType ToXmlType()
        {
            var ret= new VerdenAdresseType()
            {
                ForeignAddressStructure = new ForeignAddressStructureType()
                {                    
                    LocationDescriptionText = this.LocationDescription,
                    PostalAddressFirstLineText = this.FirstLine,
                    PostalAddressSecondLineText = this.SecondLine,
                    PostalAddressThirdLineText = this.ThirdLine,
                    PostalAddressFourthLineText = this.FourthLine,
                    PostalAddressFifthLineText = this.FifthLine
                }
            };
            if (this.CountrySchemeTypeId.HasValue)
            {
                ret.ForeignAddressStructure.CountryIdentificationCode = CountryIdentificationCodeType.Create((_CountryIdentificationSchemeType)CountrySchemeTypeId, CountryCode);
            }
            return ret;
        }

        public static ForeignAddress FromXmlType(VerdenAdresseType oio)
        {
            var ret = new ForeignAddress()
            {
                LocationDescription = oio.ForeignAddressStructure.LocationDescriptionText,

                FirstLine = oio.ForeignAddressStructure.PostalAddressFirstLineText,
                SecondLine = oio.ForeignAddressStructure.PostalAddressSecondLineText,
                ThirdLine = oio.ForeignAddressStructure.PostalAddressThirdLineText,
                FourthLine = oio.ForeignAddressStructure.PostalAddressFourthLineText,
                FifthLine = oio.ForeignAddressStructure.PostalAddressFifthLineText,

            };
            if (oio.ForeignAddressStructure.CountryIdentificationCode != null)
            {
                ret.CountryCode = oio.ForeignAddressStructure.CountryIdentificationCode.Value;
                ret.CountrySchemeTypeId = (int)oio.ForeignAddressStructure.CountryIdentificationCode.scheme;
            }
            return ret;
        }
    }
}
