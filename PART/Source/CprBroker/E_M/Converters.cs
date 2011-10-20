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
            //TODO: Handle 'D' (dead) 
            //TODO: See what fits into CivilStatusKodeType.Separeret

            if (status.HasValue)
            {
                switch (status.Value)
                {
                    case 'D':
                        break;
                    case 'E':
                        return CivilStatusKodeType.Enke;
                    case 'F': // divorced
                        return CivilStatusKodeType.Skilt;
                    case 'G':
                        return CivilStatusKodeType.Gift;
                    case 'L':
                        return CivilStatusKodeType.Laengstlevende;
                    case 'O':
                        return CivilStatusKodeType.OphaevetPartnerskab;
                    case 'P':
                        return CivilStatusKodeType.RegistreretPartner;
                    case 'U':
                        return CivilStatusKodeType.Ugift;
                }
            }
            else
            {
                throw new Exception(string.Format("Invalid input <{0}>"));
            }
            return CivilStatusKodeType.Ugift;
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
            //TODO: Fill gender conversion
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