using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data;
using CprBroker.Data.Queues;

namespace CprBroker.Engine.Queues
{
    public abstract class QueueBase : IHasConfigurationProperties
    {
        public CprBroker.Data.Queues.Queue Impl { get; internal set; }

        public virtual Engine.DataProviderConfigPropertyInfo[] ConfigurationKeys { get { return new DataProviderConfigPropertyInfo[] { }; } }
        public Dictionary<string, string> ConfigurationProperties { get; set; }

        public abstract void Run();

        public static QueueBase ToQueue(Queue impl)
        {
            var ret = CprBroker.Utilities.Reflection.CreateInstance<QueueBase>(impl.TypeName);
            if (ret != null)
            {
                ret.Impl = impl;
                ret.FillFromEncryptedStorage(ret.Impl);
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

        public static TQueue GetById<TQueue>(Guid queueId)
            where TQueue : QueueBase
        {
            var db = Queue.GetById(queueId);
            return ToQueue(db) as TQueue;
        }

        public static TQueue AddQueue<TQueue>(int queueTypeId, Dictionary<string, string> values, int batchSize, int maxRetry)
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
            if (ret is IHasConfigurationProperties)
            {
                (ret as IHasConfigurationProperties).ConfigurationProperties = values;
                (ret as IHasConfigurationProperties).CopyToEncryptedStorage(ret.Impl);
            }

            using (var dataContext = new QueueDataContext())
            {
                dataContext.Queues.InsertOnSubmit(ret.Impl);
                dataContext.SubmitChanges();
            }

            return ret;
        }


        public static void UpdateAttributesById(Guid queueId, Dictionary<string, string> props)
        {
            using (var dataContext = new QueueDataContext())
            {
                var db = dataContext.Queues.Single(dbq => dbq.QueueId == queueId);
                db.SetAll(props);

                dataContext.SubmitChanges();
            }
        }

        public static void DeleteById(Guid queueId)
        {
            using (var dataContext = new QueueDataContext())
            {
                var db = dataContext.Queues.Single(dbq => dbq.QueueId == queueId);
                dataContext.Queues.DeleteOnSubmit(db);
                dataContext.SubmitChanges();
            }
        }

        public Guid QueueId
        {
            get { return Impl.QueueId; }
        }
    }
}
