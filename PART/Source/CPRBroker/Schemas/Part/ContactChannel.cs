using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public abstract class ContactChannel
    {
        public string LimitedUse { get; set; }
        public string Note { get; set; }
    }
}
