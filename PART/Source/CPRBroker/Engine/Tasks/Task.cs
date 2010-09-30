using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CPRBroker.Engine.Tasks
{
    public class TaskPriority
    {
        public static readonly int Highest = 5;
        public static readonly int High = 4;
        public static readonly int Normal = 3;
        public static readonly int Low = 2;
        public static readonly int Lowest = 1;
    }

    public enum RunTaskResult
    {
        Success = 1,
        Failed = 3,
    }

    public class Task
    {

        public Object Result;
        public Object Input;
        public Task[] PreRequisites;
        public int Priority = TaskPriority.Normal;
        public bool IsDone = false;
        public DateTime StartTime { get; internal set; }
        public DateTime FinishTime { get; internal set; }
        public TimeSpan Duration
        {
            get
            {
                return FinishTime - StartTime;
            }
        }

        public Task()
        {

        }

        public Task(object input)
            : this(input, null)
        {

        }

        public Task(Task[] preRequisites)
            : this(null, preRequisites)
        {

        }

        public Task(object input, Task[] preRequisites)
            : this(input, preRequisites, TaskPriority.Normal)
        {

        }

        public Task(object input, Task[] preRequisites, int priority)
        {
            Input = input;
            this.PreRequisites = preRequisites;
            Priority = priority;
        }

        public virtual string HelpText
        {
            get
            {
                return GetType().ToString();
            }
        }

        public virtual void UpdateDisplay()
        { }

        public virtual bool Run()
        {
            return true;
        }

        internal static RunTaskResult RunTask(Task task, TaskQueue taskQueue)
        {
            if (task == null)
            {
                return RunTaskResult.Failed;
            }

            bool dataLoaded = false;

            try
            {
                task.StartTime = DateTime.Now;
                dataLoaded = task.Run();
            }
            catch
            {
            }

            if (dataLoaded)
            {
                Action updateDelegate = () =>
                    {
                        try
                        {
                            task.UpdateDisplay();
                        }
                        catch (Exception ex)
                        {
                            Object o = ex;
                        }
                    };
                Form activeForm = System.Windows.Forms.Form.ActiveForm;
                if (activeForm != null && activeForm.InvokeRequired)
                {
                    activeForm.Invoke(updateDelegate);
                }
                else
                {
                    updateDelegate();
                }
                return task.EndTask(RunTaskResult.Success);
            }
            task.IsDone = true;

            return task.EndTask(RunTaskResult.Failed);
        }

        public RunTaskResult EndTask(RunTaskResult result)
        {
            IsDone = true;
            FinishTime = DateTime.Now;
            Console.WriteLine(string.Format("Task: {0}, Duration: {1}", HelpText, Duration));
            return result;
        }
    }
}
