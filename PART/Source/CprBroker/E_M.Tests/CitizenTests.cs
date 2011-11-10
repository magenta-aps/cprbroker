using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.E_M;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.E_M
{
    public class CitizenTests
    {
        public decimal[] RandomCprNumbers = Utilities.RandomCprNumbers(5);

        class CitizenStub : Citizen
        {
            public CitizenStub()
            {
                PNR = Utilities.RandomCprNumber();
                Gender = 'M';
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

            public AdresseType _ToAndreAdresse = new AdresseType();
            public override AdresseType ToAndreAdresse()
            {
                return _ToAndreAdresse;
            }

            public DateTime _ToBirthdate = new DateTime(2011, 11, 9);
            public override DateTime ToBirthdate()
            {
                return _ToBirthdate;
            }

            public string _ToBirthRegistrationAuthority;
            public override string ToBirthRegistrationAuthority()
            {
                return _ToBirthRegistrationAuthority;
            }

            public KontaktKanalType _ToKontaktKanalType = new KontaktKanalType();
            public override KontaktKanalType ToKontaktKanalType()
            {
                return _ToKontaktKanalType;
            }

            public bool _ToCivilRegistrationValidityStatusIndicator;
            public override bool ToCivilRegistrationValidityStatusIndicator()
            {
                return _ToCivilRegistrationValidityStatusIndicator;
            }

            public KontaktKanalType _ToNextOfKin = new KontaktKanalType();
            public override KontaktKanalType ToNextOfKin()
            {
                return _ToNextOfKin;
            }

            public NavnStrukturType _ToNavnStrukturType = new NavnStrukturType();
            public override NavnStrukturType ToNavnStrukturType()
            {
                return _ToNavnStrukturType;
            }
        }

        [TestFixture]
        public class ToEgenskabType
        {
            [Test]
            public void ToEgenskabType_Valid_NotNull()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToEgenskabType();
                Assert.NotNull(result);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectAndreAdresser()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(citizen._ToAndreAdresse, result.AndreAdresser);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectBirthdate()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(citizen._ToBirthdate, result.BirthDate);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectBirthPlaceText()
            {
                var citizen = new CitizenStub() { BirthPlaceText = "jkhgkhkahkj" };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(citizen.BirthPlaceText, result.FoedestedNavn);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectBirthRegistrationAuthority(
                [Values("GK", "LTK")]string authorityName)
            {
                var citizen = new CitizenStub() { _ToBirthRegistrationAuthority = authorityName };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(authorityName, result.FoedselsregistreringMyndighedNavn);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectKontaktKanal()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(citizen._ToKontaktKanalType, result.KontaktKanal);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectNaermestePaaroerende()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(citizen._ToNextOfKin, result.NaermestePaaroerende);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectNavnStruktur()
            {
                var citizen = new CitizenStub() { };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(citizen._ToNavnStrukturType, result.NavnStruktur);
            }

            [Test]
            public void ToEgenskabType_Valid_CorrectGender(
                [Values('K', 'M')]char gender
                )
            {
                var citizen = new CitizenStub() { Gender = gender };
                var result = citizen.ToEgenskabType();
                Assert.AreEqual(Converters.ToPersonGenderCodeType(gender), result.PersonGenderCode);
            }
        }

        [TestFixture]
        public class ToNavnStrukturType
        {

            [Test]
            public void ToNavnStrukturType_Null_NotNull(
                [Values(null, "")]string addressingName)
            {
                var citizen = new Citizen() { AddressingName = addressingName };
                var result = citizen.ToNavnStrukturType();
                Assert.NotNull(result);
            }

            string[] AddressingNames = new string[] { "John Mendel-Jensen Smith", "Smith, John Mendel-Jensen", "John Mendel-Jensen         Smith" };

            [Test]
            public void ToNavnStrukturType_Valid_AddressingNameNotNull(
                [ValueSource("AddressingNames")]string addressingName)
            {
                var citizen = new Citizen() { AddressingName = addressingName };
                var result = citizen.ToNavnStrukturType();
                Assert.IsNotNullOrEmpty(result.PersonNameForAddressingName);
            }

            [Test]
            public void ToNavnStrukturType_Valid_PersonNameNotNull(
                [ValueSource("AddressingNames")]string addressingName)
            {
                var citizen = new Citizen() { AddressingName = addressingName };
                var result = citizen.ToNavnStrukturType();
                Assert.NotNull(result.PersonNameStructure);
            }

            [Test]
            public void ToNavnStrukturType_Valid_CorrectLastName(
                [ValueSource("AddressingNames")]string addressingName)
            {
                var citizen = new Citizen() { AddressingName = addressingName };
                var result = citizen.ToNavnStrukturType();
                Assert.AreEqual("John Mendel-Jensen Smith", result.PersonNameStructure.ToString());
            }
        }

        [TestFixture]
        public class ToRegisterOplysningType
        {
            [Test]
            public void ToRegisterOplysningType_WhateverCountry_ReturnsNotNull(
                [Values(5100, 6763, 12)]short countryCode)
            {
                var citizen = new Citizen() { PNR = Utilities.RandomCprNumber(), CountryCode = countryCode };
                var result = citizen.ToRegisterOplysningType(DateTime.Today);
                Assert.NotNull(result.Item);
            }

            [Test]
            [Ignore]
            public void ToRegisterOplysningType_WhateverCountry_VirkningNotNull(
                [Values(5100, 6763, 12)]short countryCode)
            {
                var citizen = new Citizen() { PNR = Utilities.RandomCprNumber(), CountryCode = countryCode };
                var result = citizen.ToRegisterOplysningType(DateTime.Today);
                Assert.NotNull(result.Virkning);
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
        }

        [TestFixture]
        public class ToCprBorgerType
        {
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
            public void ToCprBorgerType_Valid_CorrectPersonNummerGyldighedStatusIndikator(
                [Values(true, false)]bool pnrValidity)
            {
                var citizen = new CitizenStub() { _ToCivilRegistrationValidityStatusIndicator = pnrValidity };
                var result = Citizen.ToCprBorgerType(citizen, DateTime.Now);
                Assert.AreEqual(citizen._ToCivilRegistrationValidityStatusIndicator, result.PersonNummerGyldighedStatusIndikator);
            }

            [Test]
            public void ToCprBorgerType_Valid_FalsePersonNummerGyldighedStatusIndikator()
            {
                var citizen = new CitizenStub() { };
                var result = Citizen.ToCprBorgerType(citizen, DateTime.Now);
                Assert.AreEqual(false, result.TelefonNummerBeskyttelseIndikator);
            }
        }

        [TestFixture]
        public class ToUdenlandskBorgerType
        {
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
        }

        [TestFixture]
        public class ToUkendtBorgerType
        {
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
        }
    }
}
