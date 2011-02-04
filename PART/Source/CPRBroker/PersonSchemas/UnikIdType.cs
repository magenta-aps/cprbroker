using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class UnikIdType
    {
        public static UnikIdType Create(Guid targetUuid)
        {
            // TODO : Check this code
            return new UnikIdType()
            {
                Item = targetUuid.ToString(""),
                ItemElementName =  ItemChoiceType.UUID
            };
        }
        public static UnikIdType Create(Uri urn)
        {
            return new UnikIdType()
            {
                Item = urn.ToString(),
                ItemElementName = ItemChoiceType.URNIdentifikator
            };
        }
    }
}
