using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part.Events
{
    public class EventObjectStructure
    {
        public object ObjectTypeReference { get; set; }
        public object EventObjectReference { get; set; }
        public object actionSchemeReference { get; set; }
        public object EventObjectActionCode { get; set; }
    }
}
