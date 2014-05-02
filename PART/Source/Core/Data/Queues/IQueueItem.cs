using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Queues
{
    public interface IQueueItem
    {
        QueueItem Impl { get; set; }
        string SerializeToKey();
        void DeserializeFromKey(string key);
    }
}
