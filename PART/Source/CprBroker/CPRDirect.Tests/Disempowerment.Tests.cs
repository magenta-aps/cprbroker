using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace DisempowermentTests
    {
        [TestFixture]
        public class ToPersonFlerRelationType
        {
            [Test]
            public void ToPersonFlerRelationType_Null_Empty()
            {
                var ret = DisempowermentType.ToPersonRelationType(null, pnr => Guid.NewGuid());
                Assert.AreEqual(0, ret.Length);
            }

            [Test]
            public void ToPersonFlerRelationType_InvalidPnr_Empty(
                [Values(null, "1234")]string pnr)
            {
                var ret = DisempowermentType.ToPersonRelationType(new DisempowermentType() { PNR = pnr }, cpr => Guid.NewGuid());
                Assert.AreEqual(0, ret.Length);
            }

            [Test]
            public void ToPersonFlerRelationType_Pnr_OneItem(
                [Values("123456789", "1234567890")]string pnr)
            {
                var ret = DisempowermentType.ToPersonRelationType(new DisempowermentType() { RelationPNR = pnr }, cpr => Guid.NewGuid());
                Assert.AreEqual(1, ret.Length);
            }

            [Test]
            public void ToPersonFlerRelationType_StartDate_CorrectStartDate()
            {
                var disemp = new DisempowermentType() { RelationPNRStartDate = DateTime.Today };
                var ret = disemp.ToPersonFlerRelationType(pnr => Guid.NewGuid());
                Assert.AreEqual(DateTime.Today, ret.Virkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonFlerRelationType_StartDate_NullEnd()
            {
                var disemp = new DisempowermentType() { RelationPNRStartDate = DateTime.Today };
                var ret = disemp.ToPersonFlerRelationType(pnr => Guid.NewGuid());
                Assert.Null(ret.Virkning.TilTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonFlerRelationType_EndDateDate_CorrectEndDate()
            {
                var disemp = new DisempowermentType() { DisempowermentEndDate = DateTime.Today };
                var ret = disemp.ToPersonFlerRelationType(pnr => Guid.NewGuid());
                Assert.AreEqual(DateTime.Today, ret.Virkning.TilTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonFlerRelationType_EndDate_NullStartDate()
            {
                var disemp = new DisempowermentType() { DisempowermentEndDate = DateTime.Today };
                var ret = disemp.ToPersonFlerRelationType(pnr => Guid.NewGuid());
                Assert.Null(ret.Virkning.FraTidspunkt.ToDateTime());
            }

        }
    }
}
