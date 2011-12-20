using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Utilities;

namespace CprBroker.Tests.Utilities
{
    [TestFixture]
    class StringsTests
    {
        string[] ValidHostNames = new string[] { "hotmail.com", "personMaster", "1PersonMaster", "999pms.dk", "m.a.a", "code-valley", "c.m-ee.12kl", "ddd234", "kkk---ppp", "88-22", "f.92.15xyz", "12.code.abc" };
        string[] InvalidHostNames = new string[] { "", "-ddd", "def-", "22", "77.76", "klm.", "..", "....." };

        [Test]
        [TestCaseSource("ValidHostNames")]
        public void IsValidHostName_OK_ReturnsTrue(string hostName)
        {
            var result = CprBroker.Utilities.Strings.IsValidHostName(hostName);
            Assert.True(result);
        }

        [Test]
        [TestCaseSource("InvalidHostNames")]
        public void IsValidHostName_Invalid_ReturnsFalse(string hostName)
        {
            var result = CprBroker.Utilities.Strings.IsValidHostName(hostName);
            Assert.False(result);
        }
    }
}
