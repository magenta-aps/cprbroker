using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas;

namespace CPRBroker.Providers.DPR
{
    public partial class Child
    {
        public ChildRelationshipType ToChildRelationship(PersonName personName)
        {
            return new ChildRelationshipType()
            {
                SimpleCPRPerson = personName.ToSimpleCprPerson()
            };
        }

    }
}
