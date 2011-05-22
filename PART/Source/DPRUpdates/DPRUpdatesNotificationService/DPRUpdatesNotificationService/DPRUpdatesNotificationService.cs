//------------------------------------------------------------------------------
// Gentofte Kommune DPR update notification service.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using DPRUpdateLib;

using GKApp2010.Common;
using GKApp2010.Core;

namespace DPRUpdatesNotification
{
    // ================================================================================
    partial class DPRUpdatesNotificationService : ServiceBase
    {
        Runner run = null;

        // -----------------------------------------------------------------------------
        public DPRUpdatesNotificationService()
        {
            InitializeComponent();
            AutoLog = true;

            run = new Runner(HaltOperation);

            run.SetCPRBrokerServiceURL(Properties.Settings.Default.CPRBrokerPartServiceUrl);
            run.SetCPRBrokerAppToken(Properties.Settings.Default.ApplicationToken);
        }

        // -----------------------------------------------------------------------------
        protected override void OnStart(string[] args)
        {
            try
            {
                run.Start();
            }
            catch (Exception ex)
            {
                HaltOperation(ex, "*** FATAL: An exception was thrown during service start up (onStart()). ");
            }
        }

        // -----------------------------------------------------------------------------
        protected override void OnStop()
        {
            run.Stop();
        }

        // -----------------------------------------------------------------------------
        public void HaltOperation(Exception ex, string auxMessage)
        {
            string msg = ExceptionMessageBuilder.Build(auxMessage, ex);

            LogHelper.LogToFile(msg);
            EventLog.WriteEntry(msg, EventLogEntryType.Error);
            MailHelper.SendMessage(msg);

            // Wait a second, then re throw to let environment handle proces termination

            throw ex;
        }
    }
}
