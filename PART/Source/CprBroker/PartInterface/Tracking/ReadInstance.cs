using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    public class ReadInstance
    {
        public Guid ApplicationId { get; set; }
        public DateTime ReadTime { get; set; }
    }
}
