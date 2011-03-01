using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using CprBroker.Engine;
using CprBroker.Utilities;

namespace CprBroker.EventBroker.Backend
{
    /// <summary>
    /// This service is responsible for the scheduling of publishing events to subscribers
    /// </summary>
    public partial class BackendService : System.ServiceProcess.ServiceBase
    {
        public BackendService()
        {
            InitializeComponent();

            this.BirthdateEventEnqueuer.EventLog = this.EventLog;
            this.DataChangeEventEnqueuer.EventLog = this.EventLog;
            this.NotificationSender.EventLog = this.EventLog;
        }

        private void StartQueues()
        {
            this.BirthdateEventEnqueuer.Start();
            this.DataChangeEventEnqueuer.Start();
            this.NotificationSender.Start();
        }

        private void StopQueues()
        {
            this.BirthdateEventEnqueuer.Stop();
            this.DataChangeEventEnqueuer.Stop();
            this.NotificationSender.Stop();
        }

        protected override void OnStart(string[] args)
        {
            BrokerContext.Initialize(Constants.EventBrokerApplicationToken.ToString(), Constants.UserToken);

            try
            {
                Security.EncryptConnectionStrings();
            }
            catch (Exception ex)
            {
                CprBroker.Engine.Local.Admin.LogException(ex);
            }

            CprBroker.Engine.Local.Admin.LogSuccess(TextMessages.BackendServiceStarted);

            StartQueues();
        }

        protected override void OnStop()
        {
            StopQueues();
        }

        protected override void OnContinue()
        {
            StartQueues();
        }

        protected override void OnPause()
        {
            StopQueues();
        }



    }
}
