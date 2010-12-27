using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part.Enums;

namespace CprBroker.Schemas.Part
{
    public class PersonAttributes
    {
        public Effect<string> Name { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }

        public PersonData PersonData { get; set; }

        /// <summary>
        /// Channels through which the person can be contacted
        /// Out of CPR Broker context, always empty
        /// </summary>
        public ContactChannel[] ContactChannel { get; set; }
        /// <summary>
        /// Addresses other than the regitered address
        /// Cannot be filled from data providers
        /// Out of CPR Broker context, always empty
        /// </summary>
        public Address[] OtherAddresses { get; set; }
    }
}
