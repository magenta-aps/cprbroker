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

    }
}
