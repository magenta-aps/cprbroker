using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class PersonRelationType
    {
        public static PersonRelationType Create(Guid targetUuid, DateTime? fromDate, DateTime? toDate)
        {
            return new PersonRelationType()
                {
                    CommentText = null,                    
                    ReferenceID = UnikIdType.Create(targetUuid),                    
                    Virkning = VirkningType.Create(fromDate, toDate)
                };
        }
        //TODO: Add fromDate and to parameters
        public static PersonRelationType[]CreateList(params Guid[] targetUuids)
        {
            return Array.ConvertAll<Guid, PersonRelationType>
            (
                targetUuids,
                (uuid) => Create(uuid,null,null)
            );
        }
    }
}
