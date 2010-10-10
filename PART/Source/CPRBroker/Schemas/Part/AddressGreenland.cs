using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public class AddressGreenland : Address
    {
        public AddressCompleteGreenlandType GreenlandicAddress { get; set; }
        public string SpecialStreetCode { get; set; }

    }
}
