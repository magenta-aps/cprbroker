using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.CPRDirect
{
    namespace CurrentCivilStatusTests
    {
        [TestFixture]
        public class ToCivilStatusType
        {
            [Test]
            public void ToCivilStatusType_NoSeparation_StatusDate(
                [Values('D', 'E', 'F', 'G', 'L', 'O', 'P', 'U')]char maritalStatus)
            {
                var status = new CivilStatusWrapper(new CurrentCivilStatusType() { CivilStatusStartDate = DateTime.Today, CivilStatusStartDateUncertainty = ' ', CivilStatusCode = maritalStatus });
                var ret = status.ToCivilStatusType(null);
                Assert.AreEqual(DateTime.Today, ret.TilstandVirkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToCivilStatusType_MarriedWithSeparation_StatusDate(
                [Values('G', 'P')]char maritalStatus)
            {
                var status = new CivilStatusWrapper(new CurrentCivilStatusType() { CivilStatusStartDate = DateTime.Today.AddDays(-1), CivilStatusStartDateUncertainty = ' ', CivilStatusCode = maritalStatus });
                var sep = new CurrentSeparationType() { SeparationStartDate = DateTime.Today, SeparationStartDateUncertainty = ' ' };
                var ret = status.ToCivilStatusType(sep);
                Assert.AreEqual(sep.ToSeparationStartDate(), ret.TilstandVirkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToCivilStatusType_OtherThanMarriedWithSeparation_StatusDate(
                [Values('D', 'E', 'F', 'L', 'O', 'U')]char maritalStatus)
            {
                var status = new CivilStatusWrapper(new CurrentCivilStatusType() { CivilStatusStartDate = DateTime.Today.AddDays(-1), CivilStatusStartDateUncertainty = ' ', CivilStatusCode = maritalStatus });
                var sep = new CurrentSeparationType() { SeparationStartDate = DateTime.Today, SeparationStartDateUncertainty = ' ' };
                var ret = status.ToCivilStatusType(sep);
                Assert.AreEqual(status.ToCivilStatusDate(), ret.TilstandVirkning.FraTidspunkt.ToDateTime());
            }
        }

        [TestFixture]
        public class ToSpouses
        {
            [Test]
            public void ToSpouses_InvalidPNR_EmptyArray(
                [Values('U', 'G', 'F', 'E', 'P', 'O', 'L', 'D')]char maritalStatus,
                [Values(1, 20, 130, 12345678)]decimal pnr)
            {
                var civil = new CurrentCivilStatusType() { CivilStatusCode = maritalStatus, PNR = Converters.DecimalToString(pnr, 10) };
                var ret = CivilStatusWrapper.ToSpouses(civil, null, cpr => Guid.NewGuid());
                Assert.IsEmpty(ret);
            }
        }

        [TestFixture]
        public class ToPersonRelationTypeArray
        {
            [Test]
            public void ToPersonRelationTypeArray_Married_CorrectStartDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char civilStatus
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatusCode = civilStatus, CivilStatusStartDate = DateTime.Today, SpousePNR = Utilities.RandomCprNumberString() };
                var ret = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(), civilStatus, Utilities.RandomSingleChar(Utilities.AlphabetChars, civilStatus), Utilities.RandomSingleChar(Utilities.AlphabetChars, civilStatus));
                Assert.AreEqual(DateTime.Today, ret[0].Virkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonRelationTypeArray_Married_NullEndDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char civilStatus,
                [Values(true, false)]bool sameGenderForDead
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatusCode = civilStatus, CivilStatusStartDate = DateTime.Today, SpousePNR = Utilities.RandomCprNumberString() };
                var ret = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(), civilStatus, Utilities.RandomSingleChar(Utilities.AlphabetChars, civilStatus), Utilities.RandomSingleChar(Utilities.AlphabetChars, civilStatus));
                Assert.Null(ret[0].Virkning.TilTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonRelationTypeArray_UnMarried_CorrectEndDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char endedCivilStatus
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatusCode = endedCivilStatus, CivilStatusStartDate = DateTime.Today, SpousePNR = Utilities.RandomCprNumberString() };
                var ret = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(), Utilities.RandomSingleChar(Utilities.AlphabetChars, endedCivilStatus), endedCivilStatus, Utilities.RandomSingleChar(Utilities.AlphabetChars, endedCivilStatus));
                Assert.AreEqual(DateTime.Today, ret[0].Virkning.TilTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonRelationTypeArray_UnMarried_NullStartDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char endedCivilStatus,
                [Values(true, false)]bool sameGenderForDead
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatusCode = endedCivilStatus, CivilStatusStartDate = DateTime.Today, SpousePNR = Utilities.RandomCprNumberString() };
                var ret = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(), Utilities.RandomSingleChar(Utilities.AlphabetChars, endedCivilStatus), endedCivilStatus, Utilities.RandomSingleChar(Utilities.AlphabetChars, endedCivilStatus));
                Assert.Null(ret[0].Virkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonRelationTypeArray_DeadNoPnr_Empty(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char deadCivilStatus
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatusCode = deadCivilStatus, CivilStatusStartDate = DateTime.Today };
                var ret = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(), Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus), Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus), deadCivilStatus);
                Assert.IsEmpty(ret);
            }

            [Test]
            public void ToPersonRelationTypeArray_DeadWithPnr_NotEmpty(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char deadCivilStatus
                )
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var f = result.Where(r => r.CurrentCivilStatus.CivilStatusCode == 'D' || r.HistoricalCivilStatus.Where(c => c.CivilStatusCode == 'D').Count() > 0).ToArray(); ;
                var civil = new CurrentCivilStatusType() { CivilStatusCode = deadCivilStatus, CivilStatusStartDate = DateTime.Today, PNR = Utilities.RandomCprNumberString(), SpousePNR = Utilities.RandomCprNumberString() };
                var ret1 = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(),
                    Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus),
                    Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus),
                    Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus)
                    );
                var ret2 = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(), Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus), Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus), Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus));
                Assert.AreEqual(0, ret1.Length + ret2.Length);
            }

        }
    }
}
