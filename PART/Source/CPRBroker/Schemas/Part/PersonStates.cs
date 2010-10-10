using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPRBroker.Schemas.Part.Enums;

namespace CPRBroker.Schemas.Part
{
    public class PersonStates
    {
        public Effect<LifeStatus> LifeStatus { get; set; }
        public Effect<MaritalStatus> CivilStatus { get; set; }
    }
}
