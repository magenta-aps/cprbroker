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
                    ReferenceIDTekst = targetUuid.ToString(),
                    Virkning = VirkningType.Create(fromDate, toDate)
                };
        }
        //TODO: Add from and to parameters
        public static List<PersonRelationType> CreateList(params Guid[] targetUuids)
        {
            return new List<PersonRelationType>(
                Array.ConvertAll<Guid, PersonRelationType>(
                    targetUuids,
                    (uuid) => Create(uuid,null,null)
                ));
        }
    }
}
