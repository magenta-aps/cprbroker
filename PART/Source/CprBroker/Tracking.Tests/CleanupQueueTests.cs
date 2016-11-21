using CprBroker.Engine;
using CprBroker.PartInterface.Tracking;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.Tracking
{
    namespace CleanupQueueTests
    {
        [TestFixture]
        public class RemovePerson : PartInterface.TestBase
        {
            [SetUp]
            public void InitBrokerContext()
            {
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            }

            [Test]
            public void RemovePerson_RegistrationExists_Removed(
                [Values(1, 5, 13, 22, 39, 58)]int pnrIndex)
            {
                var pnr = CPRDirect.Utilities.PNRs[pnrIndex];

                Func<string, int> countItems = (s) =>
                {
                    using (var dc = new ExtractDataContext())
                    {
                        if (string.IsNullOrEmpty(s))
                            return dc.ExtractItems.Count();
                        else
                            return dc.ExtractItems.Where(ei => ei.PNR == pnr).Count();
                    }
                };
                ExtractManager.ImportText(CprBroker.Tests.CPRDirect.Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE_FixedLength);
                Assert.Greater(countItems(pnr), 0);
                var pId = new PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() };
                var queue = new CleanupQueue();
                var succeeded = queue.Process(new CleanupQueueItem[] { new CleanupQueueItem() { PersonUuid = pId.UUID.Value, PNR = pId.CprNumber } });
                Assert.AreEqual(1, succeeded.Length);
                Assert.AreEqual(countItems(pnr), 0);
                Assert.Greater(countItems(null), 0);
            }
        }
    }
}
