using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.E_M;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.E_M
{
    [TestFixture]
    partial class CitizenRelationsTests
    {
        private Dictionary<string, Guid> uuids = new Dictionary<string, Guid>();
        [TearDown]
        public void ClearUUIDList()
        {
            uuids.Clear();
        }
        Guid ToUuid(string cprNumber)
        {
            var ret = Guid.NewGuid();
            uuids[cprNumber] = ret;
            return ret;
        }

        char? DateCertaintyTrue = 'T';
        public decimal?[] RandomCprNumbers
        {
            get { return Utilities.RandomCprNumbers(10); }
        }
        public decimal?[] RandomCprNumbers1
        {
            get { return Utilities.RandomCprNumbers(1); }
        }
        public decimal?[] InvalidCprNumbers = new decimal?[] { 10m, 6567576m };
        public decimal?[] InvalidCprNumbersWithNull = new decimal?[] { null, 10m, 6567576m };        

        private void AssertCprNumbers(decimal?[] cprNumbers, PersonFlerRelationType[] result)
        {
            Assert.AreEqual(cprNumbers.Length, result.Length);
            for (int i = 0; i < cprNumbers.Length; i++)
            {
                var stringCprNumber = Converters.ToCprNumber(cprNumbers[i]);
                Assert.IsNotNull(result[i]);
                Assert.IsNotNull(result[i].ReferenceID);
                Assert.IsTrue(uuids.ContainsKey(stringCprNumber));
                Assert.AreEqual(uuids[stringCprNumber].ToString(), result[i].ReferenceID.Item);
            }
        }

        private void AssertCprNumbers(decimal?[] cprNumbers, PersonRelationType[] result)
        {
            Assert.AreEqual(cprNumbers.Length, result.Length);
            for (int i = 0; i < cprNumbers.Length; i++)
            {
                var stringCprNumber = Converters.ToCprNumber(cprNumbers[i]);
                Assert.IsNotNull(result[i]);
                Assert.IsNotNull(result[i].ReferenceID);
                Assert.IsTrue(uuids.ContainsKey(stringCprNumber));
                Assert.AreEqual(uuids[stringCprNumber].ToString(), result[i].ReferenceID.Item);
            }
        }

        [Test]
        [Combinatorial]
        public void TestToChildren(
            [Random(0, 100, 10)] int count,
            [Values('M', 'K')] char gender)
        {
            var citizen = new Citizen();
            var childCprNumbers = Utilities.RandomCprNumbers(count);
            var children = childCprNumbers.Select(pnr => new Child() { PNR = pnr });
            citizen.Gender = gender;
            var childrenEntitySet = gender == 'M' ? citizen.ChildrenAsFather : citizen.ChildrenAsMother;
            childrenEntitySet.AddRange(children);
            var result = Citizen.ToChildren(citizen, ToUuid);
            AssertCprNumbers(childCprNumbers, result);
        }

        DateTime?[] TestDates = new DateTime?[] { null, new DateTime(2010, 5, 5), new DateTime(2011, 7, 7) };

        [Test]
        [Combinatorial]
        public void TestToSpouses(
            [ValueSource("RandomCprNumbers1")] decimal? cprNumber,
            [Values('G', 'F', 'E')]char? maritalStatus,
            [ValueSource("TestDates")] DateTime? maritalStatusDate,
            [Values('T')]char? maritalStatusDateUncertainty,
            [ValueSource("TestDates")]DateTime? maritalStatusEndDate,
            [Values('T')]char? maritalStatusEndDateUncertainty)
        {
            var citizen = new Citizen()
            {
                SpousePNR = cprNumber,
                MaritalStatus = maritalStatus,
                MaritalStatusTimestamp = maritalStatusDate,
                MaritalStatusTimestampUncertainty = maritalStatusDateUncertainty,
                MaritalStatusTerminationTimestamp = maritalStatusEndDate,
                MaritalStatusTerminationTimestampUncertainty = maritalStatusEndDateUncertainty
            };

            var result = Citizen.ToSpouses(citizen, ToUuid);
            AssertCprNumbers(new decimal?[] { cprNumber }, result);
        }


        #region ToRegisteredPartners

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToRegisteredPartners_Null_ThrowsException()
        {
            Citizen.ToRegisteredPartners(null, ToUuid);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToRegisteredPartners_NullConverter_ThrowsException()
        {
            var citizen = new Citizen() { SpousePNR = Utilities.RandomCprNumber() };
            Citizen.ToRegisteredPartners(citizen, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        [TestCaseSource("InvalidCprNumbers")]
        public void ToRegisteredPartners_InvalidSpousePNR_ThrowsException(decimal? spousePNR)
        {
            var citizen = new Citizen() { SpousePNR = spousePNR };
            Citizen.ToRegisteredPartners(citizen, ToUuid);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToRegisteredPartners_InvalidMaritalStatus_ThrowsException(
            [Values(null, 'W')] char? maritalStatus)
        {
            var citizen = new Citizen() { SpousePNR = Utilities.RandomCprNumber(), MaritalStatus = maritalStatus };
            Citizen.ToRegisteredPartners(citizen, ToUuid);
        }

        [Test]
        public void ToRegisteredPartners_IrrelevantMaritalStatus_NotNull(
            [Values('E', 'F', 'G', 'U')] char? maritalStatus)
        {
            var citizen = new Citizen() { SpousePNR = Utilities.RandomCprNumber(), MaritalStatus = maritalStatus };
            var result = Citizen.ToRegisteredPartners(citizen, ToUuid);
            Assert.NotNull(result);
        }

        [Test]
        public void ToRegisteredPartners_IrrelevantMaritalStatus_ZeroElements(
            [Values('E', 'F', 'G', 'U')] char? maritalStatus)
        {
            var citizen = new Citizen() { SpousePNR = Utilities.RandomCprNumber(), MaritalStatus = maritalStatus };
            var result = Citizen.ToRegisteredPartners(citizen, ToUuid);
            Assert.AreEqual(0, result.Length);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToRegisteredPartners_InconsistentDates_ThrowsException(
            [Values('L', 'O')]char? maritalStatus)
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = maritalStatus,
                MaritalStatusTimestamp = DateTime.Today,
                MaritalStatusTimestampUncertainty = DateCertaintyTrue,
                MaritalStatusTerminationTimestamp = DateTime.Today.AddDays(-2),
                MaritalStatusTerminationTimestampUncertainty = DateCertaintyTrue
            };
            Citizen.ToRegisteredPartners(citizen, ToUuid);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToRegisteredPartners_TerminationDateForUnTerminatedPartnership_ThrowsException()
        {
            var citizen = new Citizen()
            {
                SpousePNR = Utilities.RandomCprNumber(),
                MaritalStatus = 'P',
                MaritalStatusTerminationTimestamp = DateTime.Today.AddDays(-2),
                MaritalStatusTerminationTimestampUncertainty = DateCertaintyTrue
            };
            Citizen.ToRegisteredPartners(citizen, ToUuid);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToRegisteredPartners_InvalidSpousePnr_Exception(
            [Values('L', 'O', 'P')]char? maritalStatus,
            [ValueSource("InvalidCprNumbers")]decimal? spousePnr)
        {
            var citizen = new Citizen() { MaritalStatus = maritalStatus, SpousePNR = spousePnr };
            Citizen.ToRegisteredPartners(citizen, ToUuid);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToRegisteredPartners_NullSpousePnr_Exception(
            [Values('L', 'O', 'P')]char? maritalStatus)
        {
            var citizen = new Citizen() { MaritalStatus = maritalStatus, SpousePNR = null};
            Citizen.ToRegisteredPartners(citizen, ToUuid);
        }

        [Test]
        [Combinatorial]
        public void TestToRegisteredPartners(
            [ValueSource("RandomCprNumbers1")] decimal? cprNumber,
            [Values('P', 'O', 'L')]char? maritalStatus,
            [ValueSource("TestDates")] DateTime? maritalStatusDate,
            [Values('T')]char? maritalStatusDateUncertainty,
            [ValueSource("TestDates")]DateTime? maritalStatusEndDate,
            [Values('T')]char? maritalStatusEndDateUncertainty)
        {
            var citizen = new Citizen()
            {
                SpousePNR = cprNumber,
                MaritalStatus = maritalStatus,
                MaritalStatusTimestamp = maritalStatusDate,
                MaritalStatusTimestampUncertainty = maritalStatusDateUncertainty,
                MaritalStatusTerminationTimestamp = maritalStatusEndDate,
                MaritalStatusTerminationTimestampUncertainty = maritalStatusEndDateUncertainty
            };

            var result = Citizen.ToRegisteredPartners(citizen, ToUuid);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.IsNotNull(result[0]);
            Assert.IsNotNull(result[0].ReferenceID);
            Assert.IsTrue(uuids.ContainsKey(stringCprNumber));
            Assert.AreEqual(uuids[stringCprNumber].ToString(), result[0].ReferenceID.Item);
        }
        #endregion


        #region ToFather
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToFather_Null_ThrowsException()
        {
            Citizen.ToFather(null, ToUuid);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToFather_NullConverter_ThrowsException()
        {
            var citizen = new Citizen();
            citizen.FatherPNR = Utilities.RandomCprNumber();
            Citizen.ToFather(citizen, null);
        }

        [Test]
        [TestCaseSource("InvalidCprNumbersWithNull")]
        [ExpectedException(typeof(ArgumentException))]
        public void ToFather_InvalidFatherPNR_Exception(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            Citizen.ToFather(citizen, ToUuid);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToFather_Valid_NotNull(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            var result = Citizen.ToFather(citizen, ToUuid);
            Assert.NotNull(result);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToFather_Valid_SingleArrayElement(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            var result = Citizen.ToFather(citizen, ToUuid);
            Assert.AreEqual(1, result.Length);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToFather_Valid_ElementNotNull(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            var result = Citizen.ToFather(citizen, ToUuid);
            Assert.NotNull(result[0]);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToFather_Valid_ReferenceIDNotNull(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            var result = Citizen.ToFather(citizen, ToUuid);
            Assert.IsNotNull(result[0].ReferenceID);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToFather_Valid_CorrectPnrPassed(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            var result = Citizen.ToFather(citizen, ToUuid);
            Assert.NotNull(result);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.IsTrue(uuids.ContainsKey(stringCprNumber));
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToFather_Valid_CorrectUuid(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            var result = Citizen.ToFather(citizen, ToUuid);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.AreEqual(uuids[stringCprNumber].ToString(), result[0].ReferenceID.Item);
        }
        #endregion

        #region ToMother
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToMother_Null_ThrowsException()
        {
            Citizen.ToMother(null, ToUuid);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToMother_NullConverter_ThrowsException()
        {
            var citizen = new Citizen();
            citizen.MotherPNR = Utilities.RandomCprNumber();
            Citizen.ToMother(citizen, null);
        }


        [Test]
        [TestCaseSource("InvalidCprNumbersWithNull")]
        [ExpectedException(typeof(ArgumentException))]
        public void ToMother_InvalidMotherPNR_Exception(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            Citizen.ToMother(citizen, ToUuid);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToMother_Valid_NotNull(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            var result = Citizen.ToMother(citizen, ToUuid);
            Assert.NotNull(result);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToMother_Valid_SingleArrayElement(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            var result = Citizen.ToMother(citizen, ToUuid);
            Assert.AreEqual(1, result.Length);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToMother_Valid_ElementNotNull(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            var result = Citizen.ToMother(citizen, ToUuid);
            Assert.NotNull(result[0]);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToMother_Valid_ReferenceIDNotNull(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            var result = Citizen.ToMother(citizen, ToUuid);
            Assert.IsNotNull(result[0].ReferenceID);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToMother_Valid_CorrectPnrPassed(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            var result = Citizen.ToMother(citizen, ToUuid);
            Assert.NotNull(result);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.IsTrue(uuids.ContainsKey(stringCprNumber));
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToMother_Valid_CorrectUuid(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            var result = Citizen.ToMother(citizen, ToUuid);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.AreEqual(uuids[stringCprNumber].ToString(), result[0].ReferenceID.Item);
        }
        #endregion
    }
}
