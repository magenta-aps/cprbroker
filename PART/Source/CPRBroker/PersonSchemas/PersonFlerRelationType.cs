using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class PersonFlerRelationType
    {
        public static PersonFlerRelationType Create(Guid uuid)
        {
            return new PersonFlerRelationType()
            {
                //TODO: Add comment text
                CommentText = null,
                ReferenceIDTekst = uuid.ToString(),
                // TODO: Fill virkning object from parameters
                Virkning = VirkningType.Create()
            };
        }

        public static List<PersonFlerRelationType> CreateList(params Guid[] targetUuids)
        {
            return new List<PersonFlerRelationType>(
                Array.ConvertAll<Guid, PersonFlerRelationType>(
                    targetUuids,
                    (uuid) => Create(uuid)
                ));
        }
    }
}
