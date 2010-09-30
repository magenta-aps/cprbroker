using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NUnitTester
{
    /// <summary>
    /// Contains methods that test the whole system's web services
    /// </summary>
    [TestFixture]
    public class FullTest : BaseTest
    {
        
        [Test]
        public void T030_ListAppRegistrations()
        {
            var apps = TestRunner.AdminService.ListAppRegistrations();
            Assert.IsNotNull(apps);
            var targetApp = (from app in apps where app.Token == TestData.AppToken select app).SingleOrDefault();
            Assert.IsNotNull(targetApp);
            Assert.Greater(targetApp.RegistrationDate, DateTime.Today);
        }

        [Test]
        public void T100_GetAndSetDataProviderList()
        {
            var dataProviders = TestRunner.AdminService.GetDataProviderList();
            Assert.IsNotNull(dataProviders);

            bool result = TestRunner.AdminService.SetDataProviderList(dataProviders);
            Assert.IsTrue(result);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T200_GetCitizenNameAndAddress(string cprNumber)
        {
            var person = TestRunner.PersonService.GetCitizenNameAndAddress(cprNumber);
            Assert.IsNotNull(person, "Person");
            Assert.IsNotNull(person.Item, "Address");
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T210_GetCitizenBasic(string cprNumber)
        {
            var person = TestRunner.PersonService.GetCitizenBasic(cprNumber);
            Assert.IsNotNull(person);
            Assert.IsNotNull(person.Item);
            Assert.IsNotNull(person.RegularCPRPerson);
            Assert.IsNotNull(person.RegularCPRPerson.SimpleCPRPerson);
            Assert.IsNotNull(person.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure);
            Assert.IsNotNullOrEmpty(person.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonGivenName);
            Assert.IsNotNullOrEmpty(person.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonSurnameName);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T220_GetCitizenFull(string cprNumber)
        {
            var person = TestRunner.PersonService.GetCitizenFull(cprNumber);
            Assert.IsNotNull(person, "Person");
            Assert.IsNotNull(person.Item, "Address");
            //Assert.IsNotNull(person.PersonNationalityCode, "PersonNationalityCode");
            Assert.IsNotNull(person.RegularCPRPerson, "RegularCPRPerson");
            Assert.IsNotNull(person.RegularCPRPerson.SimpleCPRPerson, "SimpleCPRPerson");
            Assert.IsNotNull(person.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure, "PersonNameStructure");
            Assert.IsNotNullOrEmpty(person.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonGivenName, "PersonGivenName");
            Assert.IsNotNullOrEmpty(person.RegularCPRPerson.SimpleCPRPerson.PersonNameStructure.PersonSurnameName, "PersonSurnameName");
        }

        /// <summary>
        /// Workaround for bug in NUnit
        /// </summary>
        /// <returns></returns>
        private string[] cprNumbersWithChildren()
        {
            return TestData.cprNumbersWithChildren;
        }

        [Test]
        [Combinatorial()]
        public void T230_GetCitizenChildren(
            [ValueSource("cprNumbersWithChildren")] string cprNumber,
            [Values(true, false)]bool includeChildren
            )
        {
            var children = TestRunner.PersonService.GetCitizenChildren(cprNumber, includeChildren);
            Assert.IsNotNull(children);
            Assert.Greater(children.Count(), 0);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T300_GetCitizenRelations(string cprNumber)
        {
            var rel = TestRunner.PersonService.GetCitizenRelations(cprNumber);
            Assert.IsNotNull(rel, "Return");
            Assert.IsNotNull(rel.SimpleCPRPerson, "Person");
            Assert.AreEqual(rel.SimpleCPRPerson.PersonCivilRegistrationIdentifier, cprNumber, "Same PNR");
            Assert.IsNotNull(rel.SimpleCPRPerson.PersonNameStructure, "PersonNameStructure");
            Assert.IsNotNull(rel.Children, "Children");
            Assert.IsNotNull(rel.Parents, "Parents");
            Assert.IsNotNull(rel.Spouses, "Spouses");
        }

        [Test]
        [NUnit.Framework.Sequential]
        public void T310_SetParentAuthorityOverChild(
           [Range(10000, 10009)] int parentCprNumber,
           [Range(20000, 20009)] int childCprNumber
            )
        {
            string parentCprNumberString = parentCprNumber.ToString();
            string childCprNumberString = childCprNumber.ToString();

            parentCprNumberString += parentCprNumberString;
            childCprNumberString += childCprNumberString;
            var ret = TestRunner.PersonService.SetParentAuthorityOverChild(parentCprNumberString, childCprNumberString);
            Assert.IsTrue(ret);
        }

        [Test]
        [NUnit.Framework.Sequential]
        public void T320_RemoveParentAuthorityOverChild(
           [Range(10000, 10009)] int parentCprNumber,
           [Range(20000, 20009)] int childCprNumber
            )
        {
            string parentCprNumberString = parentCprNumber.ToString();
            string childCprNumberString = childCprNumber.ToString();

            parentCprNumberString += parentCprNumberString;
            childCprNumberString += childCprNumberString;

            var ret = TestRunner.PersonService.RemoveParentAuthorityOverChild(parentCprNumberString, childCprNumberString);
            Assert.IsTrue(ret);
        }

        [Test]
        public void T330_GetParentAuthorityOverChildChanges(
           [Range(20000, 20009)] int childCprNumber
            )
        {
            string childCprNumberString = childCprNumber.ToString();
            childCprNumberString += childCprNumberString;
            var ret = TestRunner.PersonService.GetParentAuthorityOverChildChanges(childCprNumberString);
            Assert.IsNotNull(ret);
            Assert.Greater(ret.Length, 0);
        }

        [Test]
        public void T400_GetCapabilities()
        {
            var cap = TestRunner.AdminService.GetCapabilities();
            Assert.IsNotNull(cap);
            Assert.Greater(cap.Count(), 0);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CorrectMethodNamesFieldName)]
        [TestCaseSource(typeof(TestData), TestData.IncorrectMethodNamesFieldName)]
        public void T410_IsImplementing(string serviceName)
        {
            bool imp = TestRunner.AdminService.IsImplementing(serviceName, TestData.serviceVersion);
            Assert.AreEqual(Array.IndexOf<string>(TestData.correctMethodNames, serviceName) != -1, imp);
            Assert.AreNotEqual(Array.IndexOf<string>(TestData.incorrectMethodNames, serviceName) != -1, imp);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersToSubscribeFieldName)]
        public void T500_SubscribeOnBirthdate(string[] cprNumbers)
        {
            var sub = TestRunner.AdminService.SubscribeOnBirthdate(TestData.fileShareChannel, TestData.birthdateYears, TestData.birthdateDays, cprNumbers);
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
                baseCprNumber = subscription.PersonCivilRegistrationIdentifiers[0];
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
            var sub = TestRunner.AdminService.Subscribe(TestData.fileShareChannel, cprNumbers);
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

        [Test]
        [TestCaseSource(typeof(TestData), TestData.LogTextFieldName)]
        public void T600_Log(string text)
        {
            bool ret = TestRunner.AdminService.Log(text);
            Assert.IsTrue(ret);
        }

        [Test]
        public void T700_CreateTestCitizen()
        {

            bool res = TestRunner.AdminService.CreateTestCitizen(TestData.testFullPerson);
            Assert.IsTrue(res);
        }

        

    }
}
