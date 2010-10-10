using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public class PersonRegistration : Registration
    {
        public PersonAttributes Attributes { get; set; }
        public PersonStates States { get; set; }
        public PersonRelations Relations { get; set; }
    }
}
