using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CPRDirect;
using NUnit.Framework;
using System.IO;
using CprBroker.Engine.Queues;

namespace CprBroker.Tests.CPRDirect
{
    namespace ExtractManagerTests
    {
        [TestFixture]
        public class ImportText : CprBroker.Tests.PartInterface.TestBase
        {
            [SetUp]
            public void InitContext()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            }

            [Test]
            public void ImportText_Normal_OK()
            {
                ExtractManager.ImportText(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE_FixedLength);
            }
        }

        [TestFixture]
        public class ImportParseResult : CprBroker.Tests.PartInterface.TestBase
        {
            [SetUp]
            public void InitContext()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            }

            [Test]
            public void ImportParseResult_Enqueue_QueueElementsExist([Values(true, false)]bool enqueue)
            {
                var parseResult = new ExtractParseSession(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE_FixedLength, Constants.DataObjectMap);
                ExtractManager.ImportParseResult(parseResult, "", enqueue);

                using (var dataContext = new ExtractDataContext())
                {
                    var extract = dataContext.Extracts.Single();
                    var queue = CprBroker.Engine.Queues.Queue.GetQueues<ExtractStagingQueue>().Single();
                    var queueItems = queue.GetNext(1000);
                    if (enqueue)
                        Assert.AreEqual(80, queueItems.Count());
                    else
                        Assert.AreEqual(0, queueItems.Count());
                }
            }

            [Test]
            public void ImportParseResult_SemaphoreSignalled([Values(true, false)]bool enqueue)
            {
                var parseResult = new ExtractParseSession(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE_FixedLength, Constants.DataObjectMap);
                ExtractManager.ImportParseResult(parseResult, "", enqueue);

                using (var dataContext = new ExtractDataContext())
                {
                    var extract = dataContext.Extracts.Single();
                    var semaphore = Semaphore.GetById(extract.SemaphoreId.Value);
                    Assert.True(semaphore.Impl.SignaledDate.HasValue);
                    Assert.AreEqual(0, semaphore.Impl.WaitCount);
                }
            }
        }
    }
}
