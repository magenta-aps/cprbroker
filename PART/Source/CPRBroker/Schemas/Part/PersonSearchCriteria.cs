using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas.Part.Enums;

namespace CPRBroker.Schemas.Part
{
    public class PersonSearchCriteria
    {
        public PersonNameStructureType Name { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }

        public string CprNumber { get; set; }
        public string NationalityCountryCode { get; set; }

    }
}
