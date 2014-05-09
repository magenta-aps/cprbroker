using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Queues
{
    public abstract class QueueBase
    {
        public Queue Impl { get; internal set; }
        public abstract void Run();

        public static QueueBase ToQueue(Queue impl)
        {
            var ret = CprBroker.Utilities.Reflection.CreateInstance<QueueBase>(impl.TypeName);
            if (ret != null)
            {
                ret.Impl = impl;
            }
            return ret;
        }

        public static TQueue[] GetQueues<TQueue>()
            where TQueue : class
        {
            using (var dataContext = new QueueDataContext())
            {
                return dataContext.Queues
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

    }
}
