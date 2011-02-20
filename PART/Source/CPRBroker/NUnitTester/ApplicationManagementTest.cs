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
            var res = TestRunner.AdminService.RequestAppRegistration(newAppName);
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.StandardRetur);
            Assert.AreEqual(res.Item.Name, newAppName);

            TestData.AppToken = res.Item.Token;
        }

        [Test]
        public void T020_ApproveAppRegistration()
        {
            var result = TestRunner.AdminService.ApproveAppRegistration(TestData.AppToken);
            Assert.NotNull(result.StandardRetur);
            Assert.IsTrue(result.Item);

            //TestRunner.AdminService.ApplicationHeaderValue.ApplicationToken = TestData.AppToken;
            //TestRunner.PersonService.ApplicationHeaderValue.ApplicationToken = TestData.AppToken;
        }

        [Test]
        public void T030_ListAppRegistrations()
        {
            var result = TestRunner.AdminService.ListAppRegistrations();
            Assert.IsNotNull(result);
            var targetApp = (from app in result.Item where app.Token == TestData.AppToken select app).SingleOrDefault();
            Assert.IsNotNull(targetApp);
            Assert.Greater(targetApp.RegistrationDate, DateTime.Today);
        }

        [Test]
        public void T100_GetDataProviderList()
        {
            var dataProviders = TestRunner.AdminService.GetDataProviderList();
            Assert.IsNotNull(dataProviders);
            Assert.IsNotNull(dataProviders.Item);
            foreach (var dataProvider in dataProviders.Item)
            {
                Assert.NotNull(dataProvider);
                Assert.IsNotEmpty(dataProvider.TypeName);
                Assert.IsNotEmpty(dataProvider.Attributes);
            }
        }

        public void T110_GetAndSetDataProviderList()
        {
            var dataProviders = TestRunner.AdminService.GetDataProviderList();
            Assert.IsNotNull(dataProviders);
            Assert.IsNotNull(dataProviders.Item);

            if (dataProviders.Item.Length > 1)
            {
                // Use 1 data provider
                var partialUpdateResult = TestRunner.AdminService.SetDataProviderList(dataProviders.Item.Take(1).ToArray());
                Assert.IsTrue(partialUpdateResult.Item);

                var partialDataProviders = TestRunner.AdminService.GetDataProviderList();
                Assert.AreEqual(1, partialDataProviders.Item.Length);
            }
            else
            {
                Console.WriteLine("Less than 2 providers exist, ignoring partial set");
            }

            var result = TestRunner.AdminService.SetDataProviderList(dataProviders.Item);
            Assert.IsTrue(result.Item);
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
            var res = TestRunner.AdminService.UnregisterApp(TestData.AppToken);
            Assert.NotNull(res);
            Assert.IsTrue(res.Item);
        }
    }
}
