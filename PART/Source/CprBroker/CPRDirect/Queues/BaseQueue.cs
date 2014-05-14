using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine.Queues;

namespace CprBroker.Providers.CPRDirect
{
    public class BaseQueue : Engine.Queues.Queue<ExtractQueueItem>
    {
        public const int QueueId = 100;

        public override ExtractQueueItem[] Process(ExtractQueueItem[] items)
        {
            var dbr = QueueBase.GetQueues<DbrBaseQueue>(DbrBaseQueue.BaseQueueTypeId).First();
            var cpr = QueueBase.GetQueues<PartConversionQueue>().First();

            dbr.Enqueue(items);
            cpr.Enqueue(items);

            return items.ToArray();
        }
    }
}
