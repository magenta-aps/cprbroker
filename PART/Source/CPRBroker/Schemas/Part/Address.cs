using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    [System.Xml.Serialization.XmlInclude(typeof(AddressDenmark))]
    [System.Xml.Serialization.XmlInclude(typeof(AddressGreenland))]
    [System.Xml.Serialization.XmlInclude(typeof(AddressWorld))]
    public abstract class Address
    {
        public string Note { get; set; }
        public bool AddressUnknown { get; set; }
    }
}
