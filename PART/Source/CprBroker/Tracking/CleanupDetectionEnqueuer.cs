using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Engine.Tasks;
using CprBroker.Engine.Queues;
using CprBroker.Schemas;

namespace CprBroker.PartInterface.Tracking
{
    public class CleanupDetectionEnqueuer : PeriodicTaskExecuter
    {
        public static readonly TimeSpan MaxInactivePeriod = TimeSpan.FromDays(90);
        public static readonly TimeSpan DprEmulationRemovalAllowance = TimeSpan.FromDays(7);

        protected override void PerformTimerAction()
        {
            var startIndex = 0;
            var foundUuids = new PersonIdentifier[0];
            var maximumUsageDate = DateTime.Now;
            var minimumUsageDate = maximumUsageDate - MaxInactivePeriod;
            var cleanupQueue = Queue.GetQueues<CleanupQueue>().First();

            do
            {
                var prov = new TrackingDataProvider();
                if (LogTimerEvents)
                    CprBroker.Engine.Local.Admin.LogFormattedSuccess("{0}: checking persons {1} to {2}", GetType().Name, startIndex, startIndex + BatchSize);
                foundUuids = prov.EnumeratePersons(startIndex, BatchSize);

                var queueItems = foundUuids
                    .Select(uuid => new CleanupQueueItem() { PersonUuid = uuid.UUID.Value, PNR = uuid.CprNumber })
                    .ToArray();

                if (LogTimerEvents || foundUuids.Length > 0)
                    CprBroker.Engine.Local.Admin.LogFormattedSuccess("{0}: {1} persons enqueued for inspection <{2}>",
                        GetType().Name,
                        foundUuids.Length,
                        string.Join(",", foundUuids.Select(id => id.UUID))
                        );

                cleanupQueue.Enqueue(queueItems);
                startIndex += BatchSize;

            } while (foundUuids.Length < BatchSize);
        }

    }

}
