using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using CprBroker.Schemas;

namespace CprBroker.Data.Queues
{
    public partial class DbQueue : IHasEncryptedAttributes
    {
        public System.Security.Cryptography.RijndaelManaged EncryptionAlgorithm { get; set; }
        public List<AttributeType> Attributes { get; set; }

        partial void OnLoaded()
        {
            this.PreLoadAttributes();
        }

        public static DbQueue GetById(Guid queueId)
        {
            using (var dataContext = new QueueDataContext())
            {
                return dataContext.Queues
                    .Where(q => q.QueueId == queueId)
                    .FirstOrDefault();
            }
        }

        public DbQueueItem[] GetNext(int maxCount)
        {
            using (var dataContext = new QueueDataContext())
            {
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<DbQueueItem>(qi => qi.Semaphore);
                dataContext.LoadOptions = loadOptions;

                return dataContext.QueueItems
                    .Where(qi =>
                        qi.QueueId == this.QueueId
                        && qi.AttemptCount < this.MaxRetry
                        && (qi.Semaphore == null || qi.Semaphore.SignaledDate.HasValue))
                    .OrderBy(qi => qi.QueueItemId)
                    .Take(maxCount)
                    .ToArray();
            }
        }

        public void Remove(DbQueueItem[] items)
        {
            using (var dataContext = new QueueDataContext())
            {
                var ids = items.Select(it => it.QueueItemId).ToArray();
                var itemsToDelete = dataContext.QueueItems.Where(it => ids.Contains(it.QueueItemId));
                dataContext.QueueItems.DeleteAllOnSubmit(itemsToDelete);
                dataContext.SubmitChanges();
            }
        }

        public void MarkFailure(DbQueueItem[] items)
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

        public void Enqueue(string[] itemKeys, DbSemaphore semaphore)
        {
            var items = itemKeys.Select(ik => new DbQueueItem(ik, this, semaphore));
            using (var dataContext = new QueueDataContext())
            {
                dataContext.QueueItems.InsertAllOnSubmit(items);
                dataContext.SubmitChanges();
            }
        }

        public void MultiplyTo(IEnumerable<DbQueue> targetQueues, int maxCount)
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
