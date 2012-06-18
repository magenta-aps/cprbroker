using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace IndividualRequestTests
    {
        [TestFixture]
        public class IndividualRequestType_
        {
            [Test]
            public void IndividualRequestType_Pnr_CorrectPnr(
                [Values(0, 1, 12, 56, 123456789)]decimal pnr,
                [Values(true, false)] bool subscription)
            {
                var req = new IndividualRequestType(subscription, pnr);
                Assert.AreEqual(pnr, req.PNR);
            }

            [Test]
            public void IndividualRequestType_Pnr_IdentifiedByTask(
                [Values(0, 1, 12, 56, 123456789)]decimal pnr,
                [Values(true, false)] bool subscription)
            {
                var req = new IndividualRequestType(subscription, pnr);
                Assert.AreEqual(DataType.DefinedByTask, (DataType)req.DataType);
            }
        }
    }
}
