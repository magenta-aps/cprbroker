﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.Utilities;

namespace CprBroker.EventBroker.Notifications
{
    /// <summary>
    /// Base class for running a task once whenever a period passes
    /// </summary>
    public partial class PeriodicTaskExecuter : Component
    {
        private System.Threading.AutoResetEvent SyncObject = new System.Threading.AutoResetEvent(true);

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

        protected virtual void OnBeforeStart()
        {
        }

        public void Start()
        {
            OnBeforeStart();
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
            if (SyncObject.WaitOne(1))
            {
                try
                {
                    BrokerContext.Initialize(Constants.EventBrokerApplicationToken.ToString(), Constants.UserToken);
                    CprBroker.Engine.Local.Admin.LogSuccess(string.Format("{0} : {1}", TextMessages.TimerEventStarted, this.GetType()));
                    ActionTimer.Interval = this.CalculateActionTimerInterval(TimeSpan.FromMilliseconds(ActionTimer.Interval)).TotalMilliseconds;

                    try
                    {
                        PerformTimerAction();
                    }
                    catch (Exception ex)
                    {
                        CprBroker.Engine.Local.Admin.LogException(ex);
                    }

                    CprBroker.Engine.Local.Admin.LogSuccess(string.Format("{0} : {1}", TextMessages.TimerEventFinished, this.GetType()));
                }
                finally
                {
                    SyncObject.Set();
                }
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
