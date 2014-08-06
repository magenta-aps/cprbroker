using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.EventBroker.Notifications
{
    public class QueueExecutionManager : PeriodicTaskExecuter
    {
        private List<QueueExecuter> CurrentTaskExecuters = new List<QueueExecuter>();

        public QueueExecuter[] GetCurrentTaskExecuters()
        {
            return CurrentTaskExecuters.ToArray();
        }

        protected override TimeSpan CalculateActionTimerInterval(TimeSpan currentInterval)
        {
            return TimeSpan.FromSeconds(60);
        }

        protected override void PerformTimerAction()
        {
            SyncTasks();
        }

        protected override void OnBeforeStart()
        {
            base.OnBeforeStart();
        }

        public override void OnAfterStop()
        {
            foreach (var q in CurrentTaskExecuters)
            {
                q.Stop();
                CurrentTaskExecuters.Remove(q);
            }
        }


        public void SyncTasks()
        {
            var dbQueueTasks = QueueExecuter.GetQueues();
            var runningQueueTasks = CurrentTaskExecuters.ToArray();

            var comparer = new QueueExecuterComparer();
            var queueTasksToAdd = dbQueueTasks.Except(runningQueueTasks, comparer);
            var queueTasksToRemove = runningQueueTasks.Except(dbQueueTasks, comparer);

            foreach (var qt in queueTasksToAdd)
            {
                CprBroker.Engine.Local.Admin.LogSuccess(string.Format("Staring freshly loaded queue <{0}>", qt.Queue.QueueId));
                qt.Start();
                CurrentTaskExecuters.Add(qt);
            }

            foreach (var qt in queueTasksToRemove)
            {
                CprBroker.Engine.Local.Admin.LogSuccess(string.Format("Stopping queue <{0}>", qt.Queue.QueueId));
                qt.Stop();
                qt.Dispose();
                CurrentTaskExecuters.Remove(qt);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && this.CurrentTaskExecuters != null)
            {
                foreach (var q in this.CurrentTaskExecuters)
                {
                    if (q != null)
                        q.Dispose();
                }
            }
        }
    }
}
