using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part.Events
{
    public class CommonEventStructure
    {
        public object Signature { get; set; }
        public EventInfoStructure EventInfoStructure { get; set; }
        public EventTopic EventTopic { get; set; }
        public Guid EventSubscriptionReference { get; set; }
        public object ExtensionStructure { get; set; }
        public object EventDetailStructure { get; set; }
    }
}
