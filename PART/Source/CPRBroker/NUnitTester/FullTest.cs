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
