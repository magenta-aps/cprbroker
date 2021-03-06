﻿/* ***** BEGIN LICENSE BLOCK *****
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

namespace CprBroker.EventBroker.Notifications
{
    /// <summary>
    /// Syncronizes birthdates from Cpr broker and then enqueues any needed birthdate notifications
    /// </summary>
    public partial class BirthdateEventEnqueuer : CprBrokerEventEnqueuer
    {
        public BirthdateEventEnqueuer()
            : base()
        {
            InitializeComponent();
        }

        public BirthdateEventEnqueuer(IContainer container)
            : base(container)
        {
            container.Add(this);
            InitializeComponent();
        }

        protected override void PerformTimerAction()
        {
            SynchronisePersonBirthdates();
            EnqueueBirthdateEvents();
        }

        private void SynchronisePersonBirthdates()
        {
            bool morePersons = true;
            Guid? lastPersonGuid = null;
            while (morePersons)
            {
                var resp = EventsService.GetPersonBirthdates(lastPersonGuid, BatchSize);
                var personBirthdates = resp.Item;

                if (personBirthdates.Length > 0)
                {
                    lastPersonGuid = personBirthdates[personBirthdates.Length - 1].PersonUuid;
                }
                morePersons = personBirthdates.Length == BatchSize;

                using (var dataContext = new Data.EventBrokerDataContext())
                {
                    var personswithoutBirthdates = personBirthdates.Where(p => !p.Birthdate.HasValue).ToArray();
                    if (personswithoutBirthdates.Length > 0)
                    {
                        // TODO: Shall this be a critical error?
                        CprBroker.Engine.Local.Admin.LogFormattedError("Unable to get birthdates for <{0}> persons, first UUID <{1}>, last UUID <{2}>", personswithoutBirthdates.Length, personswithoutBirthdates.First().PersonUuid, personswithoutBirthdates.Last().PersonUuid);
                    }
                    var newPersons =
                    (
                        from pb in personBirthdates
                        where
                            pb.Birthdate.HasValue
                            && !(from dpb in dataContext.PersonBirthdates select dpb.PersonUuid).Contains(pb.PersonUuid)

                        select new Data.PersonBirthdate()
                        {
                            PersonUuid = pb.PersonUuid,
                            Birthdate = pb.Birthdate.Value
                        }
                    );

                    dataContext.PersonBirthdates.InsertAllOnSubmit(newPersons);
                    dataContext.SubmitChanges();
                }
            }
        }

        public void EnqueueBirthdateEvents()
        {
            using (var dataContext = new Data.EventBrokerDataContext())
            {
                DateTime today = DateTime.Today;

                var subscriptions = Data.Subscription.ActiveSubscriptions(dataContext)
                    .Where(s =>
                        s.BirthdateSubscription != null)
                    .ToArray();

                foreach (var subscription in subscriptions)
                {
                    try
                    {
                        dataContext.EnqueueBirthdateEventNotifications(subscription.SubscriptionId, today);
                    }
                    catch (Exception ex)
                    {
                        string message = string.Format("Failed to enqueue birthdate notifications for {0}", subscription.SubscriptionId);
                        CprBroker.Engine.Local.Admin.LogException(ex, message);
                    }
                }
            }
        }
    }
}
