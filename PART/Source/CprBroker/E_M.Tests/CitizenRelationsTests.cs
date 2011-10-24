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

        public decimal?[] RandomCprNumbers10
        {
            get { return UnitTests.RandomCprNumbers(10); }
        }
        public decimal?[] RandomCprNumbers1
        {
            get { return UnitTests.RandomCprNumbers(1); }
        }

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
            var childCprNumbers = UnitTests.RandomCprNumbers(count);
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

        [Test]
        [TestCaseSource("RandomCprNumbers10")]
        public void TestToFather(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.FatherPNR = cprNumber;
            var result = Citizen.ToFather(citizen, ToUuid);
            AssertCprNumbers(new decimal?[] { cprNumber }, result);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers10")]
        public void TestToMother(decimal? cprNumber)
        {
            var citizen = new Citizen();
            citizen.MotherPNR = cprNumber;
            var result = Citizen.ToMother(citizen, ToUuid);
            AssertCprNumbers(new decimal?[] { cprNumber }, result);
        }
    }
}
