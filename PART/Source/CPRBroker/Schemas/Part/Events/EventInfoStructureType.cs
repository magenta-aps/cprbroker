using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part.Events
{
    public class EventInfoStructureType
    {
        public Uri EventIdentifier { get; set; }
        public Uri EventProducerReference { get; set; }
        public DateTime EventRegistrationDateTime { get; set; }
        public EventObjectStructureType EventObjectStructure { get; set; }
        public ExtensionStructureType ExtensionStructure { get; set; }
    }
}
