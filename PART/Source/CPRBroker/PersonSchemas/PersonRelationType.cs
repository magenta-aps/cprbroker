using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public partial class PersonRelationType
    {
        public static PersonRelationType Create(Guid targetUuid)
        {
            return new PersonRelationType()
                {
                    CommentText = null,
                    ReferenceIDTekst = targetUuid.ToString(),
                    Virkning = new VirkningType()
                    {
                        AktoerTekst = null,
                        CommentText = null,
                        FraTidspunkt = new TidspunktType()
                        {
                            //TODO : Xml element called either Tidsstempel:datetime or GraenseIndikator:bool
                            Item = null
                        },
                        TilTidspunkt = new TidspunktType()
                        {
                            //TODO : Xml element called either Tidsstempel:datetime or GraenseIndikator:bool
                            Item = null
                        }
                    }
                };
        }
        public static List<PersonRelationType> CreateList(params Guid[] targetUuids)
        {
            return new List<PersonRelationType>(
                Array.ConvertAll<Guid, PersonRelationType>(
                    targetUuids,
                    (uuid) => Create(uuid)
                ));
        }
    }
}
