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

        public decimal?[] RandomCprNumbers
        {
            get { return Utilities.RandomCprNumbers(10); }
        }
        public decimal?[] RandomCprNumbers1
        {
            get { return Utilities.RandomCprNumbers(1); }
        }
        public decimal?[] InvalidCprNumbers = new decimal?[] { null, 10m, 6567576m };

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
            AssertCprNumbers(new decimal?[] { cprNumber }, result);
        }


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
        [TestCaseSource("InvalidCprNumbers")]
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
        [TestCaseSource("InvalidCprNumbers")]
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
