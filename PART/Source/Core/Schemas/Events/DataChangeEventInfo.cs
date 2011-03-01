using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Schemas.Part.Events
{
    /// <summary>
    /// Used by event broker to get information about data updates from Cpr Broker
    /// </summary>
    public class DataChangeEventInfo
    {
        public Guid EventId { get; set; }
        public Guid PersonUuid { get; set; }
        public DateTime ReceivedDate { get; set; }
    }
}
