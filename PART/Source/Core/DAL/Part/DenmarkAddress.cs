using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    public partial class DenmarkAddress
    {
        public static AdresseBaseType ToXmlType(DenmarkAddress db)
        {
            if (db.DanishAddress != null)
            {
                return DanishAddress.ToXmlType(db.DanishAddress);
            }
            else if (db.GreenlandicAddress != null)
            {
                return GreenlandicAddress.ToXmlType(db.GreenlandicAddress);
            }
            return null;
        }

        public static DenmarkAddress FromXmlType(AdresseBaseType oio)
        {
            if (oio is DanskAdresseType)
            {
                return DanishAddress.FromXmlType(oio as DanskAdresseType).DenmarkAddress;
            }
            else if (oio is GroenlandAdresseType)
            {
                return GreenlandicAddress.FromXmlType(oio as GroenlandAdresseType).DenmarkAddress;
            }
            return null;
        }

        public static void SetChildLoadOptions(DataLoadOptions loadOptions)
        {
            loadOptions.LoadWith<DenmarkAddress>(da => da.CountryRef);
            loadOptions.LoadWith<DenmarkAddress>(da => da.DanishAddress);
            loadOptions.LoadWith<DenmarkAddress>(da => da.GreenlandicAddress);

            DanishAddress.SetChildLoadOptions(loadOptions);
        }
    }
}
