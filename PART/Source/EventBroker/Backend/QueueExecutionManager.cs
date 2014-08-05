using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.EventBroker.Notifications;

namespace CprBroker.EventBroker.Backend
{
    public class QueueExecutionManager : PeriodicTaskExecuter
    {
        private System.Threading.ReaderWriterLock CurrentTaskExecuters_Lock = new System.Threading.ReaderWriterLock();
        private List<QueueExecuter> CurrentTaskExecuters = new List<QueueExecuter>();

        protected override TimeSpan CalculateActionTimerInterval(TimeSpan currentInterval)
        {
            return base.CalculateActionTimerInterval(currentInterval);
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
            try
            {
                CurrentTaskExecuters_Lock.AcquireWriterLock(1);
                foreach (var q in CurrentTaskExecuters)
                {
                    q.Stop();
                    CurrentTaskExecuters.Remove(q);
                }
            }
            finally
            {
                CurrentTaskExecuters_Lock.ReleaseLock();
            }
        }


        public void SyncTasks()
        {
            var dbQueueTasks = QueueExecuter.GetQueues();
            var runningQueueTasks = CurrentTaskExecuters.Select(t => t as QueueExecuter).ToArray();

            var comparer = new QueueExecuterComparer();
            var queueTasksToAdd = dbQueueTasks.Except(runningQueueTasks, comparer);
            var queueTasksToRemove = runningQueueTasks.Except(dbQueueTasks, comparer);

            try
            {
                CurrentTaskExecuters_Lock.AcquireWriterLock(1);
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
                    CurrentTaskExecuters.Remove(qt);
                }
            }
            finally
            {
                CurrentTaskExecuters_Lock.ReleaseWriterLock();
            }
        }
    }
}
