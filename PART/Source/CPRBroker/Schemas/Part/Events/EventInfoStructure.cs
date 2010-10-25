using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part.Events
{
    public class EventInfoStructure
    {
        public Guid EventIdentifier { get; set; }
        public Guid EventProducerReference { get; set; }
        public DateTime EventRegistrationDateTime { get; set; }
        public object EventObjectStructure { get; set; }
        public object ExtensionStructure { get; set; }
    }
}
