using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CPRDirect;
using NUnit.Framework;
using System.IO;
using CprBroker.Engine.Queues;
using CprBroker.Engine;

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

            [Test]
            public void ImportText_GoesToStagingQueue()
            {
                // TODO: Should this go under CprDirect.Tests??
                CprBroker.Providers.CPRDirect.ExtractManager.ImportText(CprBroker.Tests.CPRDirect.Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE_FixedLength);
                var q = CprBroker.Engine.Queues.Queue.GetQueues<CprBroker.Providers.CPRDirect.ExtractStagingQueue>().Single();
                var items = q.GetNext(1);
                Assert.IsNotEmpty(items);
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

        [TestFixture]
        public class CleanProcessedFolder
        {
            [Test]
            public void CleanProcessedFolder_EmptyFolder_Nothing()
            {
                var path = "Tests\\" + CprBroker.Utilities.Strings.NewRandomString(10);
                var prov = new CPRDirectExtractDataProvider() { ExtractsFolder = path };

                var processedPath = ExtractPaths.ProcessedFolder(prov);
                Directory.CreateDirectory(processedPath);

                ExtractManager.CleanProcessedFolder(prov);
            }

            [Test]
            public void CleanProcessedFolder_RootAndNonRootFiles_Deleted()
            {
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");

                var path = new DirectoryInfo("Tests\\" + CprBroker.Utilities.Strings.NewRandomString(10)).FullName;
                var prov = new CPRDirectExtractDataProvider() { ExtractsFolder = path };

                var processedPath = ExtractPaths.ProcessedFolder(prov);
                var processedSubPath = processedPath + "\\" + CprBroker.Utilities.Strings.NewRandomString(5);

                Directory.CreateDirectory(processedSubPath);

                var files = new string[] {processedPath, processedSubPath }.Select(p=> p + "\\" + CprBroker.Utilities.Strings.NewRandomString(5)).ToArray();
                Array.ForEach(files, f => File.WriteAllText(f, "ABCD"));

                var initialContents = Directory.GetFiles(processedPath, "*", SearchOption.AllDirectories);
                Assert.AreEqual(2, initialContents.Length);

                ExtractManager.CleanProcessedFolder(prov);

                var postContents = Directory.GetFiles(processedPath, "*", SearchOption.AllDirectories);
                Assert.IsEmpty(postContents);

                // Cleanup
                Directory.Delete(path, true);
            }
        }
    }
}
