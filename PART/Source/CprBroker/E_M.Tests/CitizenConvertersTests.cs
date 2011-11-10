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
    public class CitizenConvertersTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToCountryIdentificationCodeType_InvalidCountryCode_ThrowsException(
             [Values(-66, -1, 0, 5999, 5103, 5102, 5001)]              
            short countryCode)
        {
            var citizen = new Citizen() { CountryCode = countryCode };
            citizen.ToCountryIdentificationCodeType();
        }

        [Test]
        public void ToCountryIdentificationCodeType_Valid_CorrectScheme(
            [Values(123, 838, 767)] short countryCode)
        {
            var citizen = new Citizen() { CountryCode = countryCode };
            var result = citizen.ToCountryIdentificationCodeType();
            Assert.AreEqual(_CountryIdentificationSchemeType.imk, result.scheme);
        }

        [Test]
        public void ToCountryIdentificationCodeType_Valid_CorrectCode(
            [Values(123, 838, 767)] short countryCode)
        {
            var citizen = new Citizen() { CountryCode = countryCode };
            var result = citizen.ToCountryIdentificationCodeType();
            Assert.AreEqual(countryCode.ToString(), result.Value);
        }

        [Test]
        [ExpectedException()]
        public void ToBirthdate_EmptyBirthdate_EmptyBirthdateAndPNR_ThrowsException()
        {
            var citizen = new Citizen() { };
            citizen.ToBirthdate();
        }

        public DateTime[] SampleDates = new DateTime[] { new DateTime(2009, 4, 18), new DateTime(2010, 3, 8) };
        public string[] MatchingPNRs = new string[] { "1804094111", "0803104111" };

        public decimal[] RandomCprNumbers = Utilities.RandomCprNumbers(5);


        [Test]
        public void ToBirthdate_HasBirthdateNoPNR_CorrectBirthdate(
            [ValueSource("SampleDates")] DateTime birthDate)
        {
            var citizen = new Citizen() { Birthdate = birthDate, BirthdateUncertainty = ' ' };
            var result = citizen.ToBirthdate();
            Assert.AreEqual(birthDate, result);
        }

        [Test]
        [Sequential]
        public void ToBirthdate_PNRwithNoBirthdate_ReturnsCorrectBirthdate(
            [ValueSource("SampleDates")] DateTime expectedBirthDate,
            [ValueSource("MatchingPNRs")] string cprNumber)
        {
            var citizen = new Citizen() { PNR = decimal.Parse(cprNumber), BirthdateUncertainty = 'e' };
            var result = citizen.ToBirthdate();
            Assert.AreEqual(expectedBirthDate, result);
        }

        #region ToDirectoryProtectionStartDate

        [Test]
        public void ToDirectoryProtectionStartDate_DateWithNullFlag_ReturnsNull(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var result = Citizen.ToDirectoryProtectionStartDate(new Citizen() { DirectoryProtectionDate = dateValue, DirectoryProtectionDateUncertainty = 'U' });
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToDirectoryProtectionStartDate_Null_ReturnsNull()
        {
            var result = Citizen.ToDirectoryProtectionStartDate(new Citizen() { DirectoryProtectionDateUncertainty = 'U' });
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToDirectoryProtectionStartDate_Valid_ReturnsValid(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var result = Citizen.ToDirectoryProtectionStartDate(new Citizen() { DirectoryProtectionDate = dateValue, DirectoryProtectionDateUncertainty = ' ' });
            Assert.AreEqual(dateValue, result.Value);
        }

        #endregion

        #region ToDirectoryProtectionEndDate

        [Test]
        public void ToDirectoryProtectionEndDate_DateWithNullFlag_ReturnsNull(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var result = Citizen.ToDirectoryProtectionEndDate(new Citizen() { DirectoryProtectionEndDate = dateValue, DirectoryProtectionEndDateUncertainty = 'U' });
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToDirectoryProtectionEndDate_Null_ReturnsNull()
        {
            var result = Citizen.ToDirectoryProtectionEndDate(new Citizen() { DirectoryProtectionEndDateUncertainty = 'U' });
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToDirectoryProtectionEndDate_Valid_ReturnsValid(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var result = Citizen.ToDirectoryProtectionEndDate(new Citizen() { DirectoryProtectionEndDate = dateValue, DirectoryProtectionEndDateUncertainty = ' ' });
            Assert.AreEqual(dateValue, result.Value);
        }

        #endregion

        #region ToDirectoryProtectionIndicator

        [Test]
        public void ToDirectoryProtectionIndicator_NullStartAndEnd_ReturnsFalse()
        {
            var citizen = new Citizen() { DirectoryProtectionDateUncertainty = 'U', DirectoryProtectionEndDateUncertainty = 'U' };
            var result = citizen.ToDirectoryProtectionIndicator(DateTime.Now);
            Assert.False(result);
        }

        [Test]
        public void ToDirectoryProtectionIndicator_NullStart_ReturnsFalse(
            [ValueSource("SampleDates")]DateTime endDate)
        {
            var citizen = new Citizen() { DirectoryProtectionDateUncertainty = 'U', DirectoryProtectionEndDate = endDate, DirectoryProtectionEndDateUncertainty = ' ' };
            var result = citizen.ToDirectoryProtectionIndicator(DateTime.Now);
            Assert.False(result);
        }

        [Test]
        public void ToDirectoryProtectionIndicator_NullEnd_ReturnsTrue(
            [ValueSource("SampleDates")]DateTime startDate)
        {
            var citizen = new Citizen() { DirectoryProtectionDate = startDate, DirectoryProtectionDateUncertainty = ' ', DirectoryProtectionEndDateUncertainty = 'U' };
            var result = citizen.ToDirectoryProtectionIndicator(DateTime.Now);
            Assert.True(result);
        }

        [Test]
        public void ToDirectoryProtectionIndicator_FilledWithOutEffect_ReturnsFalse(
            [ValueSource("SampleDates")]DateTime startDate)
        {
            var citizen = new Citizen() { DirectoryProtectionDate = startDate, DirectoryProtectionDateUncertainty = ' ', DirectoryProtectionEndDate = startDate.AddDays(10), DirectoryProtectionEndDateUncertainty = ' ' };
            var result = citizen.ToDirectoryProtectionIndicator(startDate.AddDays(20));
            Assert.False(result);
        }

        [Test]
        public void ToDirectoryProtectionIndicator_FilledWithInEffect_ReturnsTrue(
            [ValueSource("SampleDates")]DateTime startDate)
        {
            var citizen = new Citizen() { DirectoryProtectionDate = startDate, DirectoryProtectionDateUncertainty = ' ', DirectoryProtectionEndDate = startDate.AddDays(10), DirectoryProtectionEndDateUncertainty = ' ' };
            var result = citizen.ToDirectoryProtectionIndicator(startDate.AddDays(5));
            Assert.True(result);
        }

        #endregion

        #region ToAddressProtectionStartDate

        [Test]
        public void ToAddressProtectionStartDate_DateWithNullFlag_ReturnsNull(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var result = Citizen.ToAddressProtectionStartDate(new Citizen() { AddressProtectionDate = dateValue, AddressProtectionDateUncertainty = 'U' });
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToAddressProtectionStartDate_Null_ReturnsNull()
        {
            var result = Citizen.ToAddressProtectionStartDate(new Citizen() { AddressProtectionDateUncertainty = 'U' });
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToAddressProtectionStartDate_Valid_ReturnsValid(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var result = Citizen.ToAddressProtectionStartDate(new Citizen() { AddressProtectionDate = dateValue, AddressProtectionDateUncertainty = ' ' });
            Assert.AreEqual(dateValue, result.Value);
        }

        #endregion

        #region ToAddressProtectionEndDate

        [Test]
        public void ToAddressProtectionEndDate_DateWithNullFlag_ReturnsNull(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var result = Citizen.ToAddressProtectionEndDate(new Citizen() { AddressProtectionEndDate = dateValue, AddressProtectionEndDateUncertainty = 'U' });
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToAddressProtectionEndDate_Null_ReturnsNull()
        {
            var result = Citizen.ToAddressProtectionEndDate(new Citizen() { AddressProtectionEndDateUncertainty = 'U' });
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToAddressProtectionEndDate_Valid_ReturnsValid(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var result = Citizen.ToAddressProtectionEndDate(new Citizen() { AddressProtectionEndDate = dateValue, AddressProtectionEndDateUncertainty = ' ' });
            Assert.AreEqual(dateValue, result.Value);
        }

        #endregion

        #region ToAddressProtectionIndicator

        [Test]
        public void ToAddressProtectionIndicator_NullStartAndEnd_ReturnsFalse()
        {
            var citizen = new Citizen() { AddressProtectionDateUncertainty = 'U', AddressProtectionEndDateUncertainty = 'U' };
            var result = citizen.ToAddressProtectionIndicator(DateTime.Now);
            Assert.False(result);
        }

        [Test]
        public void ToAddressProtectionIndicator_NullStart_ReturnsFalse(
            [ValueSource("SampleDates")]DateTime endDate)
        {
            var citizen = new Citizen() { AddressProtectionDateUncertainty = 'U', AddressProtectionEndDate = endDate, AddressProtectionEndDateUncertainty = ' ' };
            var result = citizen.ToAddressProtectionIndicator(DateTime.Now);
            Assert.False(result);
        }

        [Test]
        public void ToAddressProtectionIndicator_NullEnd_ReturnsTrue(
            [ValueSource("SampleDates")]DateTime startDate)
        {
            var citizen = new Citizen() { AddressProtectionDate = startDate, AddressProtectionDateUncertainty = ' ', AddressProtectionEndDateUncertainty = 'U' };
            var result = citizen.ToAddressProtectionIndicator(DateTime.Now);
            Assert.True(result);
        }

        [Test]
        public void ToAddressProtectionIndicator_FilledWithOutEffect_ReturnsFalse(
            [ValueSource("SampleDates")]DateTime startDate)
        {
            var citizen = new Citizen() { AddressProtectionDate = startDate, AddressProtectionDateUncertainty = ' ', AddressProtectionEndDate = startDate.AddDays(10), AddressProtectionEndDateUncertainty = ' ' };
            var result = citizen.ToAddressProtectionIndicator(startDate.AddDays(20));
            Assert.False(result);
        }

        [Test]
        public void ToAddressProtectionIndicator_FilledWithInEffect_ReturnsTrue(
            [ValueSource("SampleDates")]DateTime startDate)
        {
            var citizen = new Citizen() { AddressProtectionDate = startDate, AddressProtectionDateUncertainty = ' ', AddressProtectionEndDate = startDate.AddDays(10), AddressProtectionEndDateUncertainty = ' ' };
            var result = citizen.ToAddressProtectionIndicator(startDate.AddDays(5));
            Assert.True(result);
        }

        #endregion

        #region ToCivilRegistrationValidityStatusIndicator

        [Test]
        public void ToCivilRegistrationValidityStatusIndicator_Valid_ReturnsCorrect(
            [Values(1, 3, 90, 80)]short status)
        {
            var citizen = new Citizen() { CitizenStatusCode = status };
            var result = citizen.ToCivilRegistrationValidityStatusIndicator();
            Assert.AreEqual(Schemas.Util.Enums.IsActiveCivilRegistrationStatus(status), result);
        }

        #endregion
    }
}
