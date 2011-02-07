using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class ForeignAddress
    {
        public static VerdenAdresseType ToXmlType(ForeignAddress db)
        {
            if (db != null)
            {
                var ret = new VerdenAdresseType()
                {
                    ForeignAddressStructure = new ForeignAddressStructureType()
                    {
                        LocationDescriptionText = db.LocationDescription,
                        PostalAddressFirstLineText = db.FirstLine,
                        PostalAddressSecondLineText = db.SecondLine,
                        PostalAddressThirdLineText = db.ThirdLine,
                        PostalAddressFourthLineText = db.FourthLine,
                        PostalAddressFifthLineText = db.FifthLine
                    }
                };
                if (db.CountrySchemeTypeId.HasValue)
                {
                    ret.ForeignAddressStructure.CountryIdentificationCode = CountryIdentificationCodeType.Create((_CountryIdentificationSchemeType)db.CountrySchemeTypeId, db.CountryCode);
                }
                return ret;
            }
            return null;
        }

        public static ForeignAddress FromXmlType(VerdenAdresseType oio)
        {
            if (oio != null)
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
            return null;
        }
    }
}
