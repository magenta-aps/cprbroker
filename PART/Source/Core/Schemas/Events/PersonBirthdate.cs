using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part.Events
{
    /// <summary>
    /// Represents the birthdate of a person
    /// </summary>
    public class PersonBirthdate
    {
        public Guid PersonUuid { get; set; }
        public DateTime Birthdate { get; set; }
    }
}
