using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public abstract class Address
    {
        public string Note { get; set; }
        public bool AddressUnknown { get; set; }
    }
}
