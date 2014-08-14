using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Queues
{
    public partial class DbQueueItem
    {
        public DbQueueItem(string key, DbQueue queue, DbSemaphore semaphore)
        {
            this.QueueId = queue.QueueId;
            this.ItemKey = key;
            this.CreatedTS = DateTime.Now;
            if (semaphore != null)
                this.SemaphoreId = semaphore.SemaphoreId;
            AttemptCount = 0;
        }

        public DbQueueItem Clone(DbQueue newQueue)
        {
            return new DbQueueItem()
            {
                ItemKey = this.ItemKey,

                QueueId = newQueue.QueueId,
                CreatedTS = DateTime.Now,
                AttemptCount = 0,
            };
        }


    }
}
