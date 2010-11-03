using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.Schemas.Part
{
    public class ForeignCitizenData : PersonData
    {
        public string ForeignNumber { get; set; }
        public string PermissionNumber { get; set; }
        public string NationalityCountryCode { get; set; }
    }
}
