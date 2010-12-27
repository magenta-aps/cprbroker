
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part.Events
{
    public class EventObjectStructureType
    {
        public Uri ObjectTypeReference { get; set; }
        public Uri EventObjectReference { get; set; }
        public Uri actionSchemeReference { get; set; }
        public String EventObjectActionCode { get; set; }
    }
}
