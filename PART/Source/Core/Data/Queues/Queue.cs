using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Queues
{
    public partial class Queue
    {
        private IEnumerable<QueueItem> GetNext(int maxCount)
        {
            return this.QueueItems.OrderBy(qi => qi.QueueItemId).Take(maxCount);
        }

        public void Distribute(IEnumerable<Queue> targetQueues, int maxCount)
        {
            using (var dataContext = new QueueDataContext())
            {
                var items = this.GetNext(maxCount);

                foreach (var q in targetQueues)
                {
                    q.QueueItems.AddRange(items.Select(i => i.Clone(q)));
                }

                foreach (var i in items)
                {
                    this.QueueItems.Remove(i);
                }

                using (var trans = dataContext.Connection.BeginTransaction())
                {
                    dataContext.SubmitChanges();
                    trans.Commit();
                }
            }
            
        }

    }
}
