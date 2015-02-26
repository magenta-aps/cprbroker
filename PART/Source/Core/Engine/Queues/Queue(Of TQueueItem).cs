using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Queues;

namespace CprBroker.Engine.Queues
{
    public abstract class Queue<TQueueItem> : Queue
        where TQueueItem : IQueueItem, new()
    {
        public Queue()
        {
        }

        public Queue(Guid queueId)
        {
            this.Impl = DbQueue.GetById(queueId);
        }

        public virtual TQueueItem[] GetNext(int maxCount)
        {
            return Impl.GetNext(maxCount)
                .Select(
                (qi) =>
                {
                    var ret = new TQueueItem() { Impl = qi };
                    ret.DeserializeFromKey(qi.ItemKey);
                    return ret;
                })
                .ToArray();
        }

        public virtual void Remove(TQueueItem[] items)
        {
            Impl.Remove(items.Select(i => i.Impl).ToArray());
        }

        public virtual void MarkFailure(TQueueItem[] items)
        {
            Impl.MarkFailure(items.Select(i => i.Impl).ToArray());
        }

        public void Enqueue(TQueueItem item, Semaphore semaphore = null)
        {
            Enqueue(new TQueueItem[] { item }, semaphore);
        }

        public virtual void Enqueue(TQueueItem[] items, Semaphore semaphore = null)
        {
            var itemKeys = items.Select(it => it.SerializeToKey()).ToArray();
            Impl.Enqueue(
                itemKeys,
                semaphore != null ? semaphore.Impl : null);
        }

        /// <summary>
        /// Implements the actual task that is supposed to be implemented for each queue item
        /// Successful
        /// </summary>
        /// <param name="items">The queue items</param>
        /// <returns>A subset if the input that was processed successfully</returns>
        public abstract TQueueItem[] Process(TQueueItem[] items);

        public override void RunAll()
        {
            var items = GetNext(Impl.BatchSize);
            while (items.FirstOrDefault() != null)
            {
                CprBroker.Engine.Local.Admin.LogFormattedSuccess("Queue <{0}><{1}>, Processing <{2}> items", GetType().Name, Impl.QueueId, items.Length);
                RunItems(items);
                items = GetNext(Impl.BatchSize);
            }
        }

        public override void RunOneBatch()
        {
            var items = GetNext(Impl.BatchSize);
            if (items.FirstOrDefault() != null)
            {
                RunItems(items);
            }
        }

        private void RunItems(TQueueItem[] items)
        {
            var succeeded = new TQueueItem[0];
            try
            {
                succeeded = Process(items);
            }
            catch (Exception ex)
            {
                CprBroker.Engine.Local.Admin.LogException(ex);
            }
            Remove(succeeded);

            var failedItems = items.Except(succeeded).ToArray();
            MarkFailure(failedItems);
        }
    }

}