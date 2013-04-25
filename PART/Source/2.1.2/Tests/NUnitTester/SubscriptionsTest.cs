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

namespace CprBroker.NUnitTester
{
    [TestFixture]
    public class SubscriptionsTest : BaseTest
    {
        private Dictionary<string, Guid> CprNumberMap = new Dictionary<string, Guid>();

        public void Validate(Subscriptions.StandardReturType ret)
        {
            Assert.IsNotNull(ret);
            base.Validate(ret.StatusKode, ret.FejlbeskedTekst);
        }

        public void MapCprNumbers()
        {
            foreach (string cprNumber in TestData.cprNumbers)
            {
                if (!CprNumberMap.ContainsKey(cprNumber))
                {
                    var uuid = TestRunner.PartService.GetUuid(cprNumber);
                    CprNumberMap[cprNumber] = new Guid(uuid.UUID);
                }
            }
        }
        private Guid[] GetUuids(string[] cprNumbers)
        {
            if (cprNumbers == null)
                return null;
            else
                return Array.ConvertAll<string, Guid>(cprNumbers, (cpr) => CprNumberMap[cpr]);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersToSubscribeFieldName)]
        public void T500_SubscribeOnBirthdate(string[] cprNumbers)
        {
            MapCprNumbers();
            var uuids = GetUuids(cprNumbers);
            var res = TestRunner.SubscriptionsService.SubscribeOnBirthdate( TestData.fileShareChannel, TestData.birthdateYears, TestData.birthdateDays, uuids);
            Assert.IsNotNull(res);
            Validate(res.StandardRetur);
            Assert.IsNotNull(res.Item);
            Assert.IsInstanceOf<Subscriptions.BirthdateSubscriptionType>(res.Item);
            TestData.birthdateSubscriptions.Add(res.Item);
        }

        IEnumerable<Func<Subscriptions.BirthdateSubscriptionType>> birthDateSubscriptionFuncs()
        {
            return TestData.birthdateSubscriptionFunctions;
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersToSubscribeFieldName)]
        public void T520_Subscribe(string[] cprNumbers)
        {
            MapCprNumbers();
            var uuids = GetUuids(cprNumbers);
            var res = TestRunner.SubscriptionsService.Subscribe( TestData.fileShareChannel, uuids);
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.Item);
            Validate(res.StandardRetur);
            Assert.AreEqual(TestRunner.SubscriptionsApplicationHeader.ApplicationToken, res.Item.ApplicationToken);
            Assert.IsInstanceOf<Subscriptions.ChangeSubscriptionType>(res.Item);
            TestData.changeSubscriptions.Add(res.Item);
        }

        private IEnumerable<Func<Subscriptions.ChangeSubscriptionType>> changeSubscriptionFuncs()
        {
            return TestData.changeSubscriptionFunctions;
        }

        [Test]
        public void T550_GetActiveSubscriptionList()
        {
            var res = TestRunner.SubscriptionsService.GetActiveSubscriptionsList();
            Assert.IsNotNull(res);
            Validate(res.StandardRetur);
            Assert.IsNotNull(res.Item);
            Assert.GreaterOrEqual(res.Item.Count(), 2);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.birthdateSubscriptionFunctionsFieldName)]
        public void T560_RemoveBirthdateSubscription(Func<Subscriptions.BirthdateSubscriptionType> subscriptionFunc)
        {
            Subscriptions.BirthdateSubscriptionType subscription = subscriptionFunc();
            var res = TestRunner.SubscriptionsService.RemoveBirthDateSubscription( new Guid(subscription.SubscriptionId));
            Assert.IsNotNull(res);
            Validate(res.StandardRetur);
            Assert.IsTrue(res.Item);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.changeSubscriptionFunctionsFieldName)]
        public void T570_Unsubscribe(Func<Subscriptions.ChangeSubscriptionType> subscriptionFunc)
        {
            Subscriptions.ChangeSubscriptionType sub = subscriptionFunc();
            var res = TestRunner.SubscriptionsService.Unsubscribe( new Guid(sub.SubscriptionId));
            Assert.IsNotNull(res);
            Validate(res.StandardRetur);
            Assert.IsTrue(res.Item);
        }
    }
}
