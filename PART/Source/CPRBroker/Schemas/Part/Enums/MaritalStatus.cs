using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part.Enums
{
    public enum MaritalStatus
    {
        single,
        married,
        divorced,
        widow,
        registeredpartner,
        repealedpartnership,
        surviving,
        // TODO: Verify that deceased conforms to the standard, because it is not in the specs
        deceased,
    }
}
