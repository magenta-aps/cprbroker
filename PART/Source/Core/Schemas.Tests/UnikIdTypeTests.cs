using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.Schemas
{
    [TestFixture]
    class UnikIdTypeTests
    {
        #region Clone

        [Test]
        public void Clone_Null_ReturnsNull()
        {
            var result = UnikIdType.Clone(null);
            Assert.Null(result);
        }

        [Test]
        public void Clone_Valid_CorrectType(
            [Values(ItemChoiceType.URNIdentifikator, ItemChoiceType.UUID)]ItemChoiceType type)
        {
            var unikId = new UnikIdType() { ItemElementName = type };
            var result = UnikIdType.Clone(unikId);
            Assert.AreEqual(type, result.ItemElementName);
        }

        [Test]
        [Combinatorial]
        public void Clone_Valid_CorrectType(
            [Values(ItemChoiceType.URNIdentifikator, ItemChoiceType.UUID)]ItemChoiceType type,
            [Values("fjkjhfjkh", "uiyyuyui")]string value)
        {
            var unikId = new UnikIdType() { ItemElementName = type, Item = value };
            var result = UnikIdType.Clone(unikId);
            Assert.AreEqual(value, result.Item);
        }
        #endregion

    }
}
