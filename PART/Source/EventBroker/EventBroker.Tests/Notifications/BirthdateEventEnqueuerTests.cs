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
using System.Linq;
using System.Text;
using CprBroker.EventBroker.Notifications;
using NUnit.Framework;
using CprBroker.EventBroker.Data;
using CprBroker.Tests.PartInterface;
using CprBroker.Engine;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.EventBroker.Notifications
{
    namespace BirthdateEventEnqueuerTests
    {
        [Ignore]
        public class BirthdateEventEnqueuerTestBase : NotificationTestBase
        {
            
            public BasicOutputType<BirthdateSubscriptionType> CreateBirthdateSubscription(int? years, int days, params Guid[] uuids)
            {
                var mgr = new CprBroker.EventBroker.Subscriptions.SubscriptionManager();
                var ret = mgr.SubscribeOnBirthdate("", BrokerContext.Current.ApplicationToken, new Schemas.Part.FileShareChannelType() { Path = "c:\\Notif\\Temp" }, years, days, uuids);
                return ret;
            }

            public BasicOutputType<bool> RemoveBirthdateSubscription(Guid subscriptionId)
            {
                var mgr = new CprBroker.EventBroker.Subscriptions.SubscriptionManager();
                var ret = mgr.RemoveBirthDateSubscription("", BrokerContext.Current.ApplicationToken, subscriptionId);
                return ret;
            }

            public void SetBirthdate(Guid uuid, DateTime birthdate)
            {
                using (var dataContext = new EventBrokerDataContext(this.EventDatabase.ConnectionString))
                {
                    dataContext.PersonBirthdates.InsertOnSubmit(new PersonBirthdate() { PersonUuid = uuid, Birthdate = birthdate.Date });
                    dataContext.SubmitChanges();
                }
            }
        }

        [TestFixture]
        public class EnqueueBirthdateEvents : BirthdateEventEnqueuerTestBase
        {
            [Test]
            public void EnqueueBirthdateEvents_Match_Enqueued([Values(10, 2, 1)] int priorDays)
            {
                var uuid = Guid.NewGuid();
                var sub = base.CreateBirthdateSubscription(null, priorDays, uuid);
                base.SetBirthdate(uuid, DateTime.Today.AddYears(-1).AddDays(priorDays));

                using (var task = new BirthdateEventEnqueuer())
                {
                    task.EnqueueBirthdateEvents();
                }

                using (var dataContext = new EventBrokerDataContext())
                {
                    var notif = dataContext.EventNotifications.ToArray();
                    Assert.AreEqual(1, notif.Length);
                }
            }

            [Test]
            public void EnqueueBirthdateEvents_NoMatch_NotEnqueued([Values(10, 2, 1)] int priorDays)
            {
                var uuid = Guid.NewGuid();
                var sub = base.CreateBirthdateSubscription(null, priorDays, uuid);
                base.SetBirthdate(uuid, DateTime.Today.AddYears(-1));

                using (var task = new BirthdateEventEnqueuer())
                {
                    task.EnqueueBirthdateEvents();
                }

                using (var dataContext = new EventBrokerDataContext())
                {
                    var notif = dataContext.EventNotifications.ToArray();
                    Assert.AreEqual(0, notif.Length);
                }
            }

            [Test]
            public void EnqueueBirthdateEvents_DeactivatedSubscription_NotEnqueued()
            {
                var uuid = Guid.NewGuid();

                var sub = base.CreateBirthdateSubscription(null, 0, uuid);
                base.RemoveBirthdateSubscription(new Guid(sub.Item.SubscriptionId));

                base.SetBirthdate(uuid, DateTime.Today.AddYears(-1));

                using (var task = new BirthdateEventEnqueuer())
                {
                    task.EnqueueBirthdateEvents();
                }

                using (var dataContext = new EventBrokerDataContext())
                {
                    var notif = dataContext.EventNotifications.ToArray();
                    Assert.AreEqual(0, notif.Length);
                }
            }

            [Test]
            public void EnqueueBirthdateEvents_ChangeSubscription_NotEnqueued()
            {
                var uuid = Guid.NewGuid();

                var sub = base.CreateChangeSubscription(uuid);

                base.SetBirthdate(uuid, DateTime.Today.AddYears(-1));

                using (var task = new BirthdateEventEnqueuer())
                {
                    task.EnqueueBirthdateEvents();
                }

                using (var dataContext = new EventBrokerDataContext())
                {
                    var notif = dataContext.EventNotifications.ToArray();
                    Assert.AreEqual(0, notif.Length);
                }
            }
        }
    }
}
