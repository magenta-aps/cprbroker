using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.CPRDirect
{
    public partial class ChildType
    {
        public PersonFlerRelationType ToPersonFlerRelationType(Func<string, Guid> cpr2uuidFunc)
        {
            return PersonFlerRelationType.Create(
                cpr2uuidFunc(this.ToChildPnr()),
                null,
                null
                );
        }

        public string ToChildPnr()
        {
            return Converters.ToPnrStringOrNull(this.ChildPNR);
        }

        public static PersonFlerRelationType[] ToPersonFlerRelationType(IList<ChildType> list, Func<string, Guid> cpr2uuidFunc)
        {
            return list
                .Select(ch => ch.ToPersonFlerRelationType(cpr2uuidFunc))
                .ToArray()
                ;
        }
    }
}
