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
using NUnit.Framework;
using CprBroker.EventBroker;
using CprBroker.EventBroker.Notifications;
using CprBroker.EventBroker.Data;

namespace CprBroker.Tests.EventBroker.Notifications
{
    namespace DataChangeEventEnqueuerTests
    {

        class DataChangeEventEnqueuerStub : DataChangeEventEnqueuer
        {
            public DataChangeEventEnqueuerStub()
            {
                this.BatchSize = 100;
            }
        }

        [TestFixture]
        public class EnqueueAllDataChangesTests : NotificationTestBase
        {
            [Test]
            public void EnqueueAllDataChanges_Nothing_Nothing()
            {
                using (var dataContext = new EventBrokerDataContext())
                {
                    var c = dataContext.EventNotifications.Count();
                    Assert.AreEqual(0, c);
                }
            }

            [Test]
            public void EnqueueAllDataChanges_EventNoSubscriptions_ChangesCleared()
            {
                using (var dataContext = new EventBrokerDataContext())
                {
                    var dce = AddChanges(dataContext, 1);
                }

                using (var task = new DataChangeEventEnqueuerStub())
                {
                    task.EnqueueAllDataChanges();
                }

                using (var dataContext = new EventBrokerDataContext())
                {
                    var c = dataContext.DataChangeEvents.Count();
                    Assert.AreEqual(0, c);
                }
            }

            [Test]
            public void EnqueueAllDataChanges_EventNoSubscriptions_NoNotifications()
            {
                using (var dataContext = new EventBrokerDataContext())
                {
                    var dce = AddChanges(dataContext, 1);
                }

                using (var task = new DataChangeEventEnqueuerStub())
                {
                    task.EnqueueAllDataChanges();
                }

                using (var dataContext = new EventBrokerDataContext())
                {
                    var c = dataContext.EventNotifications.Count();
                    Assert.AreEqual(0, c);
                }
            }

            [Test]
            public void EnqueueAllDataChanges_EventAndSubscription_Enqueued()
            {
                using (var dataContext = new EventBrokerDataContext())
                {
                    var dce = AddChanges(dataContext, 1);
                    base.CreateChangeSubscription(dce.First().PersonUuid);
                }

                using (var task = new DataChangeEventEnqueuerStub())
                {
                    task.EnqueueAllDataChanges();
                }

                using (var dataContext = new EventBrokerDataContext())
                {
                    var c = dataContext.EventNotifications.Count();
                    Assert.AreEqual(1, c);
                }
            }

            [Test]
            public void EnqueueAllDataChanges_EventAndInactiveSubscription_ChangesCleared()
            {
                using (var dataContext = new EventBrokerDataContext())
                {
                    var dce = AddChanges(dataContext, 1);
                    base.CreateChangeSubscription(dce.First().PersonUuid);
                }

                using (var task = new DataChangeEventEnqueuerStub())
                {
                    task.EnqueueAllDataChanges();
                }

                using (var dataContext = new EventBrokerDataContext())
                {
                    var c = dataContext.DataChangeEvents.Count();
                    Assert.AreEqual(0, c);
                }
            }

            [Test]
            public void EnqueueAllDataChanges_EventAndInactiveSubscription_NotEnqueued()
            {
                using (var dataContext = new EventBrokerDataContext())
                {
                    var dce = AddChanges(dataContext, 1);
                    var sub = base.CreateChangeSubscription(dce.First().PersonUuid);
                    base.RemoveChangeSubscription(new Guid(sub.Item.SubscriptionId));
                }

                using (var task = new DataChangeEventEnqueuerStub())
                {
                    task.EnqueueAllDataChanges();
                }

                using (var dataContext = new EventBrokerDataContext())
                {
                    var c = dataContext.EventNotifications.Count();
                    Assert.AreEqual(0, c);
                }
            }

            [Test]
            public void EnqueueAllDataChanges_EventAndActiveAndInactiveSubscription_ActiveEnqueued()
            {
                DataChangeEvent[] dce;
                using (var dataContext = new EventBrokerDataContext())
                {
                    dce = AddChanges(dataContext, 1);

                }

                var actSub = base.CreateChangeSubscription(dce.First().PersonUuid);
                var delSub = base.CreateChangeSubscription(dce.First().PersonUuid);
                base.RemoveChangeSubscription(new Guid(delSub.Item.SubscriptionId));

                using (var task = new DataChangeEventEnqueuerStub())
                {
                    task.EnqueueAllDataChanges();
                }

                using (var dataContext = new EventBrokerDataContext())
                {
                    var c = dataContext.EventNotifications.Count();
                    Assert.AreEqual(1, c);

                    var en = dataContext.EventNotifications.Single();
                    Assert.AreEqual(actSub.Item.SubscriptionId, en.SubscriptionId.ToString());
                }
            }
        }
    }
}
