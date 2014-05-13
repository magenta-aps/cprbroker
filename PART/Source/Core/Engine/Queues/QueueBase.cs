using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Queues;

namespace CprBroker.Engine.Queues
{
    public abstract class QueueBase
    {
        public CprBroker.Data.Queues.Queue Impl { get; internal set; }
        public abstract void Run();

        public static QueueBase ToQueue(Queue impl)
        {
            var ret = CprBroker.Utilities.Reflection.CreateInstance<QueueBase>(impl.TypeName);
            if (ret != null)
            {
                ret.Impl = impl;
                if (ret is IHasConfigurationProperties)
                {
                    (ret as IHasConfigurationProperties).FillFromEncryptedStorage(ret.Impl);
                }
            }
            return ret;
        }

        public static TQueue[] GetQueues<TQueue>()
            where TQueue : class
        {
            using (var dataContext = new QueueDataContext())
            {
                return dataContext.Queues
                    .ToArray()
                    .Select(q => QueueBase.ToQueue(q) as TQueue)
                    .Where(q => q != null)
                    .ToArray();
            }
        }

        public static TQueue[] GetQueues<TQueue>(int typeId)
            where TQueue : QueueBase, new()
        {
            using (var dataContext = new QueueDataContext())
            {
                return dataContext.Queues
                    .Where(q => q.TypeId.HasValue && q.TypeId.Value == typeId)
                    .ToArray()
                    .Select(q => new TQueue() { Impl = q })
                    .ToArray();
            }
        }

        public static TQueue AddQueue<TQueue>(int queueTypeId, int batchSize, int maxRetry, Action<TQueue> initializer)
            where TQueue : QueueBase, new()
        {
            var ret = new TQueue();
            ret.Impl = new Queue()
            {
                QueueId = Guid.NewGuid(),
                BatchSize = batchSize,
                MaxRetry = maxRetry,
                TypeId = queueTypeId,
                TypeName = typeof(TQueue).AssemblyQualifiedName,
                
                Attributes = new List<Schemas.AttributeType>()
            };

            if(initializer!=null)
            {
                initializer(ret);
            }

            using (var dataContext = new QueueDataContext())
            {
                dataContext.Queues.InsertOnSubmit(ret.Impl);
                dataContext.SubmitChanges();
            }

            return ret;
        }

    }
}
