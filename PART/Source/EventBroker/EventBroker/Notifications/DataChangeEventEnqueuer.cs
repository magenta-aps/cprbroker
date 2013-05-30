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
using CprBroker.Utilities;
using CprBroker.Schemas.Part;
using CprBroker.Data.Part;
using CprBroker.Engine.Local;

namespace CprBroker.EventBroker.Notifications
{
    /// <summary>
    /// Gets data change events for Event Broker from Cpr Broker
    /// </summary>
    public partial class DataChangeEventEnqueuer : CprBrokerEventEnqueuer
    {
        public DataChangeEventEnqueuer()
        {

        }

        public DataChangeEventEnqueuer(IContainer container)
            : base(container)
        {
            container.Add(this);
        }

        protected override TimeSpan CalculateActionTimerInterval(TimeSpan currentInterval)
        {
            return TimeSpan.FromMinutes(1);
        }

        protected override void PerformTimerAction()
        {
            UpdateSubscriptionCriteriaLists();
            PushNotifications();
        }

        public static void UpdateSubscriptionCriteriaLists()
        {
            int batchSize = CprBroker.Config.Properties.Settings.Default.SubscriptionCriteriaMatchingBatchSize;

            Admin.LogFormattedSuccess("DataChangeEventEnqueuer.UpdateSubscriptionCriteriaLists() started, batch size <{0}>", batchSize);

            using (var eventDataContext = new Data.EventBrokerDataContext())
            {
                var subscriptions = eventDataContext.Subscriptions.Where(s => s.Deactivated == null && s.LastCheckedUUID != null).OrderBy(s => s.Created).ToArray();
                Admin.LogFormattedSuccess("Found <{0}> pending criteria subscriptions", subscriptions.Length);
                foreach (var sub in subscriptions)
                {
                    while (sub.LastCheckedUUID.HasValue)
                    {
                        // TODO: Merge this log entry with the one at the end
                        Admin.LogFormattedSuccess("Adding persons to subscription <{0}>, start UUID <{1}>, batch size <{2}>", sub.SubscriptionId, sub.LastCheckedUUID.Value, batchSize);
                        var added = sub.AddMatchingSubscriptionPersons(eventDataContext, batchSize);
                        eventDataContext.SubmitChanges();
                        Admin.LogFormattedSuccess("Added <{0}> persons to subscription <{1}>, next start UUID <{2}>", added, sub.SubscriptionId, sub.LastCheckedUUID);
                    }
                }
            }
            Admin.LogFormattedSuccess("DataChangeEventEnqueuer.UpdateSubscriptionCriteriaLists() finished");
        }

        public void PushNotifications()
        {
            var batchSize = CprBroker.Config.Properties.Settings.Default.DataChangeDequeueBatchSize;

            Admin.LogFormattedSuccess("DataChangeEventEnqueuer.PushNotifications() started, batch size <{0}>", batchSize);
            bool moreChangesExist = true;

            while (moreChangesExist)
            {
                var resp = EventsService.DequeueDataChangeEvents(batchSize);
                var changedPeople = resp.Item;
                if (changedPeople == null)
                    changedPeople = new EventBroker.EventsService.DataChangeEventInfo[0];
                moreChangesExist = changedPeople.Length == batchSize;

                Admin.LogFormattedSuccess("DataChangeEventEnqueuer.PushNotifications(): <{0}> data changes found", changedPeople.Length);

                using (var dataContext = new Data.EventBrokerDataContext())
                {
                    var dbObjects = Array.ConvertAll<EventsService.DataChangeEventInfo, Data.DataChangeEvent>(
                        changedPeople,
                        p => new Data.DataChangeEvent()
                        {
                            DataChangeEventId = p.EventId,
                            DueDate = p.ReceivedDate,
                            PersonUuid = p.PersonUuid,
                            PersonRegistrationId = p.PersonRegistrationId,
                            ReceivedDate = DateTime.Now
                        }
                    );

                    dataContext.DataChangeEvents.InsertAllOnSubmit(dbObjects);
                    dataContext.SubmitChanges();

                    DateTime now = DateTime.Now;
                    MatchDataChangeEvents(dataContext, dbObjects, now);

                    dataContext.UpdatePersonLists(now, (int)Data.SubscriptionType.SubscriptionTypes.DataChange);
                    dataContext.EnqueueDataChangeEventNotifications(now, (int)Data.SubscriptionType.SubscriptionTypes.DataChange);

                    //TODO: Move this logic to above stored procedure
                    dataContext.DataChangeEvents.DeleteAllOnSubmit(dbObjects);
                    dataContext.SubmitChanges();
                }
            }
        }

        private void MatchDataChangeEvents(Data.EventBrokerDataContext dataContext, Data.DataChangeEvent[] dataChangeEvents, DateTime now)
        {
            var criteriaSubscriptions = dataContext.Subscriptions.Where(sub => sub.Criteria != null).ToArray();
            foreach (var subscription in criteriaSubscriptions)
            {
                subscription.GetDataChangeEventMatches(dataChangeEvents);
            }
            dataContext.SubmitChanges();
        }
    }
}
