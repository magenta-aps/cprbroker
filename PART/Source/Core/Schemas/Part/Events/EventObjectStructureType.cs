
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part.Events
{
    public class EventObjectStructureType
    {
        public string ObjectTypeReference { get; set; }
        public string EventObjectReference { get; set; }
        public string actionSchemeReference { get; set; }
        public String EventObjectActionCode { get; set; }
    }
}
