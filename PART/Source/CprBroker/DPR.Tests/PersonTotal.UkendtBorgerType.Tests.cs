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

namespace CprBroker.Tests.DPR
{
    [TestFixture]
    public class PersonTotalTests : BaseTests
    {

        #region ToUkendtBorgerType
        [Test]
        public void ToUkendtBorgerType_Normal_NotNull(
            [ValueSource("RandomCprNumbers5")]decimal cprNumber)
        {
            var personTotal = new PersonTotal() { PNR = cprNumber };
            var result = personTotal.ToUkendtBorgerType();
            Assert.NotNull(result);
        }

        [Test]
        public void ToUkendtBorgerType_Normal_CorrectPnr(
            [ValueSource("RandomCprNumbers5")]decimal cprNumber)
        {
            var personTotal = new PersonTotal() { PNR = cprNumber };
            var result = personTotal.ToUkendtBorgerType();
            Assert.AreEqual(cprNumber, decimal.Parse(result.PersonCivilRegistrationReplacementIdentifier));
        }
        #endregion

        #region ToUkendtBorgerTypeVirvning
        [Test]
        public void ToUkendtBorgerTypVirvninge_Empty_NotNull()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUkendtBorgerTypeVirkning();
            Assert.NotNull(result);
        }

        [Test]
        public void ToUkendtBorgerTypVirvninge_Empty_EmptyResultFra()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUkendtBorgerTypeVirkning();
            Assert.Null(result.FraTidspunkt.ToDateTime());
        }

        [Test]
        public void ToUkendtBorgerTypVirvninge_Empty_EmptyResultTil()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUkendtBorgerTypeVirkning();
            Assert.Null(result.TilTidspunkt.ToDateTime());
        }

        [Test]
        public void ToUkendtBorgerTypeVirvning_Normal_CorrectDate(
            [ValueSource("RandomDecimalDates5")]decimal statusDate)
        {
            var personTotal = new PersonTotal() { StatusDate = statusDate };
            var result = personTotal.ToUkendtBorgerTypeVirkning();
            Assert.AreEqual(Providers.DPR.Utilities.DateFromDecimal(statusDate), result.FraTidspunkt.Item);
        }

        [Test]
        public void ToUkendtBorgerTypeVirvning_Normal_EmptyResultTil(
            [ValueSource("RandomDecimalDates5")]decimal statusDate)
        {
            var personTotal = new PersonTotal() { StatusDate = statusDate };
            var result = personTotal.ToUkendtBorgerTypeVirkning();
            Assert.Null(result.TilTidspunkt.ToDateTime());
        }

        [Test]
        public void ToUkendtBorgerTypeVirvning_MiscDates_CorrectDate(
            [ValueSource("RandomDecimalDates5")]decimal statusDate,
            [ValueSource("RandomDecimalDates5")]decimal otherDate)
        {
            var personTotal = new PersonTotal() { StatusDate = statusDate, AddressDate = otherDate, DateOfBirth = otherDate, MaritalStatusDate = otherDate, MunicipalityArrivalDate = otherDate, MunicipalityLeavingDate = otherDate, PaternityDate = otherDate, PersonalSelectionDate = otherDate, UnderGuardianshipDate = otherDate, VotingDate = otherDate };
            var result = personTotal.ToUkendtBorgerTypeVirkning();
            Assert.AreEqual(Providers.DPR.Utilities.DateFromDecimal(statusDate), result.FraTidspunkt.Item);
        }

        [Test]
        public void ToUkendtBorgerTypeVirvning_MiscDates_EmptyResultTil(
            [ValueSource("RandomDecimalDates5")]decimal statusDate,
            [ValueSource("RandomDecimalDates5")]decimal otherDate)
        {
            var personTotal = new PersonTotal() { StatusDate = statusDate, AddressDate = otherDate, DateOfBirth = otherDate, MaritalStatusDate = otherDate, MunicipalityArrivalDate = otherDate, MunicipalityLeavingDate = otherDate, PaternityDate = otherDate, PersonalSelectionDate = otherDate, UnderGuardianshipDate = otherDate, VotingDate = otherDate };
            var result = personTotal.ToUkendtBorgerTypeVirkning();
            Assert.Null(result.TilTidspunkt.ToDateTime());
        }
        #endregion
    }
}
