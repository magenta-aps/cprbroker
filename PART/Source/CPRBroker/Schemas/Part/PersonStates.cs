using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part.Enums;

namespace CprBroker.Schemas.Part
{
    public class PersonStates
    {
        public Effect<LifeStatus> LifeStatus { get; set; }
        public Effect<MaritalStatus> CivilStatus { get; set; }
    }
}
