﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public class AddressWorld : Address
    {
        public ForeignAddressStructureType ForeignAddress { get; set; }
    }
}
