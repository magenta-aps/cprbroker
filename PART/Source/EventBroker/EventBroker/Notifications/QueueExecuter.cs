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
        public Queue Queue { get; set; }

        public QueueExecuter(Queue queue)
        {
            this.Queue = queue;
        }

        public static QueueExecuter[] GetQueues()
        {
            using (var dataContext = new QueueDataContext())
            {
                return Queue.GetQueues<Queue>()
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
            this.Queue.RunOneBatch();
        }
    }
}
