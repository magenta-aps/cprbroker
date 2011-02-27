using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part.Events
{
    public class EventInfoStructureType
    {
        public string EventIdentifier { get; set; }
        public string EventProducerReference { get; set; }
        public DateTime EventRegistrationDateTime { get; set; }
        public EventObjectStructureType EventObjectStructure { get; set; }
        public ExtensionStructureType ExtensionStructure { get; set; }
    }
}
