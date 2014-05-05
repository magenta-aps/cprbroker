using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Queues
{
    public abstract class QueueItemBase : IQueueItem
    {
        public QueueItem Impl { get; set; }
        public abstract string SerializeToKey();
        public abstract void DeserializeFromKey(string key);
    }
}
