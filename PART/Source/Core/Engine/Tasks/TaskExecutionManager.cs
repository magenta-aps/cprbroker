using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Engine.Tasks
{
    public abstract class TaskExecutionManager<T, TComparer> : PeriodicTaskExecuter
        where TComparer : IEqualityComparer<T>, new()
    {
        protected List<T> CurrentTasks = new List<T>();

        public T[] GetCurrentTaskExecuters()
        {
            return CurrentTasks.ToArray();
        }

        protected override void PerformTimerAction()
        {
            SyncTasks();
        }

        public override void OnAfterStop()
        {
            foreach (var q in CurrentTasks)
            {
                DisposeTask(q);
            }
        }

        public abstract void StartTask(T task);

        public virtual void DisposeTask(T task)
        {
            if (task != null && task is IDisposable)
            {
                (task as IDisposable).Dispose();
            }
        }

        public virtual T[] GetTasks()
        {
            return new T[] { };
        }

        public virtual void RefreshTask(T existingTask, T freshTask)
        {
        }

        public void SyncTasks()
        {
            var dbTasks = GetTasks();
            var runningTasks = CurrentTasks.ToArray();

            var comparer = new TComparer();
            var tasksToAdd = dbTasks.Except(runningTasks, comparer);
            var tasksToRemove = runningTasks.Except(dbTasks, comparer);

            foreach (var task in tasksToAdd)
            {
                StartTask(task);
                CurrentTasks.Add(task);
            }

            foreach (var task in tasksToRemove)
            {
                DisposeTask(task);
                CurrentTasks.Remove(task);
            }

            var existingTasks = from ex in runningTasks
                                from db in dbTasks
                                where comparer.Equals(ex, db)
                                select new { Existing = ex, Fresh = db };

            foreach (var pair in existingTasks)
            {
                RefreshTask(pair.Existing, pair.Fresh);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && this.CurrentTasks != null)
            {
                foreach (var task in this.CurrentTasks)
                {
                    if (task != null)
                        DisposeTask(task);
                }
            }
        }
    }
}
