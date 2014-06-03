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
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Data.Part;
using CprBroker.Schemas.Part;
using CprBroker.EventBroker.Data;
using System.IO;

namespace CprBroker.EventBroker.Tests
{
    [TestFixture]
    public class UpdatePersonLists : TestBase
    {
        public static string[] MunicipalityCodes = new string[] { "1950", "935" };

        [Test]
        [TestCaseSource("MunicipalityCodes")]
        public void UpdatePersonLists_2Changes_1MovingIn_PersonAdded(string municipalityCode)
        {
            using (var dataContext = new EventBrokerDataContext(ConnectionString))
            {
                var dce = AddChanges(dataContext, 2);
                var sub = AddSubscription(dataContext, Utils.CreateSoegObject(municipalityCode), false, true, Data.SubscriptionType.SubscriptionTypes.DataChange);
                sub.SubscriptionCriteriaMatches.Add(new SubscriptionCriteriaMatch() { SubscriptionCriteriaMatchId = Guid.NewGuid(), DataChangeEvent = dce[0] });

                dataContext.SubmitChanges();

                int records = dataContext.UpdatePersonLists(DateTime.Now, int.MaxValue, (int)Data.SubscriptionType.SubscriptionTypes.DataChange);
                var subscriptionPersons = dataContext.SubscriptionPersons.Where(sp => sp.SubscriptionId == sub.SubscriptionId && sp.Removed == null).ToArray();
                Assert.AreEqual(1, subscriptionPersons.Length);
                Assert.Null(subscriptionPersons[0].Removed);

                var notif = dataContext.EventNotifications.Where(en => en.SubscriptionId == sub.SubscriptionId).SingleOrDefault();
                Assert.Null(notif);
            }

        }

        [Test]
        [TestCaseSource("MunicipalityCodes")]
        public void UpdatePersonLists_2Changes_1MovingOut_PersonRemovedAndNotoficationAdded(string municipalityCode)
        {
            using (var dataContext = new EventBrokerDataContext(ConnectionString))
            {
                // Init data
                var dce = AddChanges(dataContext, 2);
                var sub = AddSubscription(dataContext, Utils.CreateSoegObject(municipalityCode), false, true, Data.SubscriptionType.SubscriptionTypes.DataChange);
                var persons = AddPersons(sub, 1);
                persons[0].PersonUuid = dce[0].PersonUuid;                

                dataContext.Subscriptions.InsertOnSubmit(sub);
                dataContext.SubmitChanges();

                // Action
                int records = dataContext.UpdatePersonLists(DateTime.Now, int.MaxValue, (int)Data.SubscriptionType.SubscriptionTypes.DataChange);
                
                // Refresh from DB
                dataContext.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, dataContext.SubscriptionPersons);
                
                // Test outcome
                var subscriptionPerson = dataContext.SubscriptionPersons.Where(sp => sp.SubscriptionId == sub.SubscriptionId).Single();
                Assert.NotNull(subscriptionPerson.Removed);

                var notif = dataContext.EventNotifications.Where(en => en.SubscriptionId == sub.SubscriptionId).Single();
                Assert.NotNull(notif.IsLastNotification);
                Assert.True(notif.IsLastNotification.Value);
            }
        }

        [Test]
        [TestCaseSource("MunicipalityCodes")]
        public void UpdatePersonLists_2Changes_1In_AlreadyInSubscription_StaysInSubscriptionAndNoNotificationCreated(string municipalityCode)
        {
            using (var dataContext = new EventBrokerDataContext(ConnectionString))
            {
                var dce = AddChanges(dataContext, 2);
                var sub = AddSubscription(dataContext, Utils.CreateSoegObject(municipalityCode), false, true, Data.SubscriptionType.SubscriptionTypes.DataChange);
                sub.SubscriptionPersons.Add(new SubscriptionPerson() { SubscriptionPersonId = Guid.NewGuid(), PersonUuid = dce[0].PersonUuid, Created = DateTime.Now, Removed = null });
                sub.SubscriptionCriteriaMatches.Add(new SubscriptionCriteriaMatch() { SubscriptionCriteriaMatchId = Guid.NewGuid(), DataChangeEvent = dce[0] });

                dataContext.SubmitChanges();

                int records = dataContext.UpdatePersonLists(DateTime.Now, int.MaxValue, (int)Data.SubscriptionType.SubscriptionTypes.DataChange);
                var subscriptionPerson = dataContext.SubscriptionPersons.Where(sp => sp.SubscriptionId == sub.SubscriptionId).Single();
                Assert.Null(subscriptionPerson.Removed);

                var notif = dataContext.EventNotifications.Where(en => en.SubscriptionId == sub.SubscriptionId).SingleOrDefault();
                Assert.Null(notif);
            }
        }

        [Test]
        [TestCaseSource("MunicipalityCodes")]
        public void UpdatePersonLists_2Changes_NonInSubscription_NothingAddedInSubscription_And_NoNotificationCreated(string municipalityCode)
        {
            using (var dataContext = new EventBrokerDataContext(ConnectionString))
            {
                var dce = AddChanges(dataContext, 2);
                var sub = AddSubscription(dataContext, Utils.CreateSoegObject(municipalityCode), false, true, Data.SubscriptionType.SubscriptionTypes.DataChange);
                dataContext.SubmitChanges();

                int records = dataContext.UpdatePersonLists(DateTime.Now, int.MaxValue, (int)Data.SubscriptionType.SubscriptionTypes.DataChange);
                var subscriptionPerson = dataContext.SubscriptionPersons.Where(sp => sp.SubscriptionId == sub.SubscriptionId).SingleOrDefault();
                Assert.Null(subscriptionPerson);

                var notif = dataContext.EventNotifications.Where(en => en.SubscriptionId == sub.SubscriptionId).SingleOrDefault();
                Assert.Null(notif);
            }
        }
    }
}
