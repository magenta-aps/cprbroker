using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NUnitTester
{
    public abstract class BaseTest
    {
        [TestFixtureSetUp]
        public void Initialize()
        {
            TestRunner.Initialize();
            TestData.Initialize();
        }

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

            TestRunner.AdminService.ApplicationHeaderValue.ApplicationToken = TestData.AppToken;
            TestRunner.PersonService.ApplicationHeaderValue.ApplicationToken = TestData.AppToken;
        }



        [Test]
        public void T990_UnregisterApp()
        {
            bool res = TestRunner.AdminService.UnregisterApp(TestData.AppToken);
            Assert.IsTrue(res);
        }
    }
}
