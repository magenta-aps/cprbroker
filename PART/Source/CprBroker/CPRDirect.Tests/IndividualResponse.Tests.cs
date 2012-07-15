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

            [Test]
            public void ToRegistreringType1_Parsed_Passes(
                [Range(0, 79)]int index)
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var ret = result[index].ToRegistreringType1(cpr => Guid.NewGuid(), DateTime.Today);
            }

        }
    }
}
