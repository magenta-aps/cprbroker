using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public static class PersonRelationTypeHelper
    {
        public static TRelation Create<TRelation>(Guid targetUuid, DateTime? fromDate, DateTime? toDate) where TRelation : IPersonRelationType, new()
        {
            if (targetUuid != Guid.Empty)
            {
                return new TRelation()
                {
                    //CommentText not supported
                    CommentText = null,
                    CprNumber = null,
                    ReferenceID = UnikIdType.Create(targetUuid),
                    // TODO: Fill virkning object from parameters
                    Virkning = VirkningType.Create(fromDate, toDate)
                };
            }
            else
            {
                throw new ArgumentNullException("targetUuid");
            }
        }

        public static TRelation Create<TRelation>(string cprNumber, DateTime? fromDate, DateTime? toDate) where TRelation : IPersonRelationType, new()
        {
            if (!string.IsNullOrEmpty(cprNumber))
            {
                return new TRelation()
                {
                    //CommentText not supported
                    CommentText = null,
                    CprNumber = cprNumber,
                    ReferenceID = null,
                    // TODO: Fill virkning object from parameters
                    Virkning = VirkningType.Create(fromDate, toDate)
                };
            }
            else
            {
                throw new ArgumentNullException("cprNumber");
            }
        }
    }
}
