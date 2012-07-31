using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;
namespace CprBroker.Tests.CPRDirect
{
    namespace PersonInformationTests
    {
        [TestFixture]
        public class LoadAll_
        {
            [Test]
            public void LoadAll()
            {
                var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                System.Diagnostics.Debugger.Launch();
                var ss = all
                    .AsQueryable()
                    .GroupBy(p => new { Value = p.PersonInformation.Birthdate.HasValue, Certain = p.PersonInformation.BirthdateUncertainty })
                    .Select(g => new { Value = g.Key.Value, Certainty = g.Key.Certain, Data = g.ToArray() })
                    .ToArray();
                object dd = "";
            }
        }

        [TestFixture]
        public class ToBirthdate
        {
            [Test]
            public void ToBirthdate_Empty_Null()
            {
                var inf = new PersonInformationType();
                var ret = inf.ToBirthdate();
                Assert.Null(ret);
            }

            [Test]
            public void ToBirthdate_UncertainValue_Null(
                [Values('s', '2', '4', 'S')] char uncertainty)
            {
                var inf = new PersonInformationType() { Birthdate = DateTime.Today, BirthdateUncertainty = uncertainty };
                var ret = inf.ToBirthdate();
                Assert.Null(ret);
            }

            [Test]
            public void ToBirthdate_Value_OK()
            {
                var inf = new PersonInformationType() { Birthdate = DateTime.Today };
                var ret = inf.ToBirthdate();
                Assert.AreEqual(DateTime.Today, ret);
            }
        }

        [TestFixture]
        public class ToStatusDate
        {
            [Test]
            public void ToStatusDate_Empty_Null()
            {
                var inf = new PersonInformationType();
                var ret = inf.ToStatusDate();
                Assert.Null(ret);
            }

            [Test]
            public void ToStatusDate_UncertainValue_Null(
                [Values('s', '2', '4', 'S')] char uncertainty)
            {
                var inf = new PersonInformationType() { StatusStartDate = DateTime.Today, StatusDateUncertainty = uncertainty };
                var ret = inf.ToStatusDate();
                Assert.Null(ret);
            }

            [Test]
            public void ToStatusDate_Value_OK()
            {
                var inf = new PersonInformationType() { StatusStartDate = DateTime.Today };
                var ret = inf.ToStatusDate();
                Assert.AreEqual(DateTime.Today, ret.Value);
            }
        }

        [TestFixture]
        public class ToPnr
        {
            [Test]
            public void ToPnr_Normal_OK(
                [Values("1234567890", "123456789")]string pnr)
            {
                var inf = new PersonInformationType() { PNR = pnr };
                var ret = inf.ToPnr();
                Assert.IsNotNullOrEmpty(ret);
            }

            [Test]
            public void ToPnr_Invalid_Emypty(
                [Values("12345678", "0", "224123", "12345", "123")]string pnr)
            {
                var inf = new PersonInformationType() { PNR = pnr };
                var ret = inf.ToPnr();
                Assert.IsNullOrEmpty(ret);
            }
        }

        [TestFixture]
        public class ToPersonGenderCodeType
        {
            [Test]
            public void PersonGenderCodeType_M_Male(
                [Values('M', 'm')]char gender)
            {
                var info = new PersonInformationType() { Gender = gender };
                var ret = info.ToPersonGenderCodeType();
                Assert.AreEqual(PersonGenderCodeType.male, ret);
            }

            [Test]
            public void PersonGenderCodeType_K_Female(
                [Values('K', 'k')]char gender)
            {
                var info = new PersonInformationType() { Gender = gender };
                var ret = info.ToPersonGenderCodeType();
                Assert.AreEqual(PersonGenderCodeType.female, ret);
            }

            [Test]
            [ExpectedException]
            public void PersonGenderCodeType_Other_Exception(
                [Values('s', ' ', '2')]char gender)
            {
                var info = new PersonInformationType() { Gender = gender };
                var ret = info.ToPersonGenderCodeType();
            }
        }

        [TestFixture]
        public class ToLivStatusType
        {
            [Test]
            public void ToLivStatusType_Status_CorrectStatus(
                [Values(1, 3, 5, 50, 60)]decimal status)
            {
                var inf = new PersonInformationType() { Status = status };
                var res = inf.ToLivStatusType();
                Assert.AreEqual(LivStatusKodeType.Foedt, res.LivStatusKode);
            }

            [Test]
            public void ToLivStatusType_Status_HasVirkning(
                [Values(1, 3, 5, 50, 60)]decimal status)
            {
                var inf = new PersonInformationType() { Status = status };
                var res = inf.ToLivStatusType();
                Assert.NotNull(res.TilstandVirkning);
            }

            [Test]
            public void ToLivStatusType_Status_EmptyDate(
                [Values(1, 3, 5, 50, 60)]decimal status)
            {
                var inf = new PersonInformationType() { Status = status };
                var res = inf.ToLivStatusType();
                Assert.Null(res.TilstandVirkning.FraTidspunkt.ToDateTime());
            }


        }
    }
}
