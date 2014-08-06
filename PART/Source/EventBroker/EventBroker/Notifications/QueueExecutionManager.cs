using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine.Queues;

namespace CprBroker.EventBroker.Notifications
{
    public class QueueExecutionManager : TaskExecutionManager<QueueExecuter, QueueExecuterComparer>
    {
        public override void StartTask(QueueExecuter task)
        {
            CprBroker.Engine.Local.Admin.LogSuccess(string.Format("Staring freshly loaded queue <{0}>", task.Queue.QueueId));
            task.Start();
        }

        public override void DisposeTask(QueueExecuter q)
        {
            CprBroker.Engine.Local.Admin.LogSuccess(string.Format("Stopping queue <{0}>", q.Queue.QueueId));
            q.Stop();
            q.Dispose();
        }

        public override QueueExecuter[] GetTasks()
        {
            return Queue.GetQueues<Queue>()
                .Where(q => q != null)
                .Select(q => new QueueExecuter(q))
                .ToArray();
        }

    }
}
