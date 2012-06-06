using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    [TestFixture]
    public class ConvertersTests
    {
        [Test]
        public void DecimalToString_Misc_CorrectResult(
            [Values(0, 10, 20)] decimal value,
            [Values(15, 7, 9)]int length
            )
        {
            var ss = Converters.DecimalToString(value, length);
            Assert.AreEqual(length, ss.Length);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DecimalToString_ShorterLength_CorrectResult(
            [Values(150, 1022, 2213)] decimal value,
            [Values(0, 1, 2)]int length
            )
        {
            var ss = Converters.DecimalToString(value, length);
            Assert.AreEqual(length, ss.Length);
        }
    }
}
