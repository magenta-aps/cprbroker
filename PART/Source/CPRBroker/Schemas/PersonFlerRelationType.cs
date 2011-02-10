using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class PersonFlerRelationType
    {
        public static PersonFlerRelationType Create(Guid uuid, DateTime? fromDate, DateTime? toDate)
        {
            return new PersonFlerRelationType()
            {
                //TODO: Add comment text
                CommentText = null,
                ReferenceID = UnikIdType.Create(uuid),
                // TODO: Fill virkning object fromDate parameters
                Virkning = VirkningType.Create(fromDate, toDate)
            };
        }

        //TODO: add parameters for fromDate and to dates
        public static PersonFlerRelationType[] CreateList(params Guid[] targetUuids)
        {
            return Array.ConvertAll<Guid, PersonFlerRelationType>
            (
                targetUuids,
                (uuid) => Create(uuid, null, null)
            );
        }
    }
}
