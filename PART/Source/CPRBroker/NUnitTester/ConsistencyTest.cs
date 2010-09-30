using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NUnitTester
{
    /// <summary>
    /// Contains  methods that test the data consistency between various data providers
    /// Failure in these tests does not necessarily mean an error, especially in address data
    /// </summary>
    [TestFixture]
    public class ConsistencyTest : BaseTest
    {
        #region Tests
        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T200_GetCitizenNameAndAddress(string cprNumber)
        {
            RunConsistencyTest<CPRPersonWS.PersonNameAndAddressStructureType>(
                () => TestRunner.PersonService.GetCitizenNameAndAddress(cprNumber),
                (p) =>
                {
                    p.Item = IgnoreAddress(p.Item, NUnitTester.CPRPersonWS.PersonCivilRegistrationStatusCodeType.Item90);
                    return p;
                }
                );
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T300_GetCitizenBasic(string cprNumber)
        {
            RunConsistencyTest<CPRPersonWS.PersonBasicStructureType>(
                () => TestRunner.PersonService.GetCitizenBasic(cprNumber),
                (p) =>
                {
                    p.Item = IgnoreAddress(p.Item, p.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusCode);
                    p.RegularCPRPerson = IgnoreRegularCprPerson(p.RegularCPRPerson);
                    return p;
                }
                );
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T400_GetCitizenFull(string cprNumber)
        {
            // DPR returns "Not access to CPR"
            if (cprNumber == "1410733141")
                return;

            RunConsistencyTest<CPRPersonWS.PersonFullStructureType>(
                () => TestRunner.PersonService.GetCitizenFull(cprNumber),
                (p) =>
                {
                    p.Item = IgnoreAddress(p.Item, p.RegularCPRPerson.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusCode);
                    p.RegularCPRPerson = IgnoreRegularCprPerson(p.RegularCPRPerson);

                    // Number of children is unavoidable because DB provider returns 0 if it has not been updated by GetCitizenChildren
                    p.NumberOfChildren = 0;

                    // Spouse name is unavoidable because DB provider returns 0 if it has not been updated by GetCitizenRelations
                    p.SpouseName = "";
                    return p;
                }
                );
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T500_GetCitizenChildren(string cprNumber)
        {
            // Note : CPR number 2101773607 returns an error from KMD
            if (cprNumber == "2101773607")
            {
                return;
            }
            RunConsistencyTest<CPRPersonWS.SimpleCPRPersonType[]>(
                () => TestRunner.PersonService.GetCitizenChildren(cprNumber, false),
                (c) =>
                {
                    return c;
                }
                );
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void T600_GetCitizenRelations(string cprNumber)
        {
            // DPR returns "No access to CPR" for 1410733141 & 2301813506
            if (cprNumber == "1410733141" || cprNumber == "2301813506")
            {
                return;
            }
            // DPR does not return parents if they are not already in database (for 1805812172)            
            if (cprNumber == "1805812172")
            {
                return;
            }

            RunConsistencyTest<CPRPersonWS.PersonRelationsType>(
                () => TestRunner.PersonService.GetCitizenRelations(cprNumber),
                (relations) =>
                {
                    // KMD cannot get previous marriages, so we keep only the last one
                    if (relations != null)
                    {
                        if (relations.Spouses.Length > 0)
                        {
                            relations.Spouses.Last().RelationStartDate = null;
                            if (relations.Spouses.Length > 1)
                            {
                                relations.Spouses = new NUnitTester.CPRPersonWS.MaritalRelationshipType[] { relations.Spouses[relations.Spouses.Length - 1] };
                            }
                        }

                    }
                    return relations;
                }
                );
            var rel = TestRunner.PersonService.GetCitizenRelations(cprNumber);
        }
        #endregion

        #region Ignore methods
        private bool IsStatusCodeLessThan10(CPRPersonWS.PersonCivilRegistrationStatusCodeType status)
        {
            return new NUnitTester.CPRPersonWS.PersonCivilRegistrationStatusCodeType[]
                            {
                                NUnitTester.CPRPersonWS.PersonCivilRegistrationStatusCodeType.Item01,
                                NUnitTester.CPRPersonWS.PersonCivilRegistrationStatusCodeType.Item03,
                                NUnitTester.CPRPersonWS.PersonCivilRegistrationStatusCodeType.Item05,
                                NUnitTester.CPRPersonWS.PersonCivilRegistrationStatusCodeType.Item07
                            }.Contains(status);
        }
        private CPRPersonWS.RegularCPRPersonType IgnoreRegularCprPerson(CPRPersonWS.RegularCPRPersonType p)
        {
            // Status code is unavoidable because KMD always returns either 01 or >10
            if (IsStatusCodeLessThan10(p.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusCode))
            {
                p.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusCode = CPRPersonWS.PersonCivilRegistrationStatusCodeType.Item01;
            }

            // Status date is unavoidable because StatusDate in DPR is sometimes null, so the AddressDate is used instead
            p.PersonCivilRegistrationStatusStructure.PersonCivilRegistrationStatusStartDate = DateTime.Today;
            return p;
        }

        private object IgnoreAddress(object address, CPRPersonWS.PersonCivilRegistrationStatusCodeType personCivilRegistrationStatus)
        {
            //if (!IsStatusCodeLessThan10(personCivilRegistrationStatus))
            {
                address = null;
            }
            return address;
        }
        #endregion

        #region Setup & Teardown
        private CPRAdministrationWS.DataProviderType[] OriginalDataProviders;

        [SetUp]
        public void GetDataProviders()
        {
            OriginalDataProviders = TestRunner.AdminService.GetDataProviderList();
        }

        [TearDown]
        public void RestoreDataProviders()
        {
            TestRunner.AdminService.SetDataProviderList(OriginalDataProviders);
        }

        void RunConsistencyTest<T>(Func<T> method)
        {
            RunConsistencyTest<T>(method, null);
        }
        #endregion

        #region COnsistency methods
        void RunConsistencyTest<T>(Func<T> method, Func<T, T> fieldIgnoreMethod)
        {
            List<CPRAdministrationWS.DataProviderType[]> dataProviders = new List<NUnitTester.CPRAdministrationWS.DataProviderType[]>();
            dataProviders.AddRange(from prov in OriginalDataProviders select new CPRAdministrationWS.DataProviderType[] { prov });
            if (OriginalDataProviders.Length > 0)
            {
                dataProviders.Add(new NUnitTester.CPRAdministrationWS.DataProviderType[0]);
            }

            string lastDataProviderString = null;
            string lastResultString = null;
            T lastResult = default(T);
            for (int i = 0; i < dataProviders.Count; i++)
            {
                TestRunner.AdminService.SetDataProviderList(dataProviders[i]);
                var result = method();
                if (fieldIgnoreMethod != null)
                {
                    result = fieldIgnoreMethod(result);
                }
                string resultString = CPRBroker.Engine.Util.Strings.SerializeObject(result);

                string dataProviderString = DataProviderToString(dataProviders[i]);
                if (i > 0)
                {
                    string message = string.Format("Previous = {0} and Current={1}", lastDataProviderString, dataProviderString);
                    Utilities.AreEqual<T>(dataProviderString, result, lastDataProviderString, lastResult);
                    Assert.AreEqual(resultString, lastResultString, message);
                }
                lastDataProviderString = dataProviderString;
                lastResultString = resultString;
                lastResult = result;
            }

        }

        private string DataProviderToString(CPRAdministrationWS.DataProviderType[] dataProviders)
        {
            return string.Join(",", Array.ConvertAll<CPRAdministrationWS.DataProviderType, string>(dataProviders, (p) => p.GetType().Name));
        }
        #endregion
    }
}
