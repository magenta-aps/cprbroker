using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the UnknownCitizenData table
    /// </summary>
    public partial class UnknownCitizenData
    {
        public static UkendtBorgerType ToXmlType(UnknownCitizenData db)
        {
            if (db != null)
            {
                return new UkendtBorgerType()
                {
                    PersonCivilRegistrationReplacementIdentifier = db.CprNumber
                };
            }
            return null;
        }

        public static UnknownCitizenData FromXmlType(RegisterOplysningType[] oio)
        {
            if (oio != null && oio.Length > 0 && oio[0] != null)
            {
                return FromXmlType(oio[0].Item as UkendtBorgerType);
            }
            return null;
        }

        public static UnknownCitizenData FromXmlType(UkendtBorgerType partUnknownData)
        {
            if (partUnknownData != null)
            {
                return new UnknownCitizenData()
                {
                    CprNumber = partUnknownData.PersonCivilRegistrationReplacementIdentifier,
                };
            }
            return null;
        }
    }
}
