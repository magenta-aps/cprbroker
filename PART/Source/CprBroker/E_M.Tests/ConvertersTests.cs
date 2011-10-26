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
        decimal[] InvalidCprNumbers = new decimal[] { 0, 13, -123456789, -12345678, 1234.56789m };
        decimal[] RandomCprNumbers = Utilities.RandomCprNumbers(5);

        #region IsValidCprNumber
        [Test]
        [TestCaseSource("InvalidCprNumbers")]
        public void IsValidCprNumber_Invalid_ReturnsFalse(decimal cprNumber)
        {
            var result = Converters.IsValidCprNumber(cprNumber);
            Assert.False(result);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void IsValidCprNumber_Valid_ReturnsTrue(decimal cprNumber)
        {
            var result = Converters.IsValidCprNumber(cprNumber);
            Assert.True(result);
        }
        #endregion

        #region ToCprNumber
        [Test]
        [TestCaseSource("InvalidCprNumbers")]
        [ExpectedException(typeof(ArgumentException))]
        public void ToCprNumber_Invalid_ThrowsException(decimal cprNumber)
        {
            Converters.ToCprNumber(cprNumber);
        }

        [Test]
        [TestCaseSource("RandomCprNumbers")]
        public void ToCprNumber_Valid_CorrectOutput(decimal cprNumber)
        {
            var result = Converters.ToCprNumber(cprNumber);
            string cprNumberString = cprNumber.ToString();
            if (cprNumberString.Length < 10)
                cprNumberString = "0" + cprNumberString;
            Assert.AreEqual(cprNumberString, result);
        }
        #endregion

        #region ToCivilStatusKodeType
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToCivilStatusKodeType_Invalid_ThrowsException(
            [Values('e', 'W', 's', ' ')] char status)
        {
            Converters.ToCivilStatusKodeType(status);
        }

        [Test]
        public void ToCivilStatusKodeType_Valid_Passes(
            [Values('E', 'F', 'G', 'L', 'O', 'P', 'U')] char status)
        {
            Converters.ToCivilStatusKodeType(status);
        }

        [Test]
        [Ignore]
        public void ToCivilStatusKodeType_Dead_Passes(
            [Values('D')] char status)
        {
            Converters.ToCivilStatusKodeType(status);
        }
        #endregion

        #region ToLivStatusKodeType
        [Test]
        [Ignore("Need to throw exception from Schemas.Util.Enums.ToLifeStatus()")]
        [ExpectedException(typeof(ArgumentException))]
        public void ToLivStatusKodeType_InvalidStatus_ThrowsExcption(
            [Values(33, 55, 12, 0, -23)] short status,
            [Values(true, false)] bool hasBirthdate)
        {
            Converters.ToLivStatusKodeType(status, hasBirthdate);
        }

        [Test]
        public void ToLivStatusKodeType_AliveStatusWithBirthdate_ReturnsFoedt(
            [Values(1, 10, 20, 3, 30, 5, 50, 60, 7, 80)] short status)
        {
            var result = Converters.ToLivStatusKodeType(status, true);
            Assert.AreEqual(LivStatusKodeType.Foedt, result);
        }

        [Test]
        public void ToLivStatusKodeType_AliveStatusWithoutBirthdate_ReturnsPrenatal(
            [Values(1, 10, 20, 3, 30, 5, 50, 60, 7, 80)] short status)
        {
            var result = Converters.ToLivStatusKodeType(status, false);
            Assert.AreEqual(LivStatusKodeType.Prenatal, result);
        }

        [Test]
        public void ToLivStatusKodeType_DeadStatus_ReturnsDoed(
            [Values(true, false)] bool hasBirthdate)
        {
            var result = Converters.ToLivStatusKodeType(90, hasBirthdate);
            Assert.AreEqual(LivStatusKodeType.Doed, result);
        }

        [Test]
        public void ToLivStatusKodeType_DisappearedStatus_ReturnsDiasppeared(
            [Values(true, false)] bool hasBirthdate)
        {
            var result = Converters.ToLivStatusKodeType(70, hasBirthdate);
            Assert.AreEqual(LivStatusKodeType.Forsvundet, result);
        }
        #endregion

        #region ToDateTime
        DateTime[] SampleDates = new DateTime[] { new DateTime(2011, 10, 10), new DateTime(2000, 8, 22), DateTime.MinValue, DateTime.MaxValue };

        [Test]
        [Combinatorial]
        public void ToDateTime_InvalidCertainty_ReturnsNull(
            [ValueSource("SampleDates")] DateTime dateValue,
            [Values('d', 'e', 'q', 'G', 'O', '2')] char certaintyFlag)
        {
            var result = Converters.ToDateTime(dateValue, certaintyFlag);
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToDateTime_InvalidDate_ReturnsDateValue(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var result = Converters.ToDateTime(dateValue, ' ');
            Assert.AreEqual(dateValue, result.Value);
        }
        #endregion


        [Test]
        [TestCase(0)]
        [TestCase((short)10)]
        [TestCase((short)100)]
        public void TestShortToString(short val)
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
            [Values('M', 'm', 'K', ' ', 'w')] char value)
        {
            var result = Converters.ToPersonGenderCodeType(value);
            if (new string[] { "M", "K" }.Contains(value.ToString().ToUpper()))
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
