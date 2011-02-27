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
        public void Validate(Admin.StandardReturType ret)
        {
            Assert.IsNotNull(ret);
            base.Validate(ret.StatusKode, ret.FejlbeskedTekst);
        }

        [Test]
        public void T010_RequestAppRegistration()
        {
            string newAppName = TestData.AppNamePrefix + new Random().Next(10000, int.MaxValue);
            var res = TestRunner.AdminService.RequestAppRegistration(TestRunner.AdminApplicationHeader, newAppName);
            Assert.IsNotNull(res);
            Assert.IsNotNull(res.StandardRetur);
            Assert.AreEqual(res.Item.Name, newAppName);

            TestData.AppToken = res.Item.Token;
        }

        [Test]
        public void T020_ApproveAppRegistration()
        {
            var result = TestRunner.AdminService.ApproveAppRegistration(TestRunner.AdminApplicationHeader, TestData.AppToken);
            Validate(result.StandardRetur);
            Assert.IsTrue(result.Item);

            //TestRunner.AdminService.ApplicationHeaderValue.ApplicationToken = TestData.AppToken;
            //TestRunner.PersonService.ApplicationHeaderValue.ApplicationToken = TestData.AppToken;
        }

        [Test]
        public void T030_ListAppRegistrations()
        {
            var result = TestRunner.AdminService.ListAppRegistrations(TestRunner.AdminApplicationHeader);
            Assert.IsNotNull(result);
            Validate(result.StandardRetur);
            var targetApp = (from app in result.Item where app.Token == TestData.AppToken select app).SingleOrDefault();
            Assert.IsNotNull(targetApp);
            Assert.Greater(targetApp.RegistrationDate, DateTime.Today);
        }

        [Test]
        public void T100_GetDataProviderList()
        {
            var dataProviders = TestRunner.AdminService.GetDataProviderList(TestRunner.AdminApplicationHeader);
            Assert.IsNotNull(dataProviders);
            Validate(dataProviders.StandardRetur);
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
            var dataProviders = TestRunner.AdminService.GetDataProviderList(TestRunner.AdminApplicationHeader);
            Assert.IsNotNull(dataProviders);
            Validate(dataProviders.StandardRetur);
            Assert.IsNotNull(dataProviders.Item);

            if (dataProviders.Item.Length > 1)
            {
                // Use 1 data provider
                var partialUpdateResult = TestRunner.AdminService.SetDataProviderList(TestRunner.AdminApplicationHeader, dataProviders.Item.Take(1).ToArray());
                Assert.IsTrue(partialUpdateResult.Item);

                var partialDataProviders = TestRunner.AdminService.GetDataProviderList(TestRunner.AdminApplicationHeader);
                Assert.AreEqual(1, partialDataProviders.Item.Length);
            }
            else
            {
                Console.WriteLine("Less than 2 providers exist, ignoring partial set");
            }

            var result = TestRunner.AdminService.SetDataProviderList(TestRunner.AdminApplicationHeader, dataProviders.Item);
            Assert.IsTrue(result.Item);
        }

        #region Legacy methods
        [Test]
        [TestCaseSource(typeof(TestData), TestData.CorrectMethodNamesFieldName)]
        [TestCaseSource(typeof(TestData), TestData.IncorrectMethodNamesFieldName)]
        public void T410_IsImplementing(string serviceName)
        {
            var imp = TestRunner.AdminService.IsImplementing(TestRunner.AdminApplicationHeader, serviceName, TestData.serviceVersion);
            Validate(imp.StandardRetur);
            Assert.AreEqual(Array.IndexOf<string>(TestData.correctMethodNames, serviceName) != -1, imp.Item);
            Assert.AreNotEqual(Array.IndexOf<string>(TestData.incorrectMethodNames, serviceName) != -1, imp.Item);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.LogTextFieldName)]
        public void T600_Log(string text)
        {
            var ret = TestRunner.AdminService.Log(TestRunner.AdminApplicationHeader, text);
            Assert.NotNull(ret);
            Validate(ret.StandardRetur);
            Assert.IsTrue(ret.Item);
        }
        #endregion

        [Test]
        public void T990_UnregisterApp()
        {
            var res = TestRunner.AdminService.UnregisterApp(TestRunner.AdminApplicationHeader, TestData.AppToken);
            Assert.NotNull(res);
            Assert.IsTrue(res.Item);
        }
    }
}
