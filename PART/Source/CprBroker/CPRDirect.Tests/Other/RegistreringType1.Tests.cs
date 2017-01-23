using CprBroker.Providers.CPRDirect;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.CPRDirect.Other
{
    namespace RegistreringType1Tests
    {

        [TestFixture]
        public class TrimFutureTests
        {
            [Test]
            [TestCaseSource(typeof(Utilities), nameof(Utilities.PNRs))]
            public void TrimFuture_Null_HasEgenskab(string pnr)
            {
                var individual = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE).Where(o => o.PersonInformation.PNR == pnr).First();
                var registration = individual.ToRegistreringType1(nr => Guid.NewGuid());
                registration.TrimFuture(null);
                Assert.IsNotEmpty(registration.AttributListe.Egenskab);
            }

            [Test]
            [TestCaseSource(typeof(Utilities), nameof(Utilities.PNRs))]
            public void TrimFuture_Null_HasRegisterOplysning(string pnr)
            {
                var individual = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE).Where(o => o.PersonInformation.PNR == pnr).First();
                var registration = individual.ToRegistreringType1(nr => Guid.NewGuid());
                registration.TrimFuture(null);
                Assert.IsNotEmpty(registration.AttributListe.RegisterOplysning);
            }

            [Test]
            [TestCaseSource(typeof(Utilities), nameof(Utilities.PNRs))]
            public void TrimFuture_MinDate_NoEgenskab(string pnr)
            {
                var individual = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE).Where(o => o.PersonInformation.PNR == pnr).First();
                var registration = individual.ToRegistreringType1(nr => Guid.NewGuid());
                registration.TrimFuture(DateTime.MinValue);
                //System.Diagnostics.Debugger.Launch();
                Assert.IsEmpty(registration.AttributListe.Egenskab);
            }

            [Test]
            [TestCaseSource(typeof(Utilities), nameof(Utilities.PNRs))]
            public void TrimFuture_MinDate_NoRegisterOplysning(string pnr)
            {
                var individual = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE).Where(o => o.PersonInformation.PNR == pnr).First();
                var registration = individual.ToRegistreringType1(nr => Guid.NewGuid());
                registration.TrimFuture(DateTime.MinValue);
                Assert.IsEmpty(registration.AttributListe.RegisterOplysning);
            }
        }
    }
}
