using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Queues;

namespace CprBroker.Engine.Queues
{
    public abstract class QueueItemBase : IQueueItem
    {
        public DbQueueItem Impl { get; set; }
        public abstract string SerializeToKey();
        public abstract void DeserializeFromKey(string key);
    }
}
