using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine.Queues;

namespace CprBroker.Providers.CPRDirect
{
    public class DbrBaseQueue : Engine.Queues.Queue<ExtractQueueItem>
    {
        public const int BaseQueueTypeId = 200;
        public const int TargetQueueTypeId = 201;
        
        public override ExtractQueueItem[] Process(ExtractQueueItem[] items)
        {
            var targetQueues = Queue.GetQueues<DbrBaseQueue>(TargetQueueTypeId);

            foreach (var q in targetQueues)
            {
                q.Enqueue(items);
            }
            return items;
        }
    }
}
