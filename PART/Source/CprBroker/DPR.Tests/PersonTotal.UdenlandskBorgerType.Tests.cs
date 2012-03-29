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
    public class ToUdenlandskBorgerType : BaseTests
    {

        #region ToUdenlandskBorgerType
        [Test]
        public void ToUdenlandskBorgerType_Normal_NotNull(
            [ValueSource("RandomCprNumbers5")]decimal cprNumber)
        {
            var personTotal = new PersonTotal() { PNR = cprNumber };
            var result = personTotal.ToUdenlandskBorgerType(new Nationality());
            Assert.NotNull(result);
        }

        [Test]
        public void ToUdenlandskBorgerType_Normal_CorrectPnr(
            [ValueSource("RandomCprNumbers5")]decimal cprNumber)
        {
            var personTotal = new PersonTotal() { PNR = cprNumber };
            var result = personTotal.ToUdenlandskBorgerType(new Nationality());
            Assert.AreEqual(cprNumber, decimal.Parse(result.PersonCivilRegistrationReplacementIdentifier));
        }

        [Test]
        public void ToUdenlandskBorgerType_Normal_CorrectNationality(
            [ValueSource("RandomCountryCodes5")]decimal countryCode)
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerType(new Nationality() { CountryCode = countryCode });
            Assert.AreEqual(countryCode, decimal.Parse(result.PersonNationalityCode[0].Value));
        }

        [Test]
        public void ToUdenlandskBorgerType_Normal_NullBirthCountry()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerType(new Nationality());
            Assert.Null(result.FoedselslandKode);
        }

        [Test]
        public void ToUdenlandskBorgerType_Normal_NullPersonIdentificator()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerType(new Nationality());
            Assert.IsNullOrEmpty(result.PersonIdentifikator);
        }

        [Test]
        public void ToUdenlandskBorgerType_Normal_EmptyLanguages()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerType(new Nationality());
            Assert.IsEmpty(result.SprogKode);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void ToUdenlandskBorgerType_NullNationality_Exception(
            [ValueSource("RandomCprNumbers5")]decimal cprNumber)
        {
            var personTotal = new PersonTotal() { PNR = cprNumber };
            personTotal.ToUdenlandskBorgerType(null);
        }


        #endregion

        #region ToUdenlandskBorgerTypeVirvning
        [Test]
        public void ToUkendtBorgerTypVirvninge_Empty_NotNull()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.NotNull(result);
        }

        [Test]
        public void ToUkendtBorgerTypVirvninge_Empty_EmptyResultFra()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.Null(result.FraTidspunkt.ToDateTime());
        }

        [Test]
        public void ToUkendtBorgerTypVirvninge_Empty_EmptyResultTil()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.Null(result.TilTidspunkt.ToDateTime());
        }

        [Test]
        public void ToUdenlandskBorgerTypeVirvning_Normal_CorrectDate(
            [ValueSource("RandomDecimalDates5")]decimal statusDate)
        {
            var personTotal = new PersonTotal() { StatusDate = statusDate };
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.AreEqual(Providers.DPR.Utilities.DateFromDecimal(statusDate), result.FraTidspunkt.Item);
        }

        [Test]
        public void ToUdenlandskBorgerTypeVirvning_Normal_EmptyResultTil(
            [ValueSource("RandomDecimalDates5")]decimal statusDate)
        {
            var personTotal = new PersonTotal() { StatusDate = statusDate };
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.Null(result.TilTidspunkt.ToDateTime());
        }

        [Test]
        public void ToUdenlandskBorgerTypeVirvning_MiscDates_CorrectDate(
            [ValueSource("RandomDecimalDates5")]decimal statusDate,
            [ValueSource("RandomDecimalDates5")]decimal otherDate)
        {
            var personTotal = new PersonTotal() { StatusDate = statusDate, AddressDate = otherDate, DateOfBirth = otherDate, MaritalStatusDate = otherDate, MunicipalityArrivalDate = otherDate, MunicipalityLeavingDate = otherDate, PaternityDate = otherDate, PersonalSelectionDate = otherDate, UnderGuardianshipDate = otherDate, VotingDate = otherDate };
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.AreEqual(Providers.DPR.Utilities.DateFromDecimal(statusDate), result.FraTidspunkt.Item);
        }

        [Test]
        public void ToUdenlandskBorgerTypeVirvning_MiscDates_EmptyResultTil(
            [ValueSource("RandomDecimalDates5")]decimal statusDate,
            [ValueSource("RandomDecimalDates5")]decimal otherDate)
        {
            var personTotal = new PersonTotal() { StatusDate = statusDate, AddressDate = otherDate, DateOfBirth = otherDate, MaritalStatusDate = otherDate, MunicipalityArrivalDate = otherDate, MunicipalityLeavingDate = otherDate, PaternityDate = otherDate, PersonalSelectionDate = otherDate, UnderGuardianshipDate = otherDate, VotingDate = otherDate };
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.Null(result.TilTidspunkt.ToDateTime());
        }
        #endregion
    }
}
