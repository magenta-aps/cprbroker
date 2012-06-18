using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace ChildTests
    {
        [TestFixture]
        public class ToPersonFlerRelationType
        {
            [Test]
            public void ToPersonFlerRelationType_InvalidPnr_OK(
                [Values("1234", "0")]string pnr)
            {
                var ret = ChildType.ToPersonFlerRelationType(new ChildType[] { new ChildType() { ChildPNR = pnr } }, cpr => Guid.NewGuid());
            }

            [Test]
            public void ToPersonFlerRelationType_Normal_NullStartDate(
                [Values("1234567890", "123456789")]string pnr)
            {
                var ret = ChildType.ToPersonFlerRelationType(new ChildType[] { new ChildType() { ChildPNR = pnr } }, cpr => Guid.NewGuid());
                Assert.Null(ret[0].Virkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonFlerRelationType_Normal_NullEndDate(
                [Values("1234567890", "123456789")]string pnr)
            {
                var ret = ChildType.ToPersonFlerRelationType(new ChildType[] { new ChildType() { ChildPNR = pnr } }, cpr => Guid.NewGuid());
                Assert.Null(ret[0].Virkning.TilTidspunkt.ToDateTime());
            }
        }
    }
}
