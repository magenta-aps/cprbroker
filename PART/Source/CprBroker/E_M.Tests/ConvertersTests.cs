using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.E_M;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.E_M
{
    [TestFixture]
    public class ConvertersTests
    {
        [Test]
        [Ignore]
        public void TestToCivilStatusKodeType(
            [Values(null, 'd', 'E', 's', ' ', 'W')]char? status
            )
        {
            var validValues = new char?[] { 'D', 'E', 'F', 'G', 'L', 'O', 'P', 'U', 'd', 'e', 'f', 'g', 'l', 'o', 'p', 'u' };
            try
            {
                var result = Converters.ToCivilStatusKodeType(status);
                Assert.IsTrue(validValues.Contains(status));
            }
            catch
            {
                Assert.IsFalse(validValues.Contains(status));
            }

        }

        DateTime?[] TestToLivStatusKodeTypeDates = new DateTime?[] { null, new DateTime(2011, 10, 10) };

        [Test]
        [Combinatorial]
        [Ignore]
        public void TestToLivStatusKodeType(
            [Values(null, (short)1, (short)2, (short)70, (short)85)] short? value,
            [ValueSource("TestToLivStatusKodeTypeDates")] DateTime? birthDate)
        {
            LivStatusKodeType result = LivStatusKodeType.Foedt;
            bool exception = false;
            try
            {
                result = Converters.ToLivStatusKodeType(value, birthDate);
            }
            catch (Exception ex)
            {
                exception = true;
            }
            if (value == null && birthDate != null)
            {
                Assert.IsTrue(exception, "Exception should have been thrown");
            }
            else
            {
                if (exception)
                {
                    Assert.Fail("Exception thrown");
                }
                if (birthDate == null)
                {
                    Assert.AreEqual(LivStatusKodeType.Prenatal, result);
                }
                else
                {
                    Assert.AreNotEqual(LivStatusKodeType.Prenatal, result);
                }
            }
        }

        DateTime?[] SampleDates = new DateTime?[] { null, new DateTime(2011, 10, 10), new DateTime(2000, 8, 22), DateTime.MinValue, DateTime.MaxValue };
        [Test]
        [Combinatorial]
        public void TestToDateTime(
            [ValueSource("SampleDates")] DateTime? value,
            [Values(null, 'T', 'd', 'W', ' ')] char? uncertainty)
        {
            var result = Converters.ToDateTime(value, uncertainty);
            if (!value.HasValue || !uncertainty.HasValue || (uncertainty.Value != 'T' && uncertainty.Value != 't'))
            {
                Assert.IsNull(result);
            }
            else
            {
                Assert.AreEqual(value.Value, result.Value);
            }
        }

        [Test]
        [TestCase(null)]
        [TestCase((short)10)]
        [TestCase((short)100)]
        public void TestShortToString(short? val)
        {
            var result = Converters.ShortToString(val);
            if (val == null)
            {
                Assert.IsNull(result);
            }
            else
            {
                Assert.IsNotNullOrEmpty(result);
                Assert.AreEqual(val.ToString(), result);
            }
        }

        [Test]
        public void TestToPersonGenderCodeType(
            [Values('M', 'm', 'K', null, ' ', 'w')] char? value)
        {
            var result = Converters.ToPersonGenderCodeType(value);
            if (value.HasValue && new string[] { "M", "K" }.Contains(value.Value.ToString().ToUpper()))
            {
                Assert.AreNotEqual(PersonGenderCodeType.unspecified, result);
            }
            else
            {
                Assert.AreEqual(PersonGenderCodeType.unspecified, result);
            }
        }

        DateTime?[][] TestGetMaxDateCases = new DateTime?[][]
            {
                new DateTime?[]{null},
                new DateTime?[]{null, null},
                new DateTime?[] { null, new DateTime(2011, 10, 10) },
                new DateTime?[]{null, new DateTime(2011, 10, 10), new DateTime(2011, 10, 10)},
                new DateTime?[]{null, new DateTime(2011, 10, 10), new DateTime(2011, 10, 10), null}
            };
        [Test]
        [TestCaseSource("TestGetMaxDateCases")]
        public void TestGetMaxDate(params DateTime?[] dates)
        {
            var result = Converters.GetMaxDate(dates);
            var dd = dates.Where(d => d.HasValue).Select(d => d.Value);
            if (dd.Count() > 0)
            {
                Assert.IsNotNull(result);
                Assert.AreEqual(dd.Max(), result);
            }
            else
            {
                Assert.IsNull(result);
            }
        }
    }
}
