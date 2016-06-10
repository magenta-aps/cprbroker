using CprBroker.Providers.CPRDirect;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.CPRDirect.Queues
{
    [TestFixture]
    public class DbrBaseQueueTests : CprBroker.Tests.PartInterface.TestBase
    {
        public class DbrQueueStub : CprBroker.Engine.Queues.Queue<ExtractQueueItem>
        {
            public override ExtractQueueItem[] Process(ExtractQueueItem[] items)
            {
                throw new NotImplementedException();
            }
        }

        [SetUp]
        public void InitApplication()
        {
            CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "Test user");
        }

        [Test]
        public void RunOneBatch_GoesToDbrQueue()
        {
            // Init DB
            CprBroker.Providers.CPRDirect.ExtractManager.ImportText(CprBroker.Tests.CPRDirect.Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE_FixedLength);
            var stagingQueue = CprBroker.Engine.Queues.Queue.GetQueues<CprBroker.Providers.CPRDirect.ExtractStagingQueue>().Single();
            var dbrBaseQueue = CprBroker.Engine.Queues.Queue.GetQueues<CprBroker.Providers.CPRDirect.DbrBaseQueue>().Single();

            var values = new Dictionary<string, string>();
            CprBroker.Engine.DataProviderConfigProperty.Templates.SetConnectionString("", values);
            var dbrQueue = CprBroker.Engine.Queues.Queue.AddQueue<DbrQueueStub>(CprBroker.Providers.CPRDirect.DbrBaseQueue.TargetQueueTypeId, values, 100, 1);

            // Now run
            stagingQueue.RunOneBatch();
            dbrBaseQueue.RunOneBatch();
            var items = dbrQueue.GetNext(1);
            Assert.IsNotEmpty(items);
        }
    }
}
