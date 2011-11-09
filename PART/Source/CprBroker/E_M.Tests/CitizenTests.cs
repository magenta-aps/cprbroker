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
    public class CitizenTests
    {
        public decimal[] RandomCprNumbers = Utilities.RandomCprNumbers(5);

        class CitizenStub : Citizen
        {
            public CitizenStub()
            {
                PNR = Utilities.RandomCprNumber();
            }

            public bool _ToChurchMembershipIndicator;
            public override bool ToChurchMembershipIndicator()
            {
                return _ToChurchMembershipIndicator;
            }

            public AdresseType _ToAdresseType = new AdresseType();
            public override AdresseType ToAdresseType()
            {
                return _ToAdresseType;
            }

            public bool _ToDirectoryProtectionIndicator;
            public override bool ToDirectoryProtectionIndicator(DateTime effectDate)
            {
                return _ToDirectoryProtectionIndicator;
            }

            public bool _ToAddressProtectionIndicator;
            public override bool ToAddressProtectionIndicator(DateTime effectDate)
            {
                return _ToAddressProtectionIndicator;
            }

            public CountryIdentificationCodeType _ToCountryIdentificationCodeType = new CountryIdentificationCodeType() { Value = "5648465" };
            public override CountryIdentificationCodeType ToCountryIdentificationCodeType()
            {
                return _ToCountryIdentificationCodeType;
            }
        }

        #region ToRegisterOplysningType

        [Test]
        public void ToRegisterOplysningType_WhateverCountry_ReturnsNotNull(
            [Values(5100, 6763, 12)]short countryCode)
        {
            var citizen = new Citizen() { PNR = Utilities.RandomCprNumber(), CountryCode = countryCode };
            var result = citizen.ToRegisterOplysningType(DateTime.Today);
            Assert.NotNull(result.Item);
        }

        [Test]
        public void ToRegisterOplysningType_UnknownCountry_ReturnsUkendtBorgerType(
            [ValueSource(typeof(Constants), "UnknownCountryCodes")]short countryCode)
        {
            var citizen = new Citizen() { PNR = Utilities.RandomCprNumber(), CountryCode = countryCode };
            var result = citizen.ToRegisterOplysningType(DateTime.Today);
            Assert.IsInstanceOf<UkendtBorgerType>(result.Item);
        }

        [Test]
        public void ToRegisterOplysningType_UnknownCountry_ReturnsCprBorgerType(
            [Values(5100)]short countryCode)
        {
            var citizen = new Citizen() { PNR = Utilities.RandomCprNumber(), CountryCode = countryCode };
            var result = citizen.ToRegisterOplysningType(DateTime.Today);
            Assert.IsInstanceOf<CprBorgerType>(result.Item);
        }

        [Test]
        public void ToRegisterOplysningType_UnknownCountry_ReturnsUdenlandskBorgerType(
            [Values(1, 7683, 628, 6583)]short countryCode)
        {
            var citizen = new Citizen() { PNR = Utilities.RandomCprNumber(), CountryCode = countryCode };
            var result = citizen.ToRegisterOplysningType(DateTime.Today);
            Assert.IsInstanceOf<UdenlandskBorgerType>(result.Item);
        }

        #endregion

        #region ToCprBorgerType

        [Test]
        public void ToCprBorgerType_Valid_NotNull()
        {
            var citizen = new CitizenStub();
            var result = Citizen.ToCprBorgerType(citizen, DateTime.Now);
            Assert.NotNull(result);
        }

        [Test]
        public void ToCprBorgerType_Valid_NullAddressNoteText()
        {
            var citizen = new CitizenStub();
            var result = Citizen.ToCprBorgerType(citizen, DateTime.Now);
            Assert.IsNullOrEmpty(result.AdresseNoteTekst);
        }

        [Test]
        public void ToCprBorgerType_Valid_CorrectFolkekirkeMedlemIndikator(
            [Values(true, false)]bool isMember)
        {
            var citizen = new CitizenStub() { _ToChurchMembershipIndicator = isMember };
            var result = Citizen.ToCprBorgerType(citizen, DateTime.Now);
            Assert.AreEqual(isMember, result.FolkekirkeMedlemIndikator);
        }

        [Test]
        public void ToCprBorgerType_Valid_CorrectAdresseType()
        {
            var citizen = new CitizenStub() { };
            var result = Citizen.ToCprBorgerType(citizen, DateTime.Now);
            Assert.AreEqual(citizen._ToAdresseType, result.FolkeregisterAdresse);
        }

        [Test]
        public void ToCprBorgerType_Valid_CorrectForskerBeskyttelseIndikator(
            [Values(true, false)] bool isProtected)
        {
            var citizen = new CitizenStub() { _ToDirectoryProtectionIndicator = isProtected };
            var result = Citizen.ToCprBorgerType(citizen, DateTime.Now);
            Assert.AreEqual(citizen._ToDirectoryProtectionIndicator, result.ForskerBeskyttelseIndikator);
        }

        [Test]
        public void ToCprBorgerType_Valid_CorrectNavneAdresseBeskyttelseIndikator(
            [Values(true, false)] bool isProtected)
        {
            var citizen = new CitizenStub() { _ToAddressProtectionIndicator = isProtected };
            var result = Citizen.ToCprBorgerType(citizen, DateTime.Now);
            Assert.AreEqual(citizen._ToAddressProtectionIndicator, result.NavneAdresseBeskyttelseIndikator);
        }

        [Test]
        public void ToCprBorgerType_Valid_CorrectPersonCivilRegistrationIdentifier(
            [ValueSource("RandomCprNumbers")] decimal cprNumber)
        {
            var citizen = new CitizenStub() { PNR = cprNumber };
            var result = Citizen.ToCprBorgerType(citizen, DateTime.Now);
            Assert.AreEqual(Converters.ToCprNumber(citizen.PNR), result.PersonCivilRegistrationIdentifier);
        }

        [Test]
        public void ToCprBorgerType_Valid_CorrectPersonNationalityCode()
        {
            var citizen = new CitizenStub() { };
            var result = Citizen.ToCprBorgerType(citizen, DateTime.Now);
            Assert.AreEqual(citizen._ToCountryIdentificationCodeType, result.PersonNationalityCode);
        }

        [Test]
        public void ToCprBorgerType_Valid_CorrectPersonNummerGyldighedStatusIndikator()
        {
            var citizen = new CitizenStub() { };
            var result = Citizen.ToCprBorgerType(citizen, DateTime.Now);
            Assert.AreEqual(true, result.PersonNummerGyldighedStatusIndikator);
        }

        [Test]
        public void ToCprBorgerType_Valid_CorrectTelefonNummerBeskyttelseIndikator()
        {
            var citizen = new CitizenStub() { };
            var result = Citizen.ToCprBorgerType(citizen, DateTime.Now);
            Assert.AreEqual(false, result.TelefonNummerBeskyttelseIndikator);
        }

        #endregion

        #region ToUdenlandskBorgerType

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToUdenlandskBorgerType_Null_ThrowsException()
        {
            Citizen.ToUdenlandskBorgerType(null);
        }

        [Test]
        [ExpectedException()]
        public void ToUdenlandskBorgerType_Empty_ThrowsException()
        {
            Citizen.ToUdenlandskBorgerType(new Citizen());
        }

        [Test]
        [Combinatorial]
        public void ToUdenlandskBorgerType_Valid_CorrectCountryCode(
            [ValueSource("RandomCprNumbers")] decimal cprNumber,
            [Values(1500, 762, 876)] short countryCode)
        {
            var result = Citizen.ToUdenlandskBorgerType(new Citizen() { PNR = cprNumber, CountryCode = countryCode });
            Assert.AreEqual(countryCode.ToString(), result.PersonNationalityCode[0].Value);
        }

        [Test]
        [Combinatorial]
        public void ToUdenlandskBorgerType_Valid_CorrectPNR(
            [ValueSource("RandomCprNumbers")] decimal cprNumber,
            [Values(1500, 762, 876)] short countryCode)
        {
            var result = Citizen.ToUdenlandskBorgerType(new Citizen() { PNR = cprNumber, CountryCode = countryCode });
            Assert.AreEqual(Converters.ToCprNumber(cprNumber), result.PersonCivilRegistrationReplacementIdentifier);
        }

        [Test]
        [Combinatorial]
        public void ToUdenlandskBorgerType_Valid_NullFoedselslandKode(
            [ValueSource("RandomCprNumbers")] decimal cprNumber,
            [Values(1500, 762, 876)] short countryCode)
        {
            var result = Citizen.ToUdenlandskBorgerType(new Citizen() { PNR = cprNumber, CountryCode = countryCode });
            Assert.Null(result.FoedselslandKode);
        }

        [Test]
        [Combinatorial]
        public void ToUdenlandskBorgerType_Valid_NullPersonIdentifikator(
            [ValueSource("RandomCprNumbers")] decimal cprNumber,
            [Values(1500, 762, 876)] short countryCode)
        {
            var result = Citizen.ToUdenlandskBorgerType(new Citizen() { PNR = cprNumber, CountryCode = countryCode });
            Assert.IsNullOrEmpty(result.PersonIdentifikator);
        }

        [Test]
        [Combinatorial]
        public void ToUdenlandskBorgerType_Valid_NullSprogCode(
            [ValueSource("RandomCprNumbers")] decimal cprNumber,
            [Values(1500, 762, 876)] short countryCode)
        {
            var result = Citizen.ToUdenlandskBorgerType(new Citizen() { PNR = cprNumber, CountryCode = countryCode });
            Assert.Null(result.SprogKode);
        }

        #endregion

        #region ToUkendtBorgerType

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ToUkendtBorgerType_Null_ThrowsException()
        {
            Citizen.ToUkendtBorgerType(null);
        }

        [Test]
        [ExpectedException]
        public void ToUkendtBorgerType_InvalidPNR_ThrowsException(
            [Values(0, 43, -99, 23.4)] decimal cprNumber)
        {
            Citizen.ToUkendtBorgerType(new Citizen() { PNR = cprNumber });
        }

        [Test]
        public void ToUkendtBorgerType_Valid_CorrectPNR(
            [ValueSource("RandomCprNumbers")] decimal cprNumber)
        {
            var result = Citizen.ToUkendtBorgerType(new Citizen() { PNR = cprNumber });
            Assert.AreEqual(Converters.ToCprNumber(cprNumber), result.PersonCivilRegistrationReplacementIdentifier);
        }

        #endregion
    }
}
