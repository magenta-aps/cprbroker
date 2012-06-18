using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace ParentsInformationTests
    {
        [TestFixture]
        public class ToFatherOrMother
        {
            [Test]
            public void ToFatherOrMother_Null_Zero(
                [Values(null, "")]string pnr)
            {
                var ret = ParentsInformationType.ToFatherOrMother(par => Guid.NewGuid(), pnr, null);
                Assert.AreEqual(0, ret.Length);
            }

            [Test]
            public void ToFatherOrMother_InvalidPnr_One(
                [Values("1234", "12345678")]string pnr)
            {
                var ret = ParentsInformationType.ToFatherOrMother(par => Guid.NewGuid(), pnr, null);
                Assert.AreEqual(1, ret.Length);
            }

            [Test]
            public void ToFatherOrMother_PnrWithData_CorrectDate(
                [Values("1234", "12345678")]string pnr)
            {
                var ret = ParentsInformationType.ToFatherOrMother(par => Guid.NewGuid(), pnr, DateTime.Today);
                Assert.AreEqual(DateTime.Today, ret[0].Virkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToFatherOrMother_PnrWithData_NullEndDate(
                [Values("1234", "12345678")]string pnr)
            {
                var ret = ParentsInformationType.ToFatherOrMother(par => Guid.NewGuid(), pnr, DateTime.Today);
                Assert.Null(ret[0].Virkning.TilTidspunkt.ToDateTime());
            }
        }

        [TestFixture]
        public class ToFather
        {
            string LatestPnr;
            Guid GetUuid(string pnr)
            {
                LatestPnr = pnr;
                return Guid.NewGuid();
            }

            [Test]
            public void ToFather_PNR_PassedCorrectly(
                [Values("123456789", "0123456789", "1234567890")]string pnr)
            {
                var info = new ParentsInformationType() { FatherPNR = pnr };
                var ret = info.ToFather(GetUuid);
                pnr = Converters.ToPnrStringOrNull(pnr);
                Assert.AreEqual(pnr, LatestPnr);
            }

            [Test]
            public void ToFather_PNRWithDate_CorrectStartDate(
                [Values("123456789", "0123456789", "1234567890")]string pnr)
            {
                var info = new ParentsInformationType() { FatherPNR = pnr, FatherDate = DateTime.Today };
                var ret = info.ToFather(GetUuid);
                Assert.AreEqual(DateTime.Today, ret[0].Virkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToFather_PNRWithDate_EmptyEndDate(
                [Values("123456789", "0123456789", "1234567890")]string pnr)
            {
                var info = new ParentsInformationType() { FatherPNR = pnr, FatherDate = DateTime.Today };
                var ret = info.ToFather(GetUuid);
                Assert.Null(ret[0].Virkning.TilTidspunkt.ToDateTime());
            }
        }

        [TestFixture]
        public class ToMother
        {
            string LatestPnr;
            Guid GetUuid(string pnr)
            {
                LatestPnr = pnr;
                return Guid.NewGuid();
            }

            [Test]
            public void ToMother_PNR_PassedCorrectly(
                [Values("123456789", "0123456789", "1234567890")]string pnr)
            {
                var info = new ParentsInformationType() { MotherPNR = pnr };
                var ret = info.ToMother(GetUuid);
                pnr = Converters.ToPnrStringOrNull(pnr);
                Assert.AreEqual(pnr, LatestPnr);
            }

            [Test]
            public void ToMother_PNRWithDate_CorrectStartDate(
                [Values("123456789", "0123456789", "1234567890")]string pnr)
            {
                var info = new ParentsInformationType() { MotherPNR = pnr, MotherDate = DateTime.Today };
                var ret = info.ToMother(GetUuid);
                Assert.AreEqual(DateTime.Today, ret[0].Virkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToMother_PNRWithDate_EmptyEndDate(
                [Values("123456789", "0123456789", "1234567890")]string pnr)
            {
                var info = new ParentsInformationType() { MotherPNR = pnr, MotherDate = DateTime.Today };
                var ret = info.ToMother(GetUuid);
                Assert.Null(ret[0].Virkning.TilTidspunkt.ToDateTime());
            }
        }

    }
}
