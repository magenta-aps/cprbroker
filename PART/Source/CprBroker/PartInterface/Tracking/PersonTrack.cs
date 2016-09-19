using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    public class PersonTrack
    {
        public Guid UUID { get; set; }
        public ApplicationId[] Subscribers { get; set; }
        public ReadInstance[] ReadOperations { get; set; }
        public DateTime? LastRead { get; set; }
    }    
}
