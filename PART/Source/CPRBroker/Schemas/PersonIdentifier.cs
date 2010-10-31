using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas
{
    public class PersonIdentifier
    {
        public string  CprNumber;
        public DateTime Birthdate;

        //public Guid UUID;

        public PersonIdentifier()
        {
 
        }
        public PersonIdentifier(string cprNumber,DateTime birthdate)
        {
            CprNumber = cprNumber;
            Birthdate = birthdate;
        }
    }
}
