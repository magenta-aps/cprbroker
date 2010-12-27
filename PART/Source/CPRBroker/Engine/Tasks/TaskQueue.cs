using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CprBroker.Engine.Tasks
{
    public class TaskQueue
    {
        public static readonly TaskQueue Main = new TaskQueue();
        List<Task> Tasks;
        public int TaskCount { get; private set; }

        Object _WorkerThreadLock = new Object();
        private int _ThreadCount = 0;
        private bool _IsAThreadRunning = false;
        private AutoResetEvent _FinishEvent = new AutoResetEvent(false);

        public int MaxThreads = Config.Properties.Settings.Default.TaskQueueMaxThreads;

        public Task Initializer = null;
        public Task Finalizer = null;


        TaskQueue()
        {
            Tasks = new List<Task>();
        }

        internal Task GetNextTask()
        {
            lock (Tasks)
            {
                var candidateTask = (from task in Tasks
                                     where task.PreRequisites == null || Array.TrueForAll<Task>(task.PreRequisites, (pr) => pr == null || pr.IsDone)
                                     select task
                                )
                                .OrderByDescending((t) => t.Priority)
                                .FirstOrDefault();

                if (candidateTask != null)
                {
                    Tasks.Remove(candidateTask);
                    TaskCount--;
                }
                return candidateTask;
            }
        }

        public void Enqueue(Task newTask)
        {
            lock (Tasks)
            {
                newTask.Priority = Math.Max(newTask.Priority, TaskPriority.Highest);
                newTask.Priority = Math.Min(newTask.Priority, TaskPriority.Lowest);
                Tasks.Add(newTask);
                TaskCount++;
            }
            Start();
        }
        public void Enqueue(IEnumerable<Task> tasks)
        {
            lock (Tasks)
            {
                foreach (Task newTask in tasks)
                {
                    newTask.Priority = Math.Max(newTask.Priority, TaskPriority.Highest);
                    newTask.Priority = Math.Min(newTask.Priority, TaskPriority.Lowest);
                    Tasks.Add(newTask);
                    TaskCount++;
                }
            }
            Start();
        }

        public void Clear()
        {
            lock (Tasks)
            {
                Tasks.Clear();
            }
        }

        public void Start()
        {
            lock (_WorkerThreadLock)
            {
                int neededThreads = TaskCount - _ThreadCount;
                int maxAllowedNeededThreads = MaxThreads - _ThreadCount;
                neededThreads = Math.Min(neededThreads, maxAllowedNeededThreads);
                neededThreads = Math.Max(neededThreads, 1);

                for (int iThread = 0; iThread < neededThreads; iThread++)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(RunThread));
                    _ThreadCount++;
                }
            }
        }

        private void RunThread(object state)
        {
            BeginThreadExecution(Thread.CurrentThread);
            while (true)
            {
                Task nextTask = GetNextTask();
                if (nextTask == null)
                {
                    break;
                }
                else
                {
                    RunTaskResult result = Task.RunTask(nextTask, this);
                }
            }
            EndThreadExecution(Thread.CurrentThread);
        }

        internal void BeginThreadExecution(Thread currentThread)
        {
            lock (_WorkerThreadLock)
            {
                if (!_IsAThreadRunning)
                {
                    Task.RunTask(this.Initializer, this);
                    _IsAThreadRunning = true;
                }
            }
        }

        internal void EndThreadExecution(Thread currentThread)
        {
            lock (_WorkerThreadLock)
            {
                try
                {
                    if (_ThreadCount == 1)
                    {
                        Task.RunTask(this.Finalizer, this);
                    }
                }
                finally
                {
                    if (_ThreadCount == 1)
                    {
                        this._IsAThreadRunning = false;
                        _FinishEvent.Set();
                    }
                    _ThreadCount--;
                }
            }
        }

        public void WaitForFinish()
        {
            _FinishEvent.WaitOne();
        }
    }
}
