using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace ConvertersTests
    {
        [TestFixture]
        public class DecimalToString
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

        [TestFixture]
        public class ToDateTime
        {
            [Test]
            public void ToDateTime_BlankFlag_NotNull()
            {
                var ret = Converters.ToDateTime(DateTime.Today, ' ');
                Assert.NotNull(ret);
            }

            [Test]
            public void ToDateTime_OtherFlag_Null(
                [Values('w', '-', ',', '1', 'u', 'M')]char uncertainty)
            {
                var ret = Converters.ToDateTime(DateTime.Today, uncertainty);
                Assert.Null(ret);
            }

            [Test]
            public void ToDateTime_EmptyDate_MiscUncertainty_Null(
                [Values(' ', 'w', '-', ',', '1', 'u', 'M')]char uncertainty)
            {
                var ret = Converters.ToDateTime(new DateTime(), uncertainty);
                Assert.Null(ret);
            }
        }

    }
}