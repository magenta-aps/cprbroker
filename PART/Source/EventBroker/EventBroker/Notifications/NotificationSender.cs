/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace CprBroker.EventBroker.Notifications
{
    /// <summary>
    /// Reads the notification queue and sends the notifications to subscribers
    /// </summary>
    public partial class NotificationSender : PeriodicTaskExecuter
    {
        public NotificationSender()
            : base()
        {
            InitializeComponent();
        }

        public NotificationSender(IContainer container)
            : base(container)
        {
            container.Add(this);
            InitializeComponent();
        }

        protected override TimeSpan CalculateActionTimerInterval(TimeSpan currentInterval)
        {
            return TimeSpan.FromMilliseconds(CprBroker.Config.Properties.Settings.Default.EventBrokerPollIntervalMilliseconds);
        }
        
        protected override void PerformTimerAction()
        {
            using (var dataContext = new Data.EventBrokerDataContext())
            {
                int batchSize = CprBroker.Config.Properties.Settings.Default.EventBrokerNotificationBatchSize;
                Data.EventNotification[] dueNotifications = new CprBroker.EventBroker.Data.EventNotification[0];
                do
                {
                    dueNotifications =
                        (
                            from eventNotification in dataContext.EventNotifications
                            where eventNotification.Succeeded == null
                            orderby eventNotification.CreatedDate
                            select eventNotification
                        ).Take(batchSize).ToArray();


                    foreach (var eventNotification in dueNotifications)
                    {
                        eventNotification.NotificationDate = DateTime.Now;
                        try
                        {
                            Channel channel = Channel.Create(eventNotification.Subscription.Channels.Single());
                            channel.Notify(eventNotification);
                            eventNotification.Succeeded = true;
                        }
                        catch (Exception ex)
                        {
                            string message = string.Format("Notification {0} failed", eventNotification.EventNotificationId);
                            //TODO: use LogNotificationFailure after simplifying its parameters
                            CprBroker.Engine.Local.Admin.LogException(ex, message);
                            eventNotification.Succeeded = false;
                        }
                    }
                    dataContext.SubmitChanges();

                } while (dueNotifications.Length == batchSize);
            }
        }
    }
}
