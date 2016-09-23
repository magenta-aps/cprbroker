using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Engine.Tasks;
using CprBroker.Engine.Queues;

namespace CprBroker.PartInterface.Tracking
{
    public class CleanupDetector : PeriodicTaskExecuter
    {
        public static readonly TimeSpan MaxInactivePeriod = TimeSpan.FromDays(90);

        protected override void PerformTimerAction()
        {
            var startIndex = 0;
            var foundUuids = new Guid[0];
            var maximumUsageDate = DateTime.Now;
            var minimumUsageDate = maximumUsageDate - MaxInactivePeriod;
            var cleanupQueue = Queue.GetQueues<CleanupQueue>().First();

            do
            {
                var prov = new TrackingDataProvider();
                if (LogTimerEvents)
                    CprBroker.Engine.Local.Admin.LogFormattedSuccess("{0}: checking persons {1} to {2}", GetType().Name, startIndex, startIndex + BatchSize);
                foundUuids = prov.EnumeratePersons(startIndex, BatchSize);
                var usage = prov.GetStatus(foundUuids, minimumUsageDate, maximumUsageDate);

                var queueItems = usage
                    .Where(u => u.IsEmpty())
                    .Select(u => new CleanupQueueItem() { PersonUuid = u.UUID })
                    .ToArray();

                if (LogTimerEvents || foundUuids.Length > 0)
                    CprBroker.Engine.Local.Admin.LogFormattedSuccess("{0}: {1} inactive persons found <{2}>", GetType().Name, foundUuids.Length, string.Join(",", foundUuids));

                cleanupQueue.Enqueue(queueItems);
                startIndex += BatchSize;

            } while (foundUuids.Length < BatchSize);
        }

    }

}
