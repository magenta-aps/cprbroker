using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part
{
    public abstract class Registration
    {
        public DateTime RegistrationDate { get; set; }
        public Guid ActorId { get; set; }
    }
}
