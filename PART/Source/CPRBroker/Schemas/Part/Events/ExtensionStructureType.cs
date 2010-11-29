using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part.Events
{
    public class ExtensionStructureType
    {
        public Guid ID { get; set; }
        public object[] Item { get; set; }
    }
}
