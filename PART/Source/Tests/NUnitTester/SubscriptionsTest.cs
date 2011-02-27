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
                    var uuid = TestRunner.PartService.GetUuid(TestRunner.PartApplicationHeader, cprNumber);
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
            var res = TestRunner.SubscriptionsService.SubscribeOnBirthdate(TestRunner.SubscriptionsApplicationHeader, TestData.fileShareChannel, TestData.birthdateYears, TestData.birthdateDays, uuids);
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
            var res = TestRunner.SubscriptionsService.Subscribe(TestRunner.SubscriptionsApplicationHeader, TestData.fileShareChannel, uuids);
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
            var res = TestRunner.SubscriptionsService.GetActiveSubscriptionsList(TestRunner.SubscriptionsApplicationHeader);
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
            var res = TestRunner.SubscriptionsService.RemoveBirthDateSubscription(TestRunner.SubscriptionsApplicationHeader, new Guid(subscription.SubscriptionId));
            Assert.IsNotNull(res);
            Validate(res.StandardRetur);
            Assert.IsTrue(res.Item);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.changeSubscriptionFunctionsFieldName)]
        public void T570_Unsubscribe(Func<Subscriptions.ChangeSubscriptionType> subscriptionFunc)
        {
            Subscriptions.ChangeSubscriptionType sub = subscriptionFunc();
            var res = TestRunner.SubscriptionsService.Unsubscribe(TestRunner.SubscriptionsApplicationHeader, new Guid(sub.SubscriptionId));
            Assert.IsNotNull(res);
            Validate(res.StandardRetur);
            Assert.IsTrue(res.Item);
        }
    }
}
