using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Queues
{
    public partial class Queue
    {
        public IEnumerable<QueueItem> GetNext(int maxCount)
        {
            return this.QueueItems.OrderBy(qi => qi.QueueItemId).Take(maxCount);
        }

        public void Remove(IEnumerable<QueueItem> items)
        {
            using (var dataContext = new QueueDataContext())
            {
                dataContext.QueueItems.DeleteAllOnSubmit(items);
                using (var trans = dataContext.Connection.BeginTransaction())
                {
                    dataContext.Transaction = trans;
                    dataContext.SubmitChanges();
                    trans.Commit();
                }
            }
        }

        public void Enqueue(IEnumerable<string> itemKeys)
        {
            var items = itemKeys.Select(ik => new QueueItem(ik, this));
            using (var dataContext = new QueueDataContext())
            {
                dataContext.QueueItems.InsertAllOnSubmit(items);
                using (var trans = dataContext.Connection.BeginTransaction())
                {
                    dataContext.Transaction = trans;
                    dataContext.SubmitChanges();
                    trans.Commit();
                }
            }
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

                dataContext.QueueItems.DeleteAllOnSubmit(items);
                using (var trans = dataContext.Connection.BeginTransaction())
                {
                    dataContext.Transaction = trans;
                    dataContext.SubmitChanges();
                    trans.Commit();
                }
            }
            
        }

        

    }
}
