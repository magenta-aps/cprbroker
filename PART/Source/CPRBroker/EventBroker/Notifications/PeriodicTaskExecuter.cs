using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CprBroker.EventBroker.Notifications
{
    public partial class PeriodicTaskExecuter : Component
    {
        public EventLog EventLog = null;

        public PeriodicTaskExecuter()
        {
            InitializeComponent();
            InitializeTimer();
        }

        public PeriodicTaskExecuter(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            ActionTimer.AutoReset = true;
            ActionTimer.Elapsed += new System.Timers.ElapsedEventHandler(ActionTimer_Elapsed);
        }

        public void Start()
        {
            SetActionTimerInterval();
            ActionTimer.Start();
        }

        public void Stop()
        {
            ActionTimer.Stop();
        }

        void SetActionTimerInterval()
        {
            ActionTimer.Interval = this.CalculateActionTimerInterval(TimeSpan.FromMilliseconds(ActionTimer.Interval)).TotalMilliseconds;
        }

        void ActionTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (EventLog != null)
            {
                this.EventLog.WriteEntry(string.Format("{0} : {1}", CprBroker.Engine.TextMessages.TimerEventStarted, this.GetType()));
            }

            ActionTimer.Interval = this.CalculateActionTimerInterval(TimeSpan.FromMilliseconds(ActionTimer.Interval)).TotalMilliseconds;
            
            try
            {
                PerformTimerAction();
            }
            catch (Exception ex)
            {
                CprBroker.Engine.Local.Admin.LogException(ex);
            }
            
            if (EventLog != null)
            {
                this.EventLog.WriteEntry(string.Format("{0} : {1}", CprBroker.Engine.TextMessages.TimerEventFinished, this.GetType()));
            }
        }

        protected virtual TimeSpan CalculateActionTimerInterval(TimeSpan currentInterval)
        {
            return currentInterval;
        }

        protected virtual void PerformTimerAction()
        {

        }

    }
}
