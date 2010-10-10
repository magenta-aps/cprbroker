using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public class PersonSearchCriteria
    {
        public string Name { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }

        public PersonData PersonData { get; set; }
    }
}
