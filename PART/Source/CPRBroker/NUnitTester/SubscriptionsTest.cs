using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NUnitTester
{
    [TestFixture]
    public class SubscriptionsTest : BaseTest
    {
        private Dictionary<string, Guid> CprNumberMap = new Dictionary<string, Guid>();
        [TestFixtureSetUp]
        public void MapCprNumbers()
        {
            foreach (string cprNumber in TestData.cprNumbers)
            {
                CprNumberMap[cprNumber] = TestRunner.PartService.GetPersonUuid(cprNumber);
            }
        }
        private Guid[] GetPersonUuids(string[] cprNumbers)
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
            var uuids = GetPersonUuids(cprNumbers);

            var sub = TestRunner.AdminService.SubscribeOnBirthdate(TestData.fileShareChannel, TestData.birthdateYears, TestData.birthdateDays, uuids);
            Assert.IsNotNull(sub);
            Assert.IsInstanceOf<CPRAdministrationWS.BirthdateSubscriptionType>(sub);
            TestData.birthdateSubscriptions.Add(sub);
        }

        IEnumerable<Func<CPRAdministrationWS.BirthdateSubscriptionType>> birthDateSubscriptionFuncs()
        {
            return TestData.birthdateSubscriptionFunctions;
        }

        [Test]
        [Combinatorial]
        public void T510_SendBirthdateNotifications(
            [ValueSource("birthDateSubscriptionFuncs")]             
            Func<CPRAdministrationWS.BirthdateSubscriptionType> subscriptionFunc,
                [Values(0, -1, 2)] int yearsDiff,
                [Values(0, 1, 2, -2)] int daysDiff
            )
        {
            CPRAdministrationWS.BirthdateSubscriptionType subscription = subscriptionFunc();
            string baseCprNumber;
            DateTime birthDate;
            if (subscription.ForAllPersons)
            {
                baseCprNumber = TestData.cprNumbers[0];
                birthDate = CPRBroker.DAL.Person.PersonNumberToDate(baseCprNumber).Value;
            }
            else
            {
                baseCprNumber = subscription.PersonUuids[0];
                birthDate = CPRBroker.DAL.Person.PersonNumberToDate(baseCprNumber).Value;
            }

            int years = subscription.AgeYears.HasValue ? subscription.AgeYears.Value : 10;
            DateTime notifyDate = birthDate.AddYears(years + yearsDiff).AddDays(-(TestData.birthdateDays + daysDiff));
            bool expected = daysDiff == 0 && (yearsDiff == 0 || !subscription.AgeYears.HasValue);
            var res = TestRunner.AccessService.SendNotifications(notifyDate);
            bool notified = res.SentNotificationIds.Contains(new Guid(subscription.SubscriptionId));
            Assert.AreEqual(expected, notified,
                string.Format("Notification match error. Expected:{0}, \r\n AgeYears={1}, PriorDays={2}, \r\n BirthDate={3}, NotifyDate={4}", expected, years, subscription.PriorDays, birthDate, notifyDate)
                );

            if (expected)
            {
                var notification = TestRunner.AdminService.GetLatestNotification(new Guid(subscription.SubscriptionId));
                Assert.IsNotNull(notification);
                Assert.AreEqual(notification.ApplicationToken, TestData.AppToken);
                Assert.GreaterOrEqual(notification.NotificationDate, notifyDate.Date);
                Assert.IsInstanceOf<CPRAdministrationWS.BirthdateNotificationType>(notification);
                CPRAdministrationWS.BirthdateNotificationType bdNotif = notification as CPRAdministrationWS.BirthdateNotificationType;
                Assert.IsNotNull(bdNotif.BirthdateSubscription);
                Assert.AreEqual(bdNotif.BirthdateSubscription.SubscriptionId, subscription.SubscriptionId);
                Assert.Greater(bdNotif.Persons.Count(), 0);

            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersToSubscribeFieldName)]
        public void T520_Subscribe(string[] cprNumbers)
        {
            var uuids = GetPersonUuids(cprNumbers);
            var sub = TestRunner.AdminService.Subscribe(TestData.fileShareChannel, uuids);
            Assert.IsNotNull(sub);
            Assert.IsInstanceOf<CPRAdministrationWS.ChangeSubscriptionType>(sub);
            TestData.changeSubscriptions.Add(sub);
        }

        [Test]
        public void T530_RefreshPersonsData()
        {
            var reseult = TestRunner.AccessService.RefreshPersonsData();
            Assert.IsNotNull(reseult);
            Assert.Greater(reseult.SucceededCprNumbers.Length, 0);
        }

        private IEnumerable<Func<CPRAdministrationWS.ChangeSubscriptionType>> changeSubscriptionFuncs()
        {
            return TestData.changeSubscriptionFunctions;
        }

        /*
        private DateTime[] changeSubscriptionNotifyDates = new DateTime[] { DateTime.Now.AddDays(1), DateTime.Now.AddDays(2) };

        [Test]
        public void T540_SendDataChangeNotifications(
            [ValueSource("changeSubscriptionFuncs")]
            Func<CPRAdministrationWS.ChangeSubscriptionType> subscriptionFunc,
            [ValueSource("changeSubscriptionNotifyDates")]
            DateTime notifyDate
            )
        {
            CPRAdministrationWS.ChangeSubscriptionType subscription = subscriptionFunc();
            bool notified;
            NUnitTester.Access.SendNotificationsResult res;

            // Create a test person and then submit an update
            this.T700_CreateTestCitizen();
            TestData.testFullPerson.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonGivenName += "S";
            this.T700_CreateTestCitizen();

            // Test whether tomorrow's notifications, should containg this update
            bool expectedDay = notifyDate.Date.Equals(DateTime.Today.AddDays(1));
            bool expectedPerson = (subscription.ForAllPersons || subscription.PersonCivilRegistrationIdentifiers.Contains(TestData.testPersonNumber));

            bool expected = expectedDay && expectedPerson;
            if (!expectedDay || expectedPerson)
            {
                res = TestRunner.AccessService.SendNotifications(notifyDate);
                notified = res.SentNotificationIds.Contains(new Guid(subscription.SubscriptionId));
                Assert.AreEqual(expected, notified, "Data change notification, NotifyDate={0}, For all={1}, Persons={2}", notifyDate, subscription.ForAllPersons, Utilities.ArrayToString(subscription.PersonCivilRegistrationIdentifiers));

                if (expected)
                {
                    var notification = TestRunner.AdminService.GetLatestNotification(new Guid(subscription.SubscriptionId));
                    Assert.AreEqual(notification.ApplicationToken, TestData.AppToken);
                    Assert.GreaterOrEqual(notification.NotificationDate, notifyDate.Date);
                    Assert.IsInstanceOf<CPRAdministrationWS.ChangeNotificationType>(notification);
                    var changeNotif = notification as CPRAdministrationWS.ChangeNotificationType;
                    Assert.Greater(changeNotif.Persons.Count(), 0);
                    Assert.IsNotNull(changeNotif.ChangeSubscription);
                    Assert.AreEqual(changeNotif.ChangeSubscription.SubscriptionId, subscription.SubscriptionId);
                }
            };
        }
        */

        [Test]
        public void T550_GetActiveSubscriptionList()
        {
            var subs = TestRunner.AdminService.GetActiveSubscriptionsList();
            Assert.IsNotNull(subs);
            Assert.GreaterOrEqual(subs.Count(), 2);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.birthdateSubscriptionFunctionsFieldName)]
        public void T560_RemoveBirthdateSubscription(Func<CPRAdministrationWS.BirthdateSubscriptionType> subscriptionFunc)
        {
            CPRAdministrationWS.BirthdateSubscriptionType subscription = subscriptionFunc();
            bool res = TestRunner.AdminService.RemoveBirthDateSubscription(new Guid(subscription.SubscriptionId));
            Assert.IsTrue(res);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.changeSubscriptionFunctionsFieldName)]
        public void T570_Unsubscribe(Func<CPRAdministrationWS.ChangeSubscriptionType> subscriptionFunc)
        {
            CPRAdministrationWS.ChangeSubscriptionType sub = subscriptionFunc();
            bool res = TestRunner.AdminService.Unsubscribe(new Guid(sub.SubscriptionId));
        }
    }
}
