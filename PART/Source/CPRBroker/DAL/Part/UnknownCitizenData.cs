using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.DAL.Part
{
    public partial class UnknownCitizenData
    {
        public UkendtBorgerType ToXmlType()
        {
            return new UkendtBorgerType()
            {
                PersonCivilRegistrationReplacementIdentifier = CprNumber
            };
        }

        public static UnknownCitizenData FromXmlType(UkendtBorgerType partUnknownData)
        {
            return new UnknownCitizenData()
            {
                CprNumber = partUnknownData.PersonCivilRegistrationReplacementIdentifier,
            };
        }
    }
}
