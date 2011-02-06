using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class DenmarkAddress
    {
        public AdresseBaseType ToXmlType()
        {
            if (this.DanishAddress != null)
            {
                return DanishAddress.ToXmlType();
            }
            else if (this.GreenlandicAddress != null)
            {
                return GreenlandicAddress.ToXmlType();
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
    }
}
