using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace IndividualResponseTests
    {
        [TestFixture]
        public class ToRegistreringType1
        {
            [Test]
            public void ToRegistreringType1_()
            {
                var individual = IndividualResponseType.ParseBatch(Properties.Resources.PNR_0101965058).First();
                var registration = individual.ToRegistreringType1(pnr => Guid.NewGuid(), DateTime.Today);
                Assert.NotNull(registration);
            }
        }
    }
}
