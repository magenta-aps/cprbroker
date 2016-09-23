using CprBroker.Schemas;
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
        public ApplicationType[] Subscribers { get; set; }
        public ReadInstance[] ReadOperations { get; set; }
        public DateTime? LastRead { get; set; }

        public bool IsEmpty()
        {
            return
                (Subscribers == null || Subscribers.Length == 0)
                && (ReadOperations == null || ReadOperations.Length == 0)
                && LastRead == null
                ;
        }
    }
}
