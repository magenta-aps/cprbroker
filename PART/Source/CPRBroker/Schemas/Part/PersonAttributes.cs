using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas.Part.Enums;

namespace CPRBroker.Schemas.Part
{
    public class PersonAttributes
    {
        public Effect<string> Name { get; set; }
        public Effect<Gender> Gender { get; set; }
        public DateTime BirthDate { get; set; }

        public Effect<PersonData> PersonData { get; set; }

        public Effect<ContactChannel>[] ContactChannel { get; set; }
        public Effect<Address>[] ContactAddresses { get; set; }
    }
}
