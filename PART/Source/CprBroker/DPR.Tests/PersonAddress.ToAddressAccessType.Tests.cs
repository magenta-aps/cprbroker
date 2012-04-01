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
    public class ToAddressAccessType : BaseTests
    {
        [Test]
        public void ToAddressAccessType_Normal_NotNull()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToAddressAccessType();
            Assert.NotNull(result);
        }

        [Test]
        public void ToAddressAccessType_Normal_CorrectMunicipalityCode(
            [ValueSource("RandomMunicipalityCodes5")]decimal municipalityCode)
        {
            var personAddress = new PersonAddressStub() { MunicipalityCode = municipalityCode };
            var result = personAddress.ToAddressAccessType();
            Assert.AreEqual(municipalityCode, decimal.Parse(result.MunicipalityCode));
        }

        [Test]
        public void ToAddressAccessType_Normal_CorrectStreetCode(
            [ValueSource("RandomStreetCodes5")]decimal streetCode)
        {
            var personAddress = new PersonAddressStub() { StreetCode = streetCode };
            var result = personAddress.ToAddressAccessType();
            Assert.AreEqual(streetCode, decimal.Parse(result.StreetCode));
        }

        [Test]
        public void ToAddressAccessType_Normal_CorrectHouseNumber(
            [ValueSource("RandomHouseNumbers5")]string houseNumber)
        {
            var personAddress = new PersonAddressStub() { HouseNumber = houseNumber };
            var result = personAddress.ToAddressAccessType();
            Assert.AreEqual(houseNumber, result.StreetBuildingIdentifier);
        }

    }
}
