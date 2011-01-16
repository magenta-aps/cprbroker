using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part.Events
{
    public class PersonBirthdate
    {
        public Guid PersonUuid { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
