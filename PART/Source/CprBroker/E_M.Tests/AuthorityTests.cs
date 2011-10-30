using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.E_M;
using NUnit.Framework;

namespace CprBroker.Tests.E_M
{
    [TestFixture]
    class AuthorityTests
    {
        [Test]
        public void ToAuthorityName_Null_ReturnsNull()
        {
            var result = Authority.ToAuthorityName(null);
            Assert.Null(result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToAuthorityName_NullName_ThrowsException()
        {
            Authority.ToAuthorityName(new Authority() { AuthorityCode=123, AuthorityName=null});
        }

        [Test]
        public void ToAuthorityName_Valid_ReturnsCorrectName(
            [Values("Main","CPR", "ITST")] string name)
        {
            var result = Authority.ToAuthorityName(new Authority() { AuthorityName = name });
            Assert.AreEqual(name, result);
        }
    }
}
