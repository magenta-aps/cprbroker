using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part.Events
{
    public class CommonEventStructureType
    {
        public System.Security.Cryptography.Xml.Signature Signature { get; set; }
        public EventInfoStructureType EventInfoStructure { get; set; }
        public string EventTopic { get; set; }
        public string EventSubscriptionReference { get; set; }
        public ExtensionStructureType ExtensionStructure { get; set; }
        public EventDetailStructureType EventDetailStructure { get; set; }
    }
}
