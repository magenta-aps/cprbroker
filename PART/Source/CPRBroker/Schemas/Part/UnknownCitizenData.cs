using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public class UnknownCitizenData : PersonData
    {
        public string ReplacementCprNumber { get; set; }
    }
}
