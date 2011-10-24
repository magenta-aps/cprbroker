using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using NUnit.Framework;

namespace CprBroker.Providers.E_M
{
    internal class Converters
    {
        internal static string ToCprNumber(decimal? cprNumber)
        {
            //TODO: Test cpr number conversion
            if (cprNumber.HasValue)
            {
                string ret = cprNumber.Value.ToString("F0");
                ret = new string('0', 10 - ret.Length) + ret;
                return ret;
            }
            else
            {
                return null;
            }
        }

        internal static CivilStatusKodeType ToCivilStatusKodeType(char? status)
        {
            if (status.HasValue)
            {
                return new CivilStatusCodes().Map(status.Value);
            }
            else
            {
                throw new Exception(string.Format("Invalid input <{0}>"));
            }
        }

        internal static LivStatusKodeType ToLivStatusKodeType(short? value, DateTime? birthDate)
        {
            decimal decimalStatus = 0;
            if (value.HasValue)
            {
                decimalStatus = (decimal)value.Value;
            }
            //TODO: Validate this call
            return Schemas.Util.Enums.ToLifeStatus(decimalStatus, birthDate);
        }

        internal static DateTime? ToDateTime(DateTime? value, char? uncertainty)
        {
            //TODO: Fill date time conversion
            if (value.HasValue && uncertainty.HasValue && uncertainty.Value == 'T')
            {
                return value.Value;
            }
            return null;
        }

        internal static string ShortToString(short? val)
        {
            //TODO: Revise this short to string conversion
            if (val.HasValue)
            {
                return val.Value.ToString("F0");
            }
            return null;
        }

        internal static PersonGenderCodeType ToPersonGenderCodeType(char? value)
        {
            //TODO: check the gender conversion char codes in database
            if (value.HasValue)
            {
                switch (value.ToString().ToUpper()[0])
                {
                    case 'M':
                        return PersonGenderCodeType.male;
                    case 'K':
                        return PersonGenderCodeType.female;
                }
            }
            return PersonGenderCodeType.unspecified;
        }

        internal static DateTime? GetMaxDate(params DateTime?[] dates)
        {
            var datesWithValues = dates.Where(d => d.HasValue).Select(d => d.Value);
            if (datesWithValues.Count() > 0)
            {
                return datesWithValues.Max();
            }
            else
            {
                return null;
            }
        }

        [TestFixture]
        private class Tests
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
                    var result = ToCivilStatusKodeType(status);
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
}