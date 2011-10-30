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
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToCountryIdentificationCodeType_Null_ThrowsException()
        {
            Citizen.ToCountryIdentificationCodeType(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ToCountryIdentificationCodeType_InvalidCountryCode_ThrowsException(
             [Values(-66, -1, 0, 5999, 5103, 5102, 5001)]              
            short countryCode)
        {
            Citizen.ToCountryIdentificationCodeType(new Citizen() { CountryCode = countryCode });
        }

        [Test]
        public void ToCountryIdentificationCodeType_Valid_CorrectScheme(
            [Values(123, 838, 767)] short countryCode)
        {
            var result = Citizen.ToCountryIdentificationCodeType(new Citizen() { CountryCode = countryCode });
            Assert.AreEqual(_CountryIdentificationSchemeType.imk, result.scheme);
        }

        [Test]
        public void ToCountryIdentificationCodeType_Valid_CorrectCode(
            [Values(123, 838, 767)] short countryCode)
        {
            var result = Citizen.ToCountryIdentificationCodeType(new Citizen() { CountryCode = countryCode });
            Assert.AreEqual(countryCode.ToString(), result.Value);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToBirthdate_Null_ThrowsException()
        {
            Citizen.ToBirthdate(null);
        }

        [Test]
        [ExpectedException()]
        public void ToBirthdate_EmptyBirthdate_EmptyBirthdateAndPNR_ThrowsException()
        {
            Citizen.ToBirthdate(new Citizen() { });
        }

        public DateTime[] SampleDates = new DateTime[]{new DateTime(2009,4,18),new DateTime(2010,3,8)};
        public string[] MatchingPNRs = new string[] { "1804094111", "0803104111" };

        public decimal[] RandomCprNumbers = Utilities.RandomCprNumbers(5);


        [Test]
        public void ToBirthdate_HasBirthdateNoPNR_CorrectBirthdate(
            [ValueSource("SampleDates")] DateTime birthDate)
        {
            var result = Citizen.ToBirthdate(new Citizen() { Birthdate = birthDate, BirthdateUncertainty = ' ' });
            Assert.AreEqual(birthDate, result);
        }

        [Test]
        [Sequential]
        public void ToBirthdate_PNRwithNoBirthdate_ReturnsCorrectBirthdate(            
            [ValueSource("SampleDates")] DateTime expectedBirthDate,
            [ValueSource("MatchingPNRs")] string cprNumber)
        {
            var result = Citizen.ToBirthdate(new Citizen() { PNR=decimal.Parse(cprNumber),BirthdateUncertainty='e'});
            Assert.AreEqual(expectedBirthDate, result);
        }
    }
}
