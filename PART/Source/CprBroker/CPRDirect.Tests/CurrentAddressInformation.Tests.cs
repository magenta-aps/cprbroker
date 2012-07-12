using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas.Part;
using CprBroker.Providers.CPRDirect;


namespace CprBroker.Tests.CPRDirect
{
    namespace CurrentAddressInformationTests
    {
        [TestFixture]
        public class ToVirkningTypeArray
        {
            [Test]
            public void ToVirkningTypeArray_Empty_Empty()
            {
                var db = new CurrentAddressInformationType();
                var ret = db.ToVirkningTypeArray();
                var v = VirkningType.Compose(ret);
                Assert.True(VirkningType.IsDoubleOpen(v));
            }
        }

        [TestFixture]
        public class ToRelocationVirkning
        {
            [Test]
            public void ToRelocationVirkning_Empty_OpenStart()
            {
                var db = new CurrentAddressInformationType();
                var ret = db.ToRelocationVirkning();
                Assert.Null(ret.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToRelocationVirkning_Date_ClosedStart()
            {
                var db = new CurrentAddressInformationType() { RelocationDate = DateTime.Today };
                var ret = db.ToRelocationVirkning();
                Assert.NotNull(ret.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToRelocationVirkning_EmptyOrValue_OpenEnd(
                [Values(true, false)]bool open)
            {
                var db = new CurrentAddressInformationType() { RelocationDate = open ? (DateTime?)null : DateTime.Today };
                var ret = db.ToRelocationVirkning();
                Assert.Null(ret.TilTidspunkt.ToDateTime());
            }
        }

    }
}
