using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Engine.Tasks;
using CprBroker.Engine.Queues;
using CprBroker.Schemas;
using CprBroker.Engine;
using CprBroker.Data.Applications;
using CprBroker.Slet;

namespace CprBroker.PartInterface.Tracking
{
    public class CleanupDetectionEnqueuer : PeriodicTaskExecuter
    {
        public bool CanRunCleanup(bool log = false)
        {
            var oldestOperationTS = Operation.OldestOperationTS();
            var warmingPeriodEndTS =
                (oldestOperationTS.HasValue ? oldestOperationTS.Value : DateTime.Now)
                + SettingsUtilities.MaxInactivePeriod;

            if (DateTime.Now > warmingPeriodEndTS)
            {
                if (log)
                    Engine.Local.Admin.LogFormattedSuccess("Earliest existing operation was at <{0}>. Proceeding with cleanup", oldestOperationTS);
                return true;
            }
            else
            {
                if (log)
                    Engine.Local.Admin.LogFormattedSuccess("Cleanup cannot be performed because it is still in the warmup period. Earliest existing operation was at <{0}>. Warmup ends at <{1}>",
                        oldestOperationTS,
                        warmingPeriodEndTS
                    );
                return false;
            }
        }

        protected override void PerformTimerAction()
        {
            // Register an operation
            BrokerContext.Current.RegisterOperation(CprBroker.Data.Applications.OperationType.Types.Generic, this.GetType().Name);

            // Cold-start protection 
            // Only perform cleanup if the system has been running for a minimum of the data storage limit (a.k.a 3 months)
            if (CanRunCleanup(log: true) == false)
            {
                return;
            }

            var startuuidAfter = null as Guid?;
            var foundUuids = new PersonIdentifier[0];
            var maximumUsageDate = DateTime.Now;
            var minimumUsageDate = maximumUsageDate - SettingsUtilities.MaxInactivePeriod;
            var cleanupQueue = Queue.GetQueues<CleanupQueue>().First();

            // Create a semaphore to block queue processing until all person UUID's have been enumerated
            var thisSemaphore = Semaphore.Create();

            do
            {
                var prov = new TrackingDataProvider();
                if (LogTimerEvents)
                    CprBroker.Engine.Local.Admin.LogFormattedSuccess("{0}: enqueuing first <{1}> persons with UUID greater than <{2}>", GetType().Name, BatchSize, startuuidAfter, BatchSize);
                foundUuids = prov.EnumeratePersons(startuuidAfter, BatchSize);

                var queueItems = foundUuids
                    .Select(uuid => new CleanupQueueItem() { removePersonItem = new RemovePersonItem(uuid) })
                    .ToArray();

                if (LogTimerEvents || foundUuids.Length > 0)
                    CprBroker.Engine.Local.Admin.LogFormattedSuccess("{0}: {1} persons enqueued for inspection <{2}>",
                        GetType().Name,
                        foundUuids.Length,
                        string.Join(",", foundUuids.Select(id => id.UUID))
                        );

                cleanupQueue.Enqueue(queueItems, thisSemaphore);
                startuuidAfter = foundUuids.LastOrDefault()?.UUID;
            } while (foundUuids.Length == BatchSize);

            // Now signal the semaphore
            thisSemaphore.Signal();
        }

    }

}
