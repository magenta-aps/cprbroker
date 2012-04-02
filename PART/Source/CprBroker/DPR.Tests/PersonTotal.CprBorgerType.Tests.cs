using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.DPR;
using CprBroker.Utilities;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.DPR.PersonTotalTests
{
    [TestFixture]
    public class ToCprBorgerType : BaseTests
    {
        class PersonTotalStub : PersonTotal
        {
            public PersonTotalStub()
            {
                Status = 1;
            }
        }
        #region ToCprBorgerType
        [Test]
        public void ToCprBorgerType_Normal_NotNull()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUkendtBorgerType();
            Assert.NotNull(result);
        }

        [Test]
        public void ToCprBorgerType_Normal_CorrectPnr(
            [ValueSource("RandomCprNumbers5")]decimal cprNumber)
        {
            var personTotal = new PersonTotalStub() { PNR = cprNumber };
            var result = personTotal.ToCprBorgerType(new Nationality(), new PersonAddress());
            Assert.AreEqual(cprNumber, decimal.Parse(result.PersonCivilRegistrationIdentifier));
        }

        [Test]
        public void ToCprBorgerType_Normal_NullAdresseNoteTekst()
        {
            var personTotal = new PersonTotalStub();
            var result = personTotal.ToCprBorgerType(new Nationality(), new PersonAddress());
            Assert.IsNullOrEmpty(result.AdresseNoteTekst);
        }

        [Test]
        public void ToCprBorgerType_NoAddress_NullAddress()
        {
            var personTotal = new PersonTotalStub();
            var result = personTotal.ToCprBorgerType(new Nationality(), null);
            Assert.Null(result.FolkeregisterAdresse);
        }

        [Test]
        public void ToCprBorgerType_WithAddress_AddressNotNull()
        {
            var personTotal = new PersonTotalStub();
            var result = personTotal.ToCprBorgerType(new Nationality(), new PersonAddress());
            Assert.NotNull(result.FolkeregisterAdresse);
        }

        [Test]
        public void ToCprBorgerType_DirProtection_CorrectResult(
            [Values(null, '1')]char? dirProtection)
        {
            var personTotal = new PersonTotalStub() { DirectoryProtectionMarker = dirProtection };
            var result = personTotal.ToCprBorgerType(new Nationality(), null);
            Assert.AreEqual(personTotal.ToDirectoryProtectionIndicator(), result.ForskerBeskyttelseIndikator);
        }

        [Test]
        public void ToCprBorgerType_Nationality_CorrectResult(
            [ValueSource("RandomCountryCodes5")]decimal countryCode)
        {
            var personTotal = new PersonTotalStub();
            var result = personTotal.ToCprBorgerType(new Nationality() { CountryCode = countryCode }, null);
            Assert.AreEqual(countryCode, decimal.Parse(result.PersonNationalityCode.Value));
        }

        [Test]
        public void ToCprBorgerType_CivilRegState_CorrectPNRValidity(
            [ValueSource("AllCivilRegistrationStates")] decimal status)
        {
            var personTotal = new PersonTotal() { Status = status };
            var result = personTotal.ToCprBorgerType(new Nationality(), new PersonAddress());
            Assert.AreEqual(personTotal.ToCivilRegistrationValidityStatusIndicator(), result.PersonNummerGyldighedStatusIndikator);
        }

        [Test]
        public void ToCprBorgerType_Church_CorrectMembership(
            [Values(null, '1')] char? christianMark)
        {
            var personTotal = new PersonTotalStub() { ChristianMark = christianMark };
            var result = personTotal.ToCprBorgerType(new Nationality(), new PersonAddress());
            Assert.AreEqual(personTotal.ToChurchMembershipIndicator(), result.FolkekirkeMedlemIndikator);
        }

        [Test]
        public void ToCprBorgerType_Normal_TelephoneNumberProtectionIndicatorIsFalse()
        {
            // TODO: Add more cases for values of other data protection types
            var personTotal = new PersonTotalStub();
            var result = personTotal.ToCprBorgerType(new Nationality(), null);
            Assert.False(result.TelefonNummerBeskyttelseIndikator);
        }


        #endregion

    }
}
