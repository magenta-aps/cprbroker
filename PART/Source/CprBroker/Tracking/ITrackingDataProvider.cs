using CprBroker.Engine;
using CprBroker.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    public interface ITrackingDataProvider : IDataProvider
    {
        PersonTrack[] GetPersonUsage(Guid[] personUuids, DateTime? fromDate, DateTime? toDate);
        PersonTrack[] GetSubscribers(Guid[] personUuids);
        PersonTrack[] GetPersonUsageAndSubscribers(Guid[] personUuids, DateTime? fromDate, DateTime? toDate);
        PersonIdentifier[] EnumeratePersons(int startIndex = 0, int maxCount = 200);
    }
}
