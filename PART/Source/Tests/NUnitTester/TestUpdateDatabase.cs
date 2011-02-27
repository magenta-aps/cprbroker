using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnitTester.CPRPersonWS;

namespace NUnitTester
{
    /// <summary>
    /// Used to test database updates.
    /// The main idea is to call a method, clear the external data providers and then call same method again
    /// This time the result will come from cpr cache
    /// The two results should be compared
    /// </summary>
    [NUnit.Framework.TestFixture]
    public class TestUpdateDatabase
    {
        static CPRAdministrationWS.DataProviderType[] OriginalDataProviders;

        [TestFixtureSetUp]
        public void Initialize()
        {
            // Ensure that SetDataProviderList works properly
            TestRunner.Initialize();
            var originalProviders = TestRunner.AdminService.GetDataProviderList();
            Assert.IsNotNull(originalProviders);
            Assert.Greater(originalProviders.Length, 0);
            TestRunner.AdminService.SetDataProviderList(originalProviders);
            var setProviders = TestRunner.AdminService.GetDataProviderList();
            Assert.IsTrue(Utilities.AreEqual<CPRAdministrationWS.DataProviderType[]>(originalProviders, setProviders), "Set data providers");
            OriginalDataProviders = originalProviders;
        }

        /// <summary>
        /// Restores the original list of data providers
        /// </summary>
        [TearDown]
        public void RestoreOriginalDataProviders()
        {
            Assert.IsTrue(TestRunner.AdminService.SetDataProviderList(OriginalDataProviders), "Restore providers");
        }

        public void ClearDataProviders()
        {
            Console.WriteLine(OriginalDataProviders.Length);
            foreach (CPRAdministrationWS.DataProviderType d in OriginalDataProviders)
            {
                Console.WriteLine(d.ToString());
            }
            //Assert.IsTrue(TestRunner.AdminService.SetDataProviderList(new NUnitTester.CPRAdministrationWS.DataProviderType[0]), "Clear Providers");
            Assert.IsTrue(TestRunner.AdminService.SetDataProviderList(new NUnitTester.CPRAdministrationWS.DataProviderType[] { OriginalDataProviders[1] }), "Use second provider");
            var current = TestRunner.AdminService.GetDataProviderList();
            Console.WriteLine(current.Length);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]

        public void T01_GetCitizenNameAndAddress(string cprNumber)
        {
            var val1 = TestRunner.PersonService.GetCitizenNameAndAddress(cprNumber);
            ClearDataProviders();
            var val2 = TestRunner.PersonService.GetCitizenNameAndAddress(cprNumber);
            Assert.IsTrue(Utilities.AreEqual<PersonNameAndAddressStructureType>(val1, val2), "NameAndAddress");
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T02_GetCitizenBasic(string cprNumber)
        {
            var val1 = TestRunner.PersonService.GetCitizenBasic(cprNumber);
            ClearDataProviders();
            var val2 = TestRunner.PersonService.GetCitizenBasic(cprNumber);
            Assert.IsTrue(Utilities.AreEqual<PersonBasicStructureType>(val1, val2), "Basic citizen", cprNumber);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T03_GetCitizenFull(string cprNumber)
        {
            var val1 = TestRunner.PersonService.GetCitizenFull(cprNumber);
            ClearDataProviders();
            var val2 = TestRunner.PersonService.GetCitizenFull(cprNumber);

            Assert.IsTrue(Utilities.AreEqual<PersonFullStructureType>(val1, val2), "Full citizen", cprNumber);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersWithChildrenFieldName)]
        public void T04_GetCitizenChildren(
            string cprNumber
            )
        {
            bool includeChildren = true;
            var val1 = TestRunner.PersonService.GetCitizenChildren(cprNumber, includeChildren);
            val1 = val1.OrderBy((p) => p.PersonCivilRegistrationIdentifier).ToArray();

            ClearDataProviders();

            var val2 = TestRunner.PersonService.GetCitizenChildren(cprNumber, includeChildren);
            val2 = val2.OrderBy((p) => p.PersonCivilRegistrationIdentifier).ToArray();

            Assert.IsTrue(Utilities.AreEqual<SimpleCPRPersonType[]>(val1, val2), "Children", cprNumber);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T05_GetCitizenRelations(string cprNumber)
        {
            var val1 = TestRunner.PersonService.GetCitizenRelations(cprNumber);
            val1.Parents = val1.Parents.OrderBy((p) => p.SimpleCPRPerson.PersonCivilRegistrationIdentifier).ToArray();
            val1.Children = val1.Children.OrderBy((p) => p.SimpleCPRPerson.PersonCivilRegistrationIdentifier).ToArray();
            val1.Spouses = val1.Spouses.OrderBy((p) => p.SimpleCPRPerson.PersonCivilRegistrationIdentifier).ToArray();
            val1.AuthoritativeParents = val1.AuthoritativeParents.OrderBy((p) => p.SimpleCPRPerson.PersonCivilRegistrationIdentifier).ToArray();
            val1.CustodiedChildren = val1.CustodiedChildren.OrderBy((p) => p.SimpleCPRPerson.PersonCivilRegistrationIdentifier).ToArray();

            ClearDataProviders();

            var val2 = TestRunner.PersonService.GetCitizenRelations(cprNumber);
            val2.Parents = val2.Parents.OrderBy((p) => p.SimpleCPRPerson.PersonCivilRegistrationIdentifier).ToArray();
            val2.Children = val2.Children.OrderBy((p) => p.SimpleCPRPerson.PersonCivilRegistrationIdentifier).ToArray();
            val2.Spouses = val2.Spouses.OrderBy((p) => p.SimpleCPRPerson.PersonCivilRegistrationIdentifier).ToArray();
            val2.AuthoritativeParents = val2.AuthoritativeParents.OrderBy((p) => p.SimpleCPRPerson.PersonCivilRegistrationIdentifier).ToArray();
            val2.CustodiedChildren = val2.CustodiedChildren.OrderBy((p) => p.SimpleCPRPerson.PersonCivilRegistrationIdentifier).ToArray();

            Assert.IsTrue(Utilities.AreEqual<PersonRelationsType>(val1, val2), "Relations", cprNumber);
        }


    }
}