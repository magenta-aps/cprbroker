using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using CprBroker.Providers.DPR;

namespace CprBroker.Tests.DPR.CivilStatusTests
{

    [TestFixture]
    class ToPersonRelationType : BaseTests
    {
        [Test]
        [ExpectedException]
        public void ToPersonRelationType_NullInput_Exception()
        {
            var civilStatus = new CivilStatusStub();
            new CivilStatusWrapper(civilStatus).ToPersonRelationType(null);
        }

        [Test]
        public void PersonRelationType_Normal_CorrectUuid(
            [ValueSource("RandomCprNumbers5")]decimal spouseCprNumber)
        {
            var civilStatus = new CivilStatusStub() { SpousePNR = spouseCprNumber };
            var result = new CivilStatusWrapper(civilStatus).ToPersonRelationType(UuidMap.CprStringToUuid);
            Assert.AreEqual(UuidMap.CprToUuid(spouseCprNumber).ToString(), result.ReferenceID.Item);
        }

        [Test]
        public void PersonRelationType_Normal_EmptyComment(
            [ValueSource("RandomCprNumbers5")]decimal spouseCprNumber)
        {
            var civilStatus = new CivilStatusStub() { SpousePNR = spouseCprNumber };
            var result = new CivilStatusWrapper(civilStatus).ToPersonRelationType(UuidMap.CprStringToUuid);
            Assert.IsNullOrEmpty(result.CommentText);
        }

        [Test]
        public void PersonRelationType_StartDateOnly_CorrectEffectStartDate(
            [ValueSource("RandomDecimalDates5")]decimal startDate,
            [Values('U', 'G', 'F', 'D', 'E', 'P', 'O', 'L', 'u', 'g', 'f', 'd', 'e', 'p', 'o', 'l')]char maritalStatus)
        {
            var civilStatus = new CivilStatusStub() { SpousePNR = Utilities.RandomCprNumber(), MaritalStatus = maritalStatus, MaritalStatusDate = startDate, MaritalEndDate = null };
            var result = new CivilStatusWrapper(civilStatus).ToPersonRelationType(UuidMap.CprStringToUuid);
            Assert.AreEqual(Providers.DPR.Utilities.DateFromDecimal(startDate), result.Virkning.FraTidspunkt.ToDateTime().Value);
        }
        [Test]
        public void PersonRelationType_StartDateOnly_NullEffectEnd(
            [ValueSource("RandomDecimalDates5")]decimal startDate,
            [Values('U', 'G', 'F', 'D', 'E', 'P', 'O', 'L', 'u', 'g', 'f', 'd', 'e', 'p', 'o', 'l')]char maritalStatus)
        {
            var civilStatus = new CivilStatusStub() { SpousePNR = Utilities.RandomCprNumber(), MaritalStatus = maritalStatus, MaritalStatusDate = startDate, MaritalEndDate = null };
            var result = new CivilStatusWrapper(civilStatus).ToPersonRelationType(UuidMap.CprStringToUuid);
            Assert.Null(result.Virkning.TilTidspunkt.ToDateTime());
        }
        [Test]
        public void PersonRelationType_StartDateOnly_CorrectEndDate(
            [ValueSource("RandomDecimalDates5")]decimal endDate,
            [Values('U', 'G', 'F', 'D', 'E', 'P', 'O', 'L', 'u', 'g', 'f', 'd', 'e', 'p', 'o', 'l')]char maritalStatus)
        {
            var civilStatus = new CivilStatusStub() { SpousePNR = Utilities.RandomCprNumber(), MaritalStatus = maritalStatus, MaritalStatusDate = null, MaritalEndDate = endDate };
            var result = new CivilStatusWrapper(civilStatus).ToPersonRelationType(UuidMap.CprStringToUuid);
            Assert.AreEqual(Providers.DPR.Utilities.DateFromDecimal(endDate), result.Virkning.TilTidspunkt.ToDateTime());
        }
        [Test]
        public void PersonRelationType_StartDateOnly_NullEffectStart(
            [ValueSource("RandomDecimalDates5")]decimal endDate,
            [Values('U', 'G', 'F', 'D', 'E', 'P', 'O', 'L', 'u', 'g', 'f', 'd', 'e', 'p', 'o', 'l')]char maritalStatus)
        {
            var civilStatus = new CivilStatusStub() { SpousePNR = Utilities.RandomCprNumber(), MaritalStatus = maritalStatus, MaritalStatusDate = null, MaritalEndDate = endDate };
            var result = new CivilStatusWrapper(civilStatus).ToPersonRelationType(UuidMap.CprStringToUuid);
            Assert.Null(result.Virkning.FraTidspunkt.ToDateTime());
        }

    }

}
