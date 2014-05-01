using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Queues
{
    public partial class QueueItem
    {
        public QueueItem Clone(Queue newQueue)
        {
            return new QueueItem() { 
                ItemKey = this.ItemKey,
                
                QueueId = newQueue.QueueId,
                CreatedTS = DateTime.Now,                
            };
        }
    }
}
