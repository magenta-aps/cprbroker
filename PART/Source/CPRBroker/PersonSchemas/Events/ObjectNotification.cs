using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Events
{
    public class ObjectNotification
    {
        public Guid ObjectUuid { get; set; }
        public Guid SubscriptionId { get; set; }
    }
}
