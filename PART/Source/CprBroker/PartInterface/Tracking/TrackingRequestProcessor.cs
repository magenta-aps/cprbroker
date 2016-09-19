using CprBroker.Data.Applications;
using CprBroker.Engine;
using CprBroker.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    public class TrackingRequestProcessor : RequestProcessor
    {
        public PersonTrack GetHistory(Guid[] personUuids, DateTime? fromDate, DateTime? toDate)
        {
            return null;
        }
    }
}
