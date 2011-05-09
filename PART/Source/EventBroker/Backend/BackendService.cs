/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 *
 * The Initial Developer of the Original Code is
 * IT- og Telestyrelsen / Danish National IT and Telecom Agency.
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */
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
            CprBroker.Engine.Local.Admin.LogSuccess(TextMessages.BackendServiceStarting);
            StartQueues();
            CprBroker.Engine.Local.Admin.LogSuccess(TextMessages.BackendServiceStarted);
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
