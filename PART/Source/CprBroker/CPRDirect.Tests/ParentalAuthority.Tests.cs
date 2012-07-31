using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace ParentalAuthorityTests
    {
        [TestFixture]
        public class ToPersonRelationType
        {
            [Test]
            public void LoadAll()
            {
                var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var ss = all
                    .Where(p => p.ParentalAuthority.Count() > 0)
                    .Select(p => p.ParentalAuthority.ToArray())
                    .ToArray();
                object o = "";
            }

            [Test]
            public void ToPersonRelationType_NoPnr_Empty(
            [Values(null, "")]string pnr)
            {
                var auth = new ParentalAuthorityType() { RelationPNR = pnr };
                var ret = ParentalAuthorityType.ToPersonRelationType(new ParentalAuthorityType[] { auth }, cpr => Guid.NewGuid());
                Assert.AreEqual(0, ret.Length);
            }

            [Test]
            public void ToPersonRelationType_PnrWithStartDate_CorrectStartDate(
            [Values("1234567890", "0123456789")]string pnr)
            {
                var auth = new ParentalAuthorityType() { RelationPNR = pnr, RelationPNRStartDate = DateTime.Today };
                var ret = auth.ToPersonRelationType(cpr => Guid.NewGuid());
                Assert.AreEqual(DateTime.Today, ret.Virkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonRelationType_PnrWithStartDate_NullEndDate(
            [Values("1234567890", "0123456789")]string pnr)
            {
                var auth = new ParentalAuthorityType() { RelationPNR = pnr, RelationPNRStartDate = DateTime.Today };
                var ret = auth.ToPersonRelationType(cpr => Guid.NewGuid());
                Assert.Null(ret.Virkning.TilTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonRelationType_PnrWithEndDate_CorrectEndDate(
            [Values("1234567890", "0123456789")]string pnr)
            {
                var auth = new ParentalAuthorityType() { RelationPNR = pnr, CustodyEndDate = DateTime.Today };
                var ret = auth.ToPersonRelationType(cpr => Guid.NewGuid());
                Assert.AreEqual(DateTime.Today, ret.Virkning.TilTidspunkt.ToDateTime());
            }

        }
    }
}
