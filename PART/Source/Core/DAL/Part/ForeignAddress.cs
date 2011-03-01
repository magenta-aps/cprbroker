using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
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
                        CountryIdentificationCode = CountryRef.ToXmlType(db.CountryRef),
                        LocationDescriptionText = db.LocationDescription,
                        PostalAddressFirstLineText = db.FirstLine,
                        PostalAddressSecondLineText = db.SecondLine,
                        PostalAddressThirdLineText = db.ThirdLine,
                        PostalAddressFourthLineText = db.FourthLine,
                        PostalAddressFifthLineText = db.FifthLine
                    }
                };
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
                    CountryRef = CountryRef.FromXmlType(oio.ForeignAddressStructure.CountryIdentificationCode),
                    FirstLine = oio.ForeignAddressStructure.PostalAddressFirstLineText,
                    SecondLine = oio.ForeignAddressStructure.PostalAddressSecondLineText,
                    ThirdLine = oio.ForeignAddressStructure.PostalAddressThirdLineText,
                    FourthLine = oio.ForeignAddressStructure.PostalAddressFourthLineText,
                    FifthLine = oio.ForeignAddressStructure.PostalAddressFifthLineText,
                };
                return ret;
            }
            return null;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<ForeignAddress>(fa => fa.CountryRef);
        }
    }
}
