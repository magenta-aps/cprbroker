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
        public void ToCivilStatusKodeType_Dead_Passes(
            [Values('D')] char status)
        {
            Converters.ToCivilStatusKodeType(status);
        }
        #endregion

        #region ToLivStatusKodeType
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToLivStatusKodeType_InvalidStatus_ThrowsExcption(
            [Values(33, 55, 12, 0, -23)] short status,
            [Values(true, false)] bool hasBirthdate)
        {
            Converters.ToLivStatusKodeType(status, hasBirthdate);
        }

        [Test]
        public void ToLivStatusKodeType_AliveStatusWithBirthdate_ReturnsFoedt(
            [Values(1, 3, 5, 7, 20, 30, 5, 50, 60, 80)] short status)
        {
            var result = Converters.ToLivStatusKodeType(status, true);
            Assert.AreEqual(LivStatusKodeType.Foedt, result);
        }

        [Test]
        public void ToLivStatusKodeType_AliveStatusWithoutBirthdate_ReturnsPrenatal(
            [Values(1, 3, 5, 7, 20, 30, 5, 50, 60, 80)] short status)
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

        #region ShortToString
        [Test]
        [Sequential]
        public void ShortToString_Valid(
            [Values(0, 10, 7652, -12)] short shortValue,
            [Values("0", "10", "7652", "-12")] string stringValue)
        {
            var result = Converters.ShortToString(shortValue);
            Assert.AreEqual(stringValue, result);
        }
        #endregion

        #region DecimalToString
        [Test]
        [Sequential]
        public void DecimalToString_Valid(
            [Values(0.0, 10.25, 7652.5, -12)] double decimalValue,
            [Values("0", "10", "7653", "-12")] string stringValue)
        {
            var result = Converters.DecimalToString((decimal)decimalValue);
            Assert.AreEqual(stringValue, result);
        }
        #endregion

        #region ToPersonGenderCodeType
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToPersonGenderCodeType_InvalidGender_ThrowsException(
            [Values('e', ' ', 'L', '2', 'F', 'f')] char gender)
        {
            Converters.ToPersonGenderCodeType(gender);
        }

        [Test]
        public void ToPersonGenderCodeType_Male_ReturnsMale(
            [Values('M', 'm')] char gender)
        {
            var result = Converters.ToPersonGenderCodeType(gender);
            Assert.AreEqual(PersonGenderCodeType.male, result);
        }

        [Test]
        public void ToPersonGenderCodeType_Female_ReturnsFemale(
            [Values('K', 'k')] char gender)
        {
            var result = Converters.ToPersonGenderCodeType(gender);
            Assert.AreEqual(PersonGenderCodeType.female, result);
        }
        #endregion

        #region ToChurchMembershipIndicator

        [Test]
        public void ToChurchMembershipIndicator_Member_F_ReturnsTrue()
        {
            var result = Converters.ToChurchMembershipIndicator('F');
            Assert.True(result);
        }

        [Test]
        public void ToChurchMembershipIndicator_OtherValues_ReturnsFalse(
            [Values('U', 'A', 'M', 'S')]char value)
        {
            var result = Converters.ToChurchMembershipIndicator(value);
            Assert.False(result);
        }

        [Test]
        public void ToChurchMembershipIndicator_InvalidValues_ReturnsFalse(
            [Values('W', 'I', '8', ' ')]char value)
        {
            var result = Converters.ToChurchMembershipIndicator(value);
            Assert.False(result);
        }

        #endregion

        #region GetMaxDate

        DateTime?[][] NullDateArrays = new DateTime?[][]{
            new DateTime?[]{null},
            new DateTime?[]{null, null}
        };

        DateTime?[][] FilledDateArrays = new DateTime?[][]{
            new DateTime?[] { null, new DateTime(2011, 10, 10) },
            new DateTime?[]{null, new DateTime(2011, 10, 10), new DateTime(2011, 10, 10)},
            new DateTime?[]{null, new DateTime(2011, 10, 10), new DateTime(2011, 10, 10), null}
        };

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetMaxDate_NullArray_ThrowsException()
        {
            Converters.GetMaxDate(null as DateTime?[]);
        }

        [Test]
        public void GetMaxDate_NullElements_ReturnsNull(
            [ValueSource("NullDateArrays")] DateTime?[] dates)
        {
            var result = Converters.GetMaxDate(dates);
            Assert.Null(result);
        }

        [Test]
        public void GetMaxDate_ValuedElements_ReturnsMaximumValue(
            [ValueSource("FilledDateArrays")] DateTime?[] dates)
        {
            var result = Converters.GetMaxDate(dates);
            var dd = dates.Where(d => d.HasValue).Select(d => d.Value);
            Assert.AreEqual(dd.Max(), result);
        }

        #endregion

        #region ToNeutralString

        [Test]
        public void ToNeutralString_NullOrEmpty_ReturnsEmpty(
            [Values(null, "", "                  ")]string value)
        {
            var ret = Converters.ToNeutralString(value);
            Assert.AreEqual("", ret);
        }

        [Test]
        [Sequential]
        public void ToNeutralString_Various_ReturnsExpected(
            [Values("aaa", "  aaa", "aaa   ", "      aaa   ", "bbb bbb", "   bbb bbb    ")]string value,
            [Values("aaa", "aaa", "aaa", "aaa", "bbb bbb", "bbb bbb")]string expectedValue)
        { }

        #endregion

        #region ToNeutralHouseNumber

        [Test]
        public void ToNeutralHouseNumber_Empty_ReturnsZero(
            [Values(null, "", " ", "     ")]string houseNumber)
        {
            var result = Converters.ToNeutralHouseNumber(houseNumber);
            Assert.AreEqual(0, result);
        }

        [Test]
        [Sequential]
        public void ToNeutralHouseNumber_Normal_ReturnsCorrect(
            [Values("1", "22", "   ")]string houseNumber,
            [Values(1, 22, 0)]int houseNumberIntExpected)
        {
            var result = Converters.ToNeutralHouseNumber(houseNumber);
            Assert.AreEqual(houseNumberIntExpected, result);
        }

        [Test]
        [Sequential]
        public void ToNeutralHouseNumber_ZeroStart_ReturnsCorrect(
            [Values("022", "0001", "0022  ")]string houseNumber,
            [Values(22, 1, 22)]int houseNumberIntExpected)
        {
            var result = Converters.ToNeutralHouseNumber(houseNumber);
            Assert.AreEqual(houseNumberIntExpected, result);
        }

        [Test]
        [Sequential]
        public void ToNeutralHouseNumber_EndChar_ReturnsCorrect(
            [Values("1A", "22W", "022C ", "0001S   ")]string houseNumber,
            [Values(1, 22, 22, 1)]int houseNumberIntExpected)
        {
            var result = Converters.ToNeutralHouseNumber(houseNumber);
            Assert.AreEqual(houseNumberIntExpected, result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToNeutralHouseNumber_Invalid_ThrowsException(
            [Values("A", "!", "S22", "23AA")]string houseNumber)
        {
            Converters.ToNeutralHouseNumber(houseNumber);
        }
        #endregion

    }
}
