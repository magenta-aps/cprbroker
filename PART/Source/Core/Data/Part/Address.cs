using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the Address table
    /// </summary>
    public partial class Address
    {
        public static AdresseType ToXmlType(Address db)
        {
            if (db != null)
            {
                var ret = new AdresseType()
                {
                    Item = null,
                };

                if (db.DenmarkAddress != null)
                {
                    ret.Item = DenmarkAddress.ToXmlType(db.DenmarkAddress);
                }
                else if (db.ForeignAddress != null)
                {
                    ret.Item = ForeignAddress.ToXmlType(db.ForeignAddress);
                }
                if (ret != null)
                {
                    ret.Item.NoteTekst = db.Note;
                    ret.Item.UkendtAdresseIndikator = db.IsUnknown;
                }
                return ret;
            }
            return null;
        }

        public static Address FromXmlType(AdresseType oio)
        {
            if (oio != null && oio.Item != null)
            {
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
            return null;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<Address>(a => a.DenmarkAddress);
            loadOptions.LoadWith<Address>(a => a.ForeignAddress);

            DenmarkAddress.SetChildLoadOptions(loadOptions);
            ForeignAddress.SetChildLoadOptions(loadOptions);
        }
    }
}
