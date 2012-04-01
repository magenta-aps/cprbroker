using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.DPR;
using CprBroker.Utilities;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.DPR.PersonAddressTests
{
    [TestFixture]
    public class ToAddressPostalType : BaseTests
    {
        [Test]
        public void ToAddressPostalType_Normal_NotNull()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToAddressPostalType(null);
            Assert.NotNull(result);
        }

        [Test]
        public void ToAddressPostalType_Normal_CorrectPostDistrict(
            [ValueSource("RandomStrings5")]string postDistrictName)
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToAddressPostalType(postDistrictName);
            Assert.AreEqual(postDistrictName, result.DistrictName);
        }

        [Test]
        public void ToAddressPostalType_Normal_DistrictSubdivisionIdentifierNull()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToAddressPostalType(null);
            Assert.Null(result.DistrictSubdivisionIdentifier);
        }

        [Test]
        public void ToAddressPostalType_Normal_CorrectFloor(
            [ValueSource("RandomStrings5")]string floor)
        {
            var personAddress = new PersonAddressStub() { Floor = floor };
            var result = personAddress.ToAddressPostalType(null);
            Assert.AreEqual(floor, result.FloorIdentifier);
        }

        [Test]
        public void ToAddressPostalType_Normal_MailDeliverySublocationIdentifierNull()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToAddressPostalType(null);
            Assert.Null(result.MailDeliverySublocationIdentifier);
        }

        [Test]
        public void ToAddressPostalType_Normal_CorrectPostCodeIdentifier(
            [ValueSource("RandomDecimals5")]decimal postCode)
        {
            var personAddress = new PersonAddressStub() { PostCode = postCode };
            var result = personAddress.ToAddressPostalType(null);
            Assert.AreEqual(postCode, decimal.Parse(result.PostCodeIdentifier));
        }

        [Test]
        public void ToAddressPostalType_Normal_PostOfficeBoxIdentifierNull()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToAddressPostalType(null);
            Assert.Null(result.PostOfficeBoxIdentifier);
        }

        [Test]
        public void ToAddressPostalType_Normal_CorrectStreetBuildingIdentifier(
            [ValueSource("RandomHouseNumbers5")]string houseNumber)
        {
            var personAddress = new PersonAddressStub() { HouseNumber = houseNumber };
            var result = personAddress.ToAddressPostalType(null);
            Assert.AreEqual(houseNumber, result.StreetBuildingIdentifier);
        }

        [Test]
        public void ToAddressPostalType_Normal_CorrectStreetName(
            [ValueSource("RandomStrings5")]string streetName)
        {
            var personAddress = new PersonAddressStub() { StreetAddressingName = streetName };
            var result = personAddress.ToAddressPostalType(null);
            Assert.AreEqual(streetName, result.StreetName);
        }

        [Test]
        public void ToAddressPostalType_Normal_CorrectStreetAddressingName(
            [ValueSource("RandomStrings5")]string streetName)
        {
            var personAddress = new PersonAddressStub() { StreetAddressingName = streetName };
            var result = personAddress.ToAddressPostalType(null);
            Assert.AreEqual(streetName, result.StreetNameForAddressingName);
        }

        [Test]
        public void ToAddressPostalType_Normal_CorrectSuiteIdentifier(
            [ValueSource("RandomStrings5")]string door)
        {
            var personAddress = new PersonAddressStub() { DoorNumber = door };
            var result = personAddress.ToAddressPostalType(null);
            Assert.AreEqual(door, result.SuiteIdentifier);
        }
    }
}
