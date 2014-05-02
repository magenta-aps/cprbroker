using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Queues
{

    public abstract class Queue<TQueueItem>
        where TQueueItem : IQueueItem, new()
    {
        public Queue Impl { get; private set; }


        public Queue(Guid queueId)
        {
            this.Impl = Queue.GetById(queueId);
        }

        public IEnumerable<TQueueItem> GetNext(int maxCount)
        {
            return Impl.GetNext(maxCount).Select(
                (qi) =>
                {
                    var ret = new TQueueItem() { Impl = qi };
                    ret.DeserializeFromKey(qi.ItemKey);
                    return ret;
                });
        }

        public void Remove(IEnumerable<TQueueItem> items)
        {
            Impl.Remove(items.Select(i => i.Impl));
        }

        public void Enqueue(IEnumerable<TQueueItem> items)
        {
            var itemKeys = items.Select(it => it.SerializeToKey());
            Impl.Enqueue(itemKeys);
        }

        /// <summary>
        /// Implements the actual task that is supposed to be implemented for each queue item
        /// Successful
        /// </summary>
        /// <param name="items">The queue items</param>
        /// <returns>A subset if the input that was processed successfully</returns>
        public virtual IEnumerable<TQueueItem> Handle(IEnumerable<TQueueItem> items)
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            var items = GetNext(Impl.BatchSize);
            while (items.FirstOrDefault() != null)
            {
                var succeeded = Handle(items);
                Remove(succeeded);
                var failedItems = items.Except(succeeded);
                foreach (var failedItem in failedItems)
                {
                    failedItem.Impl.AttemptCount++;
                }
                using (var dataContext = new QueueDataContext())
                {
                    dataContext.QueueItems.AttachAll(failedItems.Select(fi => fi.Impl));
                    dataContext.SubmitChanges();
                }
                items = GetNext(Impl.BatchSize);
            }
        }
    }

}