using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.CPRDirect
{
    namespace ExtractStagingQueueTests
    {
        [TestFixture]
        public class ExtractStagingQueueTests : CprBroker.Tests.PartInterface.TestBase
        {
            [SetUp]
            public void InitApplication()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "Test user");
            }

            [Test]
            public void RunOneBatch_GoesToBaseDbrQueue()
            {
                CprBroker.Providers.CPRDirect.ExtractManager.ImportText(CprBroker.Tests.CPRDirect.Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE_FixedLength);
                var stagingQueue = CprBroker.Engine.Queues.Queue.GetQueues<CprBroker.Providers.CPRDirect.ExtractStagingQueue>().Single();

                stagingQueue.RunOneBatch();

                var dbrBaseQueue = CprBroker.Engine.Queues.Queue.GetQueues<CprBroker.Providers.CPRDirect.DbrBaseQueue>().Single();
                var items = dbrBaseQueue.GetNext(1);
                Assert.IsNotEmpty(items);
            }
        }

    }
}
