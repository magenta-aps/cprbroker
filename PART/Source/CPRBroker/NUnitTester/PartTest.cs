using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NUnitTester
{
    [TestFixture]
    public class PartTest : BaseTest
    {
        #region App registrations
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
        #endregion

        #region Part person methods

        private void ValidatePerson(Guid uuid, Part.PersonRegistration person, Part.Part service)
        {
            Assert.IsNotNull(person, "Person not found : {0}", uuid);
            Assert.AreNotEqual(Guid.Empty, person.ActorId);

            Assert.IsNotNull(service.QualityHeaderValue, "Quality header");
            Assert.IsNotNull(service.QualityHeaderValue.QualityLevel, "Quality header value");
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T200_GetPersonUuid(string cprNumber)
        {
            var uuid = TestRunner.PartService.GetPersonUuid(cprNumber);
            Assert.AreNotEqual(uuid, Guid.Empty);

            var uuid2 = TestRunner.PartService.GetPersonUuid(cprNumber);
            Assert.AreEqual(uuid, uuid2);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T200_Read(string cprNumber)
        {
            var uuid = TestRunner.PartService.GetPersonUuid(cprNumber);
            Assert.AreNotEqual(uuid, Guid.Empty);

            var person = TestRunner.PartService.Read(uuid);
            ValidatePerson(uuid, person, TestRunner.PartService);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T250_RefreshRead(string cprNumber)
        {
            var uuid = TestRunner.PartService.GetPersonUuid(cprNumber);
            Assert.AreNotEqual(uuid, Guid.Empty);

            var person = TestRunner.PartService.Read(uuid);
            ValidatePerson(uuid, person, TestRunner.PartService);

            var freshPerson = TestRunner.PartService.RefreshRead(uuid);
            ValidatePerson(uuid, freshPerson, TestRunner.PartService);
            Assert.AreNotEqual(TestRunner.PartService.QualityHeaderValue.QualityLevel.Value, Part.QualityLevel.LocalCache);
        }

        [Test]
        [TestCaseSource(typeof(PartTestData), PartTestData.PersonUUIDsArrayFieldName)]
        public void T300_List(Guid[] personUuids)
        {
            var persons = TestRunner.PartService.List(personUuids);
            Assert.IsNotNull(persons, "Persons array is null");
            Assert.AreEqual(personUuids.Length, personUuids.Length, "Incorrect length of returned array");
            for (int i = 0; i < personUuids.Length; i++)
            {
                ValidatePerson(personUuids[i], persons[i], TestRunner.PartService);
            }
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T400_Search_CprNumber(string cprNumber)
        {
            var searchCriteria = new Part.PersonSearchCriteria()
            {
                CprNumber = cprNumber
            };
            var result = TestRunner.PartService.Search(searchCriteria);
            Assert.IsNotNull(result, "Search result");

            var personUuid = TestRunner.PartService.GetPersonUuid(cprNumber);
            if (result.Length == 0)
            {
                var personObject = TestRunner.PartService.Read(personUuid);
            }
            Assert.AreEqual(1, result.Length, "Number of search results");
            Assert.AreNotEqual(Guid.Empty, result[0], "Empty person uuid from search");
            Assert.AreEqual(personUuid, result[0], "Search result returns wrong uuids");
        }

        // TODO: Add more methods to test Search for criteria other than CPR number
        #endregion

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


    }
}
