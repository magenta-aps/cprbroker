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
            return new UnikIdType()
            {
                Item = targetUuid.ToString(""),
                ItemElementName = ItemChoiceType.UUID
            };
        }
    }
}
