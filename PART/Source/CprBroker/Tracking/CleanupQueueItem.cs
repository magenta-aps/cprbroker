using CprBroker.Data.Queues;
using CprBroker.Engine.Queues;
using CprBroker.Schemas;
using CprBroker.Slet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.PartInterface.Tracking
{
    public class CleanupQueueItem : IQueueItem
    {
        public RemovePersonItem removePersonItem;

        public void DeserializeFromKey(string key)
        {
            var arr = key.Split('|');

            removePersonItem = new RemovePersonItem(new Guid(arr[0]), arr[1], arr[2].Equals("1"));
        }

        public string SerializeToKey()
        {
            return string.Format("{0}|{1}|{2}",
                removePersonItem.PersonUuid,
                string.Format("{0}", removePersonItem.PNR).PadLeft(10, '0'),
                removePersonItem.forceRemoval ? 1 : 0
                );
        }

        public DbQueueItem Impl { get; set; }
    }
}
