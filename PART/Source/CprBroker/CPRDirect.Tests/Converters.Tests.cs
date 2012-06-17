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
                [Values('w', '-', ',', '1', 'u', 'M')]char uncertainty)
            {
                var ret = Converters.ToDateTime(new DateTime(), uncertainty);
                Assert.Null(ret);
            }
            [Test]
            public void ToDateTime_TodayDate_MiscUncertainty_Null(
                [Values('w', '-', ',', '1', 'u', 'M')]char uncertainty)
            {
                var ret = Converters.ToDateTime(DateTime.Today, uncertainty);
                Assert.Null(ret);
            }
        }

        [TestFixture]
        public class ToPnrStringOrNull
        {
            [Test]
            public void ToPnrStringOrNull_InvalidPnr_Null(
                [Values(null, "kjkjkl", "987889789999", "123456789011", "123456", "0012345678", "  12345678")]string pnr)
            {
                var ret = Converters.ToPnrStringOrNull(pnr);
                Assert.Null(ret);
            }

            [Test]
            public void ToPnrStringOrNull_Valid_Correct(
                [Values("1234567890", "0123456789", "123456789")]string pnr)
            {
                var ret = Converters.ToPnrStringOrNull(pnr);
                Assert.AreEqual(decimal.Parse(pnr), decimal.Parse(ret));
            }
        }

    }
}