using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Engine.Tasks;
using CprBroker.Engine.Queues;
using CprBroker.Schemas;
using CprBroker.Engine;
using CprBroker.Data.Applications;

namespace CprBroker.PartInterface.Tracking
{
    public class CleanupDetectionEnqueuer : PeriodicTaskExecuter
    {
        public static TimeSpan MaxInactivePeriod
        {
            get
            {
                return Properties.Settings.Default
                    .MaxInactivePeriod
                    .Duration();
            }
        }

        public static TimeSpan DprEmulationRemovalAllowance
        {
            get
            {
                return Properties.Settings.Default
                    .DprEmulationRemovalAllowance
                    .Duration();
            }
        }

        public static string[] ExcludedMunicipalityCodes
        {
            get
            {
                return Properties.Settings.Default.ExcludedMunicipalityCodes
                    .Cast<string>()
                    .Select(v => string.Format("{0}", v).TrimStart('0', ' '))
                    .Where(v => !string.IsNullOrEmpty(v))
                    .ToArray();
            }
        }

        public bool CanRunCleanup(bool log = false)
        {
            var oldestOperationTS = Operation.OldestOperationTS();
            var warmingPeriodEndTS =
                (oldestOperationTS.HasValue ? oldestOperationTS.Value : DateTime.Now)
                + MaxInactivePeriod;

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
