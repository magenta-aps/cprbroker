using CprBroker.Engine;
using CprBroker.Providers.DPR;
using CprBroker.Providers.DPR.Queues;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.DPR
{
    namespace DprUpdateQueueTests
    {
        [TestFixture]
        public class Process : PartInterface.TestBase
        {
            [Test]
            public void Process_NonExixting_Fails(
                [ValueSource(typeof(Tests.PartInterface.Utilities), nameof(Tests.PartInterface.Utilities.RandomCprNumbers5))]string pnr)
            {
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
                var prov = new DprDatabaseDataProvider() { ConfigurationProperties = new Dictionary<string, string>(), AutoUpdate = true };
                var dbProv = AddDataProvider<DprDatabaseDataProvider>(prov);
                var queue = new DprUpdateQueue();
                var ret = queue.Process(new DprUpdateQueueItem[] { new DprUpdateQueueItem() { DataProviderId = dbProv.DataProviderId, Pnr = decimal.Parse(pnr) } });
                Assert.IsEmpty(ret);
            }

        }
    }
}
