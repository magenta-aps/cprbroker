/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
        public void ToCountryIdentificationCodeType_InvalidCountryCode_OK(
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
            var citizen = new Citizen() { DirectoryProtectionDate = dateValue, DirectoryProtectionDateUncertainty = 'U' };
            var result = citizen.ToDirectoryProtectionStartDate();
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToDirectoryProtectionStartDate_Null_ReturnsNull()
        {
            var citizen = new Citizen() { DirectoryProtectionDateUncertainty = 'U' };
            var result = citizen.ToDirectoryProtectionStartDate();
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToDirectoryProtectionStartDate_Valid_ReturnsValid(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var citizen = new Citizen() { DirectoryProtectionDate = dateValue, DirectoryProtectionDateUncertainty = ' ' };
            var result = citizen.ToDirectoryProtectionStartDate();
            Assert.AreEqual(dateValue, result.Value);
        }

        #endregion

        #region ToDirectoryProtectionEndDate

        [Test]
        public void ToDirectoryProtectionEndDate_DateWithNullFlag_ReturnsNull(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var citizen = new Citizen() { DirectoryProtectionEndDate = dateValue, DirectoryProtectionEndDateUncertainty = 'U' };
            var result = citizen.ToDirectoryProtectionEndDate();
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToDirectoryProtectionEndDate_Null_ReturnsNull()
        {
            var citizen = new Citizen() { DirectoryProtectionEndDateUncertainty = 'U' };
            var result = citizen.ToDirectoryProtectionEndDate();
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToDirectoryProtectionEndDate_Valid_ReturnsValid(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var citizen = new Citizen() { DirectoryProtectionEndDate = dateValue, DirectoryProtectionEndDateUncertainty = ' ' };
            var result = citizen.ToDirectoryProtectionEndDate();
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
            var citizen = new Citizen() { AddressProtectionDate = dateValue, AddressProtectionDateUncertainty = 'U' };
            var result = citizen.ToAddressProtectionStartDate();
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToAddressProtectionStartDate_Null_ReturnsNull()
        {
            var citizen = new Citizen() { AddressProtectionDateUncertainty = 'U' };
            var result = citizen.ToAddressProtectionStartDate();
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToAddressProtectionStartDate_Valid_ReturnsValid(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var citizen = new Citizen() { AddressProtectionDate = dateValue, AddressProtectionDateUncertainty = ' ' };
            var result = citizen.ToAddressProtectionStartDate();
            Assert.AreEqual(dateValue, result.Value);
        }

        #endregion

        #region ToAddressProtectionEndDate

        [Test]
        public void ToAddressProtectionEndDate_DateWithNullFlag_ReturnsNull(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var citizen = new Citizen() { AddressProtectionEndDate = dateValue, AddressProtectionEndDateUncertainty = 'U' };
            var result = citizen.ToAddressProtectionEndDate();
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToAddressProtectionEndDate_Null_ReturnsNull()
        {
            var citizen = new Citizen() { AddressProtectionEndDateUncertainty = 'U' };
            var result = citizen.ToAddressProtectionEndDate();
            Assert.False(result.HasValue);
        }

        [Test]
        public void ToAddressProtectionEndDate_Valid_ReturnsValid(
            [ValueSource("SampleDates")] DateTime dateValue)
        {
            var citizen = new Citizen() { AddressProtectionEndDate = dateValue, AddressProtectionEndDateUncertainty = ' ' };
            var result = citizen.ToAddressProtectionEndDate();
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

        #region ToMaritalStatusDate

        [Test]
        public void ToMaritalStatusDate_NullDate_ReturnsNull()
        {
            var citizen = new Citizen() { MaritalStatusTimestampUncertainty = 'U' };
            var result = citizen.ToMaritalStatusDate();
            Assert.Null(result);
        }

        [Test]
        public void ToMaritalStatusDate_Value_ReturnsCorrect()
        {
            var date = DateTime.Today;
            var citizen = new Citizen() { MaritalStatusTimestamp = date, MaritalStatusTimestampUncertainty = ' ' };
            var result = citizen.ToMaritalStatusDate();
            Assert.AreEqual(date, result);
        }

        #endregion

        #region ToMaritalStatusTerminationDate

        [Test]
        public void ToMaritalStatusTerminationDate_Null_ReturnsNull()
        {
            var citizen = new Citizen();
            var result = citizen.ToMaritalStatusTerminationDate();
            Assert.Null(result);
        }

        [Test]
        [Combinatorial]
        public void ToMaritalStatusTerminationDate_Value_ReturnsNull(
            [Values(1, 200, -1000)] int todayOffset,
            [Values(' ', 'U')] char dateMarker)
        {
            var date = DateTime.Today.AddDays(todayOffset);
            var citizen = new Citizen() { MaritalStatusTimestamp = date, MaritalStatusTimestampUncertainty = dateMarker };
            var result = citizen.ToMaritalStatusTerminationDate();
            Assert.Null(result);
        }

        #endregion
    }
}
