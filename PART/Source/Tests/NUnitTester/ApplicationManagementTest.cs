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
            Validate(result.StandardRetur);
            Assert.IsTrue(result.Item);

            //TestRunner.AdminService.ApplicationHeaderValue.ApplicationToken = TestData.AppToken;
            //TestRunner.PersonService.ApplicationHeaderValue.ApplicationToken = TestData.AppToken;
        }

        [Test]
        public void T030_ListAppRegistrations()
        {
            var result = TestRunner.AdminService.ListAppRegistrations();
            Assert.IsNotNull(result);
            Validate(result.StandardRetur);
            var targetApp = (from app in result.Item where app.Token == TestData.AppToken select app).SingleOrDefault();
            Assert.IsNotNull(targetApp);
            Assert.Greater(targetApp.RegistrationDate, DateTime.Today);
        }

        [Test]
        public void T100_GetDataProviderList()
        {
            var dataProviders = TestRunner.AdminService.GetDataProviderList();
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
            var dataProviders = TestRunner.AdminService.GetDataProviderList();
            Assert.IsNotNull(dataProviders);
            Validate(dataProviders.StandardRetur);
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
            var imp = TestRunner.AdminService.IsImplementing(serviceName, TestData.serviceVersion);
            Validate(imp.StandardRetur);
            Assert.AreEqual(Array.IndexOf<string>(TestData.correctMethodNames, serviceName) != -1, imp.Item);
            Assert.AreNotEqual(Array.IndexOf<string>(TestData.incorrectMethodNames, serviceName) != -1, imp.Item);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.LogTextFieldName)]
        public void T600_Log(string text)
        {
            var ret = TestRunner.AdminService.Log(text);
            Assert.NotNull(ret);
            Validate(ret.StandardRetur);
            Assert.IsTrue(ret.Item);
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
