using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public class AddressWorld : Address
    {
        public ForeignAddressStructureType ForeignAddress { get; set; }
    }
}
