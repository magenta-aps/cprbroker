using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.DPR
{
    public partial class ContactAddress
    {
        public static AdresseType ToXmlType(ContactAddress db)
        {
            if (db != null)
            {
                return new AdresseType()
                {
                    Item = new VerdenAdresseType()
                    {
                        ForeignAddressStructure = new ForeignAddressStructureType()
                        {
                            CountryIdentificationCode = null,
                            LocationDescriptionText = null,
                            PostalAddressFirstLineText = db.Line1,
                            PostalAddressSecondLineText = db.Line2,
                            PostalAddressThirdLineText = db.Line3,
                            PostalAddressFourthLineText = db.Line4,
                            PostalAddressFifthLineText = db.Line5,
                        },
                        NoteTekst = null,
                        UkendtAdresseIndikator = false,
                    }
                };
            }
            return null;
        }
    }
}
