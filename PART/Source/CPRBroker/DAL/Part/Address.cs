using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class Address
    {
        public AdresseType ToXmlType()
        {
            var ret = new AdresseType()
            {
                Item = null,
            };

            if (DenmarkAddress != null)
            {
                ret.Item = DenmarkAddress.ToXmlType();
            }
            else if (ForeignAddress != null)
            {
                ret.Item = ForeignAddress.ToXmlType();
            }
            if (ret != null)
            {
                ret.Item.NoteTekst = this.Note;
                ret.Item.UkendtAdresseIndikator = this.IsUnknown;
            }
            return ret;
        }

        public static Address FromXmlType(AdresseType oio)
        {
            if (oio == null || oio.Item == null)
            {
                return null;
            }

            var ret = new Address()
            {
                AddressId = Guid.NewGuid(),
                IsUnknown = oio.Item.UkendtAdresseIndikator,
                Note = oio.Item.NoteTekst
            };

            if (oio.Item is DanskAdresseType)
            {
                ret.DenmarkAddress = DenmarkAddress.FromXmlType(oio.Item as DanskAdresseType);
            }
            else if (oio.Item is GroenlandAdresseType)
            {
                ret.DenmarkAddress = DenmarkAddress.FromXmlType(oio.Item as GroenlandAdresseType);
            }
            else if (oio.Item is VerdenAdresseType)
            {
                ret.ForeignAddress = ForeignAddress.FromXmlType(oio.Item as VerdenAdresseType);
            }

            return ret;
        }
    }
}
