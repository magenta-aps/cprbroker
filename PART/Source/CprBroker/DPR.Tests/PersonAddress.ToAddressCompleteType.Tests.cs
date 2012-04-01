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
    public class ToAddressCompleteType : BaseTests
    {
        [Test]
        public void ToAddressCompleteType_Normal_NotNull()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToAddressCompleteType(null);
            Assert.NotNull(result);
        }

        [Test]
        public void ToAddressCompleteType_Normal_AddressAccessNotNull()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToAddressCompleteType(null);
            Assert.NotNull(result.AddressAccess);
        }

        [Test]
        public void ToAddressCompleteType_Normal_NotAddressPostalNull()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToAddressCompleteType(null);
            Assert.NotNull(result.AddressPostal);
        }

        

    }
}
