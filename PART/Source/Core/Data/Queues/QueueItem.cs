using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Queues
{
    public partial class QueueItem
    {
        public QueueItem(string key, Queue queue)
        {
            this.QueueId = queue.QueueId;
            this.ItemKey = key;
            this.CreatedTS = DateTime.Now;
            AttemptCount = 0;
        }
        
        public QueueItem Clone(Queue newQueue)
        {
            return new QueueItem() { 
                ItemKey = this.ItemKey,
                
                QueueId = newQueue.QueueId,
                CreatedTS = DateTime.Now,                
                AttemptCount = 0,
            };
        }

        
    }
}
