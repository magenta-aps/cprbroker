using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public abstract class ContactChannel
    {
        public string LimitedUse { get; set; }
        public string Note { get; set; }
    }
}
