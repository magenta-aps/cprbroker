using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Queues;
using CprBroker.Engine.Queues;


namespace CprBroker.EventBroker.Notifications
{
    public class QueueExecuter : PeriodicTaskExecuter
    {
        public QueueBase Queue { get; set; }

        public QueueExecuter(QueueBase queue)
        {
            this.Queue = queue;
        }

        public static QueueExecuter[] GetQueues()
        {
            using (var dataContext = new QueueDataContext())
            {
                return QueueBase.GetQueues<QueueBase>()
                    .Where(q => q != null)
                    .Select(q => new QueueExecuter(q))
                    .ToArray();
            }
        }

        protected override TimeSpan CalculateActionTimerInterval(TimeSpan currentInterval)
        {
            return base.CalculateActionTimerInterval(currentInterval);
        }

        protected override void PerformTimerAction()
        {
            this.Queue.Run();
        }
    }
}
