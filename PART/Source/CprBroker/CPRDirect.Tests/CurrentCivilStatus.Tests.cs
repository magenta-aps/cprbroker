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
                var status = new CivilStatusWrapper(new CurrentCivilStatusType() { CivilStatusStartDate = DateTime.Today, CivilStatusStartDateUncertainty = ' ', CivilStatus = maritalStatus });
                var ret = status.ToCivilStatusType(null);
                Assert.AreEqual(DateTime.Today, ret.TilstandVirkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToCivilStatusType_WithSeparation_StatusDate(
                [Values('D', 'E', 'F', 'G', 'L', 'O', 'P', 'U')]char maritalStatus)
            {
                var status = new CivilStatusWrapper(new CurrentCivilStatusType() { CivilStatusStartDate = DateTime.Today.AddDays(-1), CivilStatusStartDateUncertainty = ' ', CivilStatus = maritalStatus });
                var sep = new CurrentSeparationType() { SeparationStartDate = DateTime.Today, SeparationStartDateUncertainty = ' ' };
                var ret = status.ToCivilStatusType(sep);
                Assert.AreEqual(sep.ToSeparationStartDate(), ret.TilstandVirkning.FraTidspunkt.ToDateTime());
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
                var civil = new CurrentCivilStatusType() { CivilStatus = maritalStatus, PNR = Converters.DecimalToString(pnr, 10) };
                var ret = CivilStatusWrapper.ToSpouses(civil, null, cpr => Guid.NewGuid());
                Assert.IsEmpty(ret);
            }

            [Test]
            public void ToSpouses_Married_CorrectStartDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char civilStatus,
                [Values(true, false)]bool sameGenderForDead
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatus = civilStatus, CivilStatusStartDate = DateTime.Today, SpousePNR = Utilities.RandomCprNumberString() };
                var ret = CivilStatusWrapper.ToSpouses(civil, null, civilStatus, Utilities.RandomChar(Utilities.AlphabetChars, 2, civilStatus), Utilities.RandomChar(Utilities.AlphabetChars, 1, civilStatus)[0], sameGenderForDead, pnr => Guid.NewGuid());
                Assert.AreEqual(DateTime.Today, ret[0].Virkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToSpouses_Married_NullEndDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char civilStatus,
                [Values(true, false)]bool sameGenderForDead
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatus = civilStatus, CivilStatusStartDate = DateTime.Today, SpousePNR = Utilities.RandomCprNumberString() };
                var ret = CivilStatusWrapper.ToSpouses(civil, null, civilStatus, Utilities.RandomChar(Utilities.AlphabetChars, 2, civilStatus), Utilities.RandomChar(Utilities.AlphabetChars, 1, civilStatus)[0], sameGenderForDead, pnr => Guid.NewGuid());
                Assert.Null(ret[0].Virkning.TilTidspunkt.ToDateTime());
            }

            [Test]
            public void ToSpouses_UnMarried_CorrectEndDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char endedCivilStatus,
                [Values(true, false)]bool sameGenderForDead
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatus = endedCivilStatus, CivilStatusStartDate = DateTime.Today, SpousePNR = Utilities.RandomCprNumberString() };
                var ret = CivilStatusWrapper.ToSpouses(civil, null, Utilities.RandomChar(Utilities.AlphabetChars, 1, endedCivilStatus)[0], new char[] { endedCivilStatus }, Utilities.RandomChar(Utilities.AlphabetChars, 1, endedCivilStatus)[0], sameGenderForDead, pnr => Guid.NewGuid());
                Assert.AreEqual(DateTime.Today, ret[0].Virkning.TilTidspunkt.ToDateTime());
            }

            [Test]
            public void ToSpouses_UnMarried_NullStartDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char endedCivilStatus,
                [Values(true, false)]bool sameGenderForDead
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatus = endedCivilStatus, CivilStatusStartDate = DateTime.Today, SpousePNR = Utilities.RandomCprNumberString() };
                var ret = CivilStatusWrapper.ToSpouses(civil, null, Utilities.RandomChar(Utilities.AlphabetChars, 1, endedCivilStatus)[0], new char[] { endedCivilStatus }, Utilities.RandomChar(Utilities.AlphabetChars, 1, endedCivilStatus)[0], sameGenderForDead, pnr => Guid.NewGuid());
                Assert.Null(ret[0].Virkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToSpouses_DeadNoPnr_Empty(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char deadCivilStatus,
                [Values(true, false)]bool sameGenderForDead
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatus = deadCivilStatus, CivilStatusStartDate = DateTime.Today };
                var ret = CivilStatusWrapper.ToSpouses(civil, null, Utilities.RandomChar(Utilities.AlphabetChars, 1, deadCivilStatus)[0], Utilities.RandomChar(Utilities.AlphabetChars, 2, deadCivilStatus), deadCivilStatus, sameGenderForDead, pnr => Guid.NewGuid());
                Assert.IsEmpty(ret);
            }

            [Test]
            public void ToSpouses_DeadWithPnr_NotEmpty(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char deadCivilStatus
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatus = deadCivilStatus, CivilStatusStartDate = DateTime.Today, PNR = Utilities.RandomCprNumberString(), SpousePNR = Utilities.RandomCprNumberString() };
                var ret1 = CivilStatusWrapper.ToSpouses(civil, null, Utilities.RandomChar(Utilities.AlphabetChars, 1, deadCivilStatus)[0], Utilities.RandomChar(Utilities.AlphabetChars, 2, deadCivilStatus), deadCivilStatus, true, pnr => Guid.NewGuid());
                var ret2 = CivilStatusWrapper.ToSpouses(civil, null, Utilities.RandomChar(Utilities.AlphabetChars, 1, deadCivilStatus)[0], Utilities.RandomChar(Utilities.AlphabetChars, 2, deadCivilStatus), deadCivilStatus, false, pnr => Guid.NewGuid());
                Assert.AreEqual(1, ret1.Length + ret2.Length);
            }

            [Test]
            public void ToSpouses_DeadWithPnr_CorrectEndDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char deadCivilStatus
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatus = deadCivilStatus, CivilStatusStartDate = DateTime.Today, PNR = Utilities.RandomCprNumberString(), SpousePNR = Utilities.RandomCprNumberString() };
                var ret1 = CivilStatusWrapper.ToSpouses(civil, null, Utilities.RandomChar(Utilities.AlphabetChars, 1, deadCivilStatus)[0], Utilities.RandomChar(Utilities.AlphabetChars, 2, deadCivilStatus), deadCivilStatus, true, pnr => Guid.NewGuid());
                var ret2 = CivilStatusWrapper.ToSpouses(civil, null, Utilities.RandomChar(Utilities.AlphabetChars, 1, deadCivilStatus)[0], Utilities.RandomChar(Utilities.AlphabetChars, 2, deadCivilStatus), deadCivilStatus, false, pnr => Guid.NewGuid());
                Assert.AreEqual(DateTime.Today, ret1.Concat(ret2).First().Virkning.TilTidspunkt.ToDateTime());
            }

            [Test]
            public void ToSpouses_DeadWithPnr_NullStartDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char deadCivilStatus
                )
            {
                var sss = Utilities.RandomCprNumberString();
                var civil = new CurrentCivilStatusType() { CivilStatus = deadCivilStatus, CivilStatusStartDate = DateTime.Today, PNR = Utilities.RandomCprNumberString(), SpousePNR = Utilities.RandomCprNumberString() };
                var ret1 = CivilStatusWrapper.ToSpouses(civil,null,Utilities.RandomChar(Utilities.AlphabetChars, 1, deadCivilStatus)[0], Utilities.RandomChar(Utilities.AlphabetChars, 2, deadCivilStatus), deadCivilStatus, true, pnr => Guid.NewGuid());
                var ret2 = CivilStatusWrapper.ToSpouses(civil,null,Utilities.RandomChar(Utilities.AlphabetChars, 1, deadCivilStatus)[0], Utilities.RandomChar(Utilities.AlphabetChars, 2, deadCivilStatus), deadCivilStatus, false, pnr => Guid.NewGuid());
                Assert.Null(ret1.Concat(ret2).First().Virkning.FraTidspunkt.ToDateTime());
            }

        }
    }
}
