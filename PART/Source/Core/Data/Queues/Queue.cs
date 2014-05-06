using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Queues
{
    public partial class Queue
    {
        public static Queue GetById(Guid queueId)
        {
            using (var dataContext = new QueueDataContext())
            {
                return dataContext.Queues
                    .Where(q => q.QueueId == queueId)
                    .FirstOrDefault();
            }
        }

        public QueueItem[] GetNext(int maxCount)
        {
            using (var dataContext = new QueueDataContext())
            {
                return dataContext.QueueItems
                    .Where(qi => qi.QueueId == this.QueueId && qi.AttemptCount < this.MaxRetry)
                    .OrderBy(qi => qi.QueueItemId)
                    .Take(maxCount)
                    .ToArray();
            }
        }

        public void Remove(QueueItem[] items)
        {
            using (var dataContext = new QueueDataContext())
            {
                var ids = items.Select(it => it.QueueItemId).ToArray();
                var itemsToDelete = dataContext.QueueItems.Where(it => ids.Contains(it.QueueItemId));
                dataContext.QueueItems.DeleteAllOnSubmit(itemsToDelete);
                dataContext.SubmitChanges();
            }
        }

        public void MarkFailure(QueueItem[] items)
        {
            using (var dataContext = new QueueDataContext())
            {
                var ids = items.Select(it => it.QueueItemId).ToArray();
                var itemsToMark = dataContext.QueueItems.Where(it => ids.Contains(it.QueueItemId));
                foreach (var item in itemsToMark)
                {
                    item.AttemptCount++;
                }
                dataContext.SubmitChanges();
            }
        }

        public void Enqueue(string[] itemKeys)
        {
            var items = itemKeys.Select(ik => new QueueItem(ik, this));
            using (var dataContext = new QueueDataContext())
            {
                dataContext.QueueItems.InsertAllOnSubmit(items);
                dataContext.SubmitChanges();
            }
        }

        public void MultiplyTo(IEnumerable<Queue> targetQueues, int maxCount)
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



    }
}
