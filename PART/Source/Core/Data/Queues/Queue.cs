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
            return this.QueueItems
                .Where(qi => qi.AttemptCount < this.MaxRetry)
                .OrderBy(qi => qi.QueueItemId)
                .Take(maxCount);
        }

        public void Remove(IEnumerable<QueueItem> items)
        {
            using (var dataContext = new QueueDataContext())
            {
                dataContext.QueueItems.DeleteAllOnSubmit(items);
                dataContext.SubmitChanges();
            }
        }

        public void Enqueue(IEnumerable<string> itemKeys)
        {
            var items = itemKeys.Select(ik => new QueueItem(ik, this));
            using (var dataContext = new QueueDataContext())
            {
                dataContext.QueueItems.InsertAllOnSubmit(items);
                dataContext.SubmitChanges();
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
                dataContext.SubmitChanges();
            }
        }

        public virtual IEnumerable<QueueItem> Handle(IEnumerable<QueueItem> items)
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            var items = GetNext(BatchSize);
            while (items.FirstOrDefault() != null)
            {
                var succeeded = Handle(items);
                Remove(succeeded);
                var failedItems = items.Except(succeeded);
                foreach (var failedItem in failedItems)
                {
                    failedItem.AttemptCount++;
                }
                using (var dataContext = new QueueDataContext())
                {
                    dataContext.QueueItems.AttachAll(failedItems);
                    dataContext.SubmitChanges();
                }
                items = GetNext(BatchSize);
            }
        }


    }
}
