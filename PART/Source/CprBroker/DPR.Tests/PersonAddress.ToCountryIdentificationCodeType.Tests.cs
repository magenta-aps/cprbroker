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
using CprBroker.Tests.DPR;

namespace CprBroker.Tests.DPR.PersonAddressTests
{
    [TestFixture]
    public class ToCountryIdentificationCodeTypeTests : BaseTests
    {
        [Test]
        public void ToCountryIdentificationCodeType_Normal_NotNull()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToCountryIdentificationCodeType();
            Assert.NotNull(result);
        }

        [Test]
        public void ToCountryIdentificationCodeType_Normal_imk()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToCountryIdentificationCodeType();
            Assert.AreEqual(_CountryIdentificationSchemeType.imk, result.scheme);
        }

        [Test]
        public void ToCountryIdentificationCodeType_Normal_Denmark()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToCountryIdentificationCodeType();
            Assert.AreEqual("5100", result.Value);
        }

    }
}
