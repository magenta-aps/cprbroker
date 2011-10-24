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
    public class ChildTests
    {
        [TearDown]
        public void ClearUUIDList()
        {
            lastCprNumber = null;
            lastUuid = Guid.Empty;
        }

        string lastCprNumber;
        Guid lastUuid;

        Guid ToUuid(string cprNumber)
        {
            var ret = Guid.NewGuid();
            lastCprNumber = cprNumber;
            lastUuid = ret;
            return ret;
        }
        decimal?[] TestCprNumbers
        {
            get { return UnitTests.RandomCprNumbers(3); }
        }

        private Child CreateChild(decimal? cprNumber)
        {
            return new Child() { PNR = cprNumber };
        }

        decimal?[] WrongCprNumbers = new decimal?[] { null, 10m, 23m };

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToPersonFlerRelationType_WrongPNR_ThrowsException(
            [ValueSource("WrongCprNumbers")] decimal? cprNumber)
        {
            var child = CreateChild(cprNumber);
            Child.ToPersonFlerRelationType(child, ToUuid);
        }

        [Test]
        [TestCaseSource("TestCprNumbers")]
        public void ToPersonFlerRelationType_Valid_NotNull(decimal? cprNumber)
        {
            var child = CreateChild(cprNumber);
            var result = Child.ToPersonFlerRelationType(child, ToUuid);
            Assert.IsNotNull(result);
        }

        [Test]
        [TestCaseSource("TestCprNumbers")]
        public void ToPersonFlerRelationType_Valid_HasReferenceID(decimal? cprNumber)
        {
            var child = CreateChild(cprNumber);
            var result = Child.ToPersonFlerRelationType(child, ToUuid);
            Assert.IsNotNull(result.ReferenceID);
        }

        [Test]
        [TestCaseSource("TestCprNumbers")]
        public void ToPersonFlerRelationType_Valid_CorrectCprNumberPassed(decimal? cprNumber)
        {
            var child = CreateChild(cprNumber);
            var result = Child.ToPersonFlerRelationType(child, ToUuid);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.AreEqual(lastCprNumber, stringCprNumber);
        }

        [Test]
        [TestCaseSource("TestCprNumbers")]
        public void ToPersonFlerRelationType_Valid_ValidUuid(decimal? cprNumber)
        {
            var child = CreateChild(cprNumber);
            var result = Child.ToPersonFlerRelationType(child, ToUuid);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.AreEqual(lastUuid.ToString(), result.ReferenceID.Item);
        }

    }
}
