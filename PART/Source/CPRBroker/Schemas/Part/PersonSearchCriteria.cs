using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part.Enums;

namespace CprBroker.Schemas.Part
{
    public class PersonSearchCriteria
    {
        public PersonNameStructureType Name { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? BirthDateFrom { get; set; }
        public DateTime? BirthDateTo { get; set; }

        public string CprNumber { get; set; }
        public string NationalityCountryCode { get; set; }

    }
}
