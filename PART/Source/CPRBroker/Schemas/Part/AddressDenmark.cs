using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public class AddressDenmark : Address
    {
        public AddressCompleteType AddressComplete { get; set; }
        public AddressPointType AddressPoint { get; set; }
        public string SpecialStreetCode { get; set; }
    }
}
