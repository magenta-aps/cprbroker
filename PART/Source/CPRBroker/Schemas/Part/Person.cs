using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public class Person : PartObject
    {
        public PersonRegistration[] Registrations { get; set; }
    }
}
