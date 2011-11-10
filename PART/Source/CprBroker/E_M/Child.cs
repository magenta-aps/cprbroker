using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public partial class Child
    {
        public PersonFlerRelationType ToPersonFlerRelationType(Func<string, Guid> cpr2uuidFunc)
        {
            return PersonFlerRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(this.PNR)), null, null);
        }
    }
}
