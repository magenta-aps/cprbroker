using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace CprBroker.NUnitTester
{
    [TestFixture]
    public class ApplicationManagementTest : BaseTest
    {
        [Test]
        public void T010_RequestAppRegistration()
        {
            string newAppName = TestData.AppNamePrefix + new Random().Next(10000, int.MaxValue);
            var app = TestRunner.AdminService.RequestAppRegistration(newAppName);
            Assert.IsNotNull(app);
            Assert.AreEqual(app.Name, newAppName);

            TestData.AppToken = app.Token;
        }

        [Test]
        public void T020_ApproveAppRegistration()
        {
            bool result = TestRunner.AdminService.ApproveAppRegistration(TestData.AppToken);
            Assert.IsTrue(result);

            //TestRunner.AdminService.ApplicationHeaderValue.ApplicationToken = TestData.AppToken;
            //TestRunner.PersonService.ApplicationHeaderValue.ApplicationToken = TestData.AppToken;
        }

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

        #region Legacy methods
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
        [TestCaseSource(typeof(TestData), TestData.LogTextFieldName)]
        public void T600_Log(string text)
        {
            bool ret = TestRunner.AdminService.Log(text);
            Assert.IsTrue(ret);
        }
        #endregion

        [Test]
        public void T990_UnregisterApp()
        {
            bool res = TestRunner.AdminService.UnregisterApp(TestData.AppToken);
            Assert.IsTrue(res);
        }
    }
}
