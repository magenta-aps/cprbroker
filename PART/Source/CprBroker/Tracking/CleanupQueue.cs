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
            var states = prov.GetStatus(uuids, minimumUsageDate, maximumUsageDate);
            var brokerContext = BrokerContext.Current;

            var tasks = items.Zip(
                states,
                (queueItem, personStatus) => ProcessAsync(brokerContext, prov, queueItem, personStatus)
                );

            return Task.WhenAll(tasks)
                .Result
                .Where(r => r != null)
                .ToArray();
        }

        public async Task<CleanupQueueItem> ProcessAsync(BrokerContext brokerContext, TrackingDataProvider prov, CleanupQueueItem queueItem, PersonTrack personTrack)
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
            else
            {
                // Person should not be removed - also considered a success
                return queueItem;
            }
        }
    }

    public class CleanupQueueItem : IQueueItem
    {
        public Guid PersonUuid { get; set; }
        public string PNR { get; set; }

        public DbQueueItem Impl { get; set; }

        public void DeserializeFromKey(string key)
        {
            var arr = key.Split('|');
            PersonUuid = new Guid(arr[0]);
            PNR = arr[1];
        }

        public string SerializeToKey()
        {
            return string.Format("{0}|{1}",
                PersonUuid,
                string.Format("{0}", PNR).PadLeft(10, '0')
                );
        }

        public PersonIdentifier ToPersonIdentifier()
        {
            return new PersonIdentifier()
            {
                UUID = PersonUuid,
                CprNumber = PNR,
            };
        }
    }
}
