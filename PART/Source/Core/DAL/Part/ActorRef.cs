using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Data.Part
{
    /// <summary>
    /// Represents the ActorRef table
    /// </summary>
    public partial class ActorRef
    {
        public static UnikIdType ToXmlType(ActorRef db)
        {
            if (db != null)
            {
                return new UnikIdType()
                {
                    Item = db.Value,
                    ItemElementName = (ItemChoiceType)db.Type,
                };
            }
            return null;
        }

        public static ActorRef FromXmlType(UnikIdType oio)
        {
            if (oio != null)
            {
                return new ActorRef()
                {
                    ActorRefId = Guid.NewGuid(),
                    Value = oio.Item,
                    Type = (int)oio.ItemElementName,
                };
            }
            return null;
        }

    }
}
