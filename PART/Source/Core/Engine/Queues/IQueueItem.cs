using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Queues;

namespace CprBroker.Engine.Queues
{
    public interface IQueueItem
    {
        QueueItem Impl { get; set; }
        string SerializeToKey();
        void DeserializeFromKey(string key);
    }
}
