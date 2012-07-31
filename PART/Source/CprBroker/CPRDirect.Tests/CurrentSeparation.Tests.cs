using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.CPRDirect
{
    namespace CurrentSeparationTests
    {
        [TestFixture]
        public class ToCivilStatusType
        {
            [Test]
            public void ToCivilStatusType_Today_NotNull()
            {
                var sep = new CurrentSeparationType() { SeparationStartDate = DateTime.Today };
                var ret = sep.ToCivilStatusType();
                Assert.NotNull(sep);
            }

            [Test]
            public void ToCivilStatusType_Today_CorrectStatus()
            {
                var sep = new CurrentSeparationType() { SeparationStartDate = DateTime.Today };
                var ret = sep.ToCivilStatusType();
                Assert.AreEqual(CivilStatusKodeType.Separeret, ret.CivilStatusKode);
            }

            [Test]
            public void ToCivilStatusType_Today_CorrectDate()
            {
                var dt = DateTime.Today;
                var sep = new CurrentSeparationType() { SeparationStartDate = dt };
                var ret = sep.ToCivilStatusType();
                Assert.AreEqual(dt, ret.TilstandVirkning.FraTidspunkt.ToDateTime());
            }
        }
    }
}
