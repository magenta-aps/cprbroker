using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Engine.Queues;
using CprBroker.Data.Queues;
using CprBroker.Schemas;
using CprBroker.Engine;
using CprBroker.Providers.Local.Search;
using CprBroker.Engine.Local;
using System.Threading;

namespace CprBroker.PartInterface.Tracking
{
    public class CleanupQueue : Engine.Queues.Queue<CleanupQueueItem>
    {
        public override CleanupQueueItem[] Process(CleanupQueueItem[] items)
        {
            var prov = new TrackingDataProvider();
            var uuids = items.Select(i => i.PersonUuid).ToArray();

            var brokerContext = BrokerContext.Current;

            var tasks = items
                .Select(
                (queueItem) => Task.Factory.StartNew(new Func<CleanupQueueItem>(() => ProcessItemWithMutex(brokerContext, prov, queueItem)))
                );

            return Task.WhenAll(tasks)
                .Result
                .Where(r => r != null)
                .ToArray();
        }

        public virtual CleanupQueueItem ProcessItemWithMutex(BrokerContext brokerContext, TrackingDataProvider prov, CleanupQueueItem queueItem)
        {
            BrokerContext.Current = brokerContext;
            Mutex personMutex = null;

            try
            {
                // Establish a person based critical section
                personMutex = new Mutex(false, queueItem.PersonUuid.ToString().ToUpper());
                personMutex.WaitOne();

                // Now the person is locked, all possible usage has been recorded                
                var fromDate = DateTime.Now - CleanupDetectionEnqueuer.MaxInactivePeriod;
                var dbrFromDate = fromDate + CleanupDetectionEnqueuer.DprEmulationRemovalAllowance;

                return ProcessItem(brokerContext, prov, queueItem, fromDate, dbrFromDate);
            }
            catch (Exception ex)
            {
                Admin.LogException(ex);
                return null;
            }
            finally
            {
                // Release the lock
                if (personMutex != null)
                    personMutex.ReleaseMutex();
            }
        }

        public virtual CleanupQueueItem ProcessItem(BrokerContext brokerContext, TrackingDataProvider prov, CleanupQueueItem queueItem, DateTime fromDate, DateTime dbrFromDate)
        {
            BrokerContext.Current = brokerContext;
            var personIdentifier = queueItem.ToPersonIdentifier();

            // First, make and log the decisions
            var decision = prov.GetRemovalDecision(personIdentifier, fromDate, dbrFromDate);

            // Action time
            // Remove the person if needed
            switch (decision)
            {
                case PersonRemovalDecision.RemoveCompletely:
                    Admin.LogFormattedSuccess("<{0}>:Removing unused person <{1}>", this.GetType().Name, personIdentifier.UUID);
                    var task1 = prov.RemovePersonAsync(personIdentifier);
                    task1.Wait();
                    var personRemoved = task1.Result;
                    if (personRemoved)
                        return queueItem;
                    else
                        return null;

                case PersonRemovalDecision.RemoveFromDprEmulation: // Only remove from DPR emulation                    
                    Admin.LogFormattedSuccess("<{0}>:Removing semi-unused person <{1}> from DPR emulation", this.GetType().Name, personIdentifier.UUID);
                    var task2 = prov.DeletePersonFromAllDBR(brokerContext, personIdentifier);
                    task2.Wait();
                    var dbrRemoved = task2.Result;
                    if (dbrRemoved)
                        return queueItem;
                    else
                        return null;

                case PersonRemovalDecision.DoNotRemoveDueToExclusion:
                    return queueItem;

                case PersonRemovalDecision.DoNotRemoveDueToUsage:
                    return queueItem;

                default:
                    throw new Exception(string.Format("Unknown value <{0}> for type <{1}>", decision, typeof(PersonRemovalDecision).Name));
            }
        }
    }
}
