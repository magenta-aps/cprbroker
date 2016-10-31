using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBroker.Engine.Queues;
using CprBroker.Data.Queues;

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
            var ret = new List<CleanupQueueItem>();

            foreach (var person in items.Zip(states, (queueItem, personStatus) => new { queueItem, personStatus }))
            {
                if (person.personStatus.IsEmpty())
                {
                    // remove person
                    try
                    {
                        prov.RemovePerson(person.queueItem.PersonUuid);
                        ret.Add(person.queueItem);
                    }
                    catch (Exception ex)
                    {
                        CprBroker.Engine.Local.Admin.LogException(ex);
                    }
                }
                else
                {
                    // Person should not be removed - also considered a success
                    ret.Add(person.queueItem);
                }
            }
            return ret.ToArray();
        }
    }

    public class CleanupQueueItem : IQueueItem
    {
        public Guid PersonUuid { get; set; }

        public DbQueueItem Impl { get; set; }

        public void DeserializeFromKey(string key)
        {
            PersonUuid = new Guid(key);
        }

        public string SerializeToKey()
        {
            return PersonUuid.ToString();
        }
    }
}
