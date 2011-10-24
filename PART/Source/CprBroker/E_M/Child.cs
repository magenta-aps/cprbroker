using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;

namespace CprBroker.Providers.E_M
{
    public partial class Child
    {
        public static PersonFlerRelationType ToPersonFlerRelationType(Child child, Func<string, Guid> cpr2uuidFunc)
        {
            if (child != null)
            {
                return PersonFlerRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(child.PNR)), null, null);
            }
            return null;
        }

        
    }
}
