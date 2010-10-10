using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas.Part.Enums;

namespace CPRBroker.Schemas.Part
{
    public class CprData : PersonData
    {
        public string FirstName { get; set; }
        public string MiddleNames { get; set; }
        public string SurNames { get; set; }
        public string NickName { get; set; }
        public string AddressingName { get; set; }
        public bool? NameAndAddressProtection { get; set; }
        public bool? BirthDateUncertainty { get; set; }
        public Gender Gender { get; set; }
        public string CprNumber { get; set; }
        public bool IndividualTrackStatus { get; set; }
        public string NationalityCountryCode { get; set; }
        public Address PopulationAddress { get; set; }
    }
}
