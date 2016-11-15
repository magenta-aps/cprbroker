using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Engine.Queues;
using CprBroker.Data.Queues;
using CprBroker.Schemas;
using CprBroker.Engine;

namespace CprBroker.PartInterface.Tracking
{
    public class CleanupQueue : Engine.Queues.Queue<CleanupQueueItem>
    {
        public override CleanupQueueItem[] Process(CleanupQueueItem[] items)
        {
            var prov = new TrackingDataProvider();
            var uuids = items.Select(i => i.PersonUuid).ToArray();
            var maximumUsageDate = DateTime.Now;
            var minimumUsageDate = maximumUsageDate - CleanupDetectionEnqueuer.MaxInactivePeriod;
            var minimumUsageDatePlusDprAllowance = minimumUsageDate + CleanupDetectionEnqueuer.DprEmulationRemovalAllowance;
            var states = prov.GetStatus(uuids, minimumUsageDate, maximumUsageDate);
            var brokerContext = BrokerContext.Current;

            var tasks = items.Zip(
                states,
                (queueItem, personStatus) => ProcessAsync(brokerContext, prov, queueItem, personStatus, minimumUsageDatePlusDprAllowance)
                );

            return Task.WhenAll(tasks)
                .Result
                .Where(r => r != null)
                .ToArray();
        }

        public async Task<CleanupQueueItem> ProcessAsync(BrokerContext brokerContext, TrackingDataProvider prov, CleanupQueueItem queueItem, PersonTrack personTrack, DateTime dprAllowance)
        {
            BrokerContext.Current = brokerContext;
            if (personTrack.IsEmpty())
            {
                try
                {
                    // remove person
                    var personRemoved = await prov.RemovePersonAsync(queueItem.ToPersonIdentifier());
                    if (personRemoved)
                        return queueItem;
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    CprBroker.Engine.Local.Admin.LogException(ex);
                    return null;
                }
            }
            else if (personTrack.IsEmptyAfter(dprAllowance))
            {
                // Only remove from DPR emulation
                var dbrRemoved = await prov.DeletePersonFromAllDBR(brokerContext, queueItem.ToPersonIdentifier());
                if (dbrRemoved)
                    return queueItem;
                else
                    return null;
            }
            else
            {
                // Person should not be removed - also considered a success
                return queueItem;
            }
        }
    }    
}
