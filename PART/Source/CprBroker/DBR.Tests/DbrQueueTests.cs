using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.DBR;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.DBR
{
    namespace DbrQueueTests
    {
        [TestFixture]
        public class RunOneBatch : DbrTestBase
        {
            [Test]
            public void RunOneBatch_OnePerson_GoesToDbrDatabase([ValueSource(typeof(PerPerson.PersonBaseTest), "CprNumbers")] string pnr)
            {
                // Init DB & env
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "test user");
                CprBroker.Providers.CPRDirect.ExtractManager.ImportText(CprBroker.Tests.CPRDirect.Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE_FixedLength);

                var values = new Dictionary<string, string>();
                CprBroker.Engine.DataProviderConfigProperty.Templates.SetConnectionString(DbrDatabase.ConnectionString, values);

                Guid extractId;
                using (var partDataContext = new CprBroker.Providers.CPRDirect.ExtractDataContext())
                {
                    extractId = partDataContext.Extracts.Single().ExtractId;
                }

                using (var dbrDataContext = new System.Data.Linq.DataContext(DbrDatabase.ConnectionString))
                {
                    var sql = "select count(*) from DTTOTAL WHERE PNR = {0}";
                    var c1 = dbrDataContext.ExecuteQuery<int>(sql, pnr).Single();
                    Assert.AreEqual(0, c1);

                    var dbrQueue = CprBroker.Engine.Queues.Queue.AddQueue<DbrQueue>(CprBroker.Providers.CPRDirect.DbrBaseQueue.TargetQueueTypeId, values, 100, 1);
                    var dbrQueueItem = new CprBroker.Providers.CPRDirect.ExtractQueueItem() { ExtractId = extractId, PNR = pnr };
                    dbrQueue.Enqueue(dbrQueueItem);

                    dbrQueue.RunOneBatch();

                    var c2 = dbrDataContext.ExecuteQuery<int>(sql, pnr).Single();
                    Assert.Greater(c2, 0);
                }
            }
        }

        [TestFixture]
        public class Process : DbrTestBase
        {
            [Test]
            [Timeout(10 * 1000)]
            public void Process_SamplePersons_Fast()
            {
                // Init DB & env
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "test user");
                CprBroker.Providers.CPRDirect.ExtractManager.ImportText(CprBroker.Tests.CPRDirect.Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE_FixedLength);

                var values = new Dictionary<string, string>();
                CprBroker.Engine.DataProviderConfigProperty.Templates.SetConnectionString(DbrDatabase.ConnectionString, values);

                Guid extractId;
                using (var partDataContext = new CprBroker.Providers.CPRDirect.ExtractDataContext())
                {
                    extractId = partDataContext.Extracts.Single().ExtractId;
                }

                using (var dbrDataContext = new System.Data.Linq.DataContext(DbrDatabase.ConnectionString))
                {
                    var sql = "select count(*) from DTTOTAL";
                    var c1 = dbrDataContext.ExecuteQuery<int>(sql).Single();
                    Assert.AreEqual(0, c1);

                    var dbrQueue = CprBroker.Engine.Queues.Queue.AddQueue<DbrQueue>(CprBroker.Providers.CPRDirect.DbrBaseQueue.TargetQueueTypeId, values, 100, 1);
                    var dbrQueueItems = PerPerson.PersonBaseTest.CprNumbers
                        .Select(pnr => new CprBroker.Providers.CPRDirect.ExtractQueueItem() { ExtractId = extractId, PNR = pnr })
                        .ToArray();
                    dbrQueue.Enqueue(dbrQueueItems);

                    dbrQueue.RunOneBatch();

                    var c2 = dbrDataContext.ExecuteQuery<int>(sql).Single();
                    Assert.AreEqual(PerPerson.PersonBaseTest.CprNumbers.Length, c2);
                }

                /*
***** CprBroker.Tests.DBR.DbrQueueTests.DbrQueueTests.Process_SamplePersons_Fast - WITH transaction
    .49 ms  Loading extract items    , at 15:01:58.4593
 154.42 ms  Loaded                   , at 15:01:58.6137
 678.87 ms  Items added              , at 15:01:59.2926
 102.52 ms  Deleted old records      , at 15:01:59.3951
 354.65 ms  Inserted new records     , at 15:01:59.7498
   6.65 ms  Commit completed         , at 15:01:59.7564


***** CprBroker.Tests.DBR.DbrQueueTests.DbrQueueTests.Process_SamplePersons_Fast - W/O transaction
    .00 Loading extract items    , at 15:09:03.0603
 123.73 Loaded                   , at 15:09:03.1840
 316.11 Items added              , at 15:09:03.5001
 127.51 Deleted old records      , at 15:09:03.6276
 231.62 Inserted new records     , at 15:09:03.8593
    .00 Commit completed         , at 15:09:03.8593

                */
            }
        }

        [TestFixture]
        public class ProcessTotals : DbrTestBase
        {
            Extract Extract = null;

            [SetUp]
            public void ImportExtract()
            {
                ExtractManager.ImportText(CprBroker.Tests.CPRDirect.Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE_FixedLength);
                using (var dataContext = new ExtractDataContext(CprDatabase.ConnectionString))
                {
                    Extract = dataContext.Extracts.Single();
                }
            }

            [Test]
            public void ProcessTotals_NotExisting_Inserted()
            {
                var queue = this.AddDbrQueue(false);
                var queueItem = new ExtractQueueItem() { ExtractId = Extract.ExtractId, PNR = "" };
                var ret = queue.ProcessTotals(new ExtractQueueItem[] { queueItem });
                Assert.IsNotEmpty(ret);
                using (var adminDataContext = new CprBroker.Providers.DPR.AdminDataContext(DbrDatabase.ConnectionString))
                {
                    var update = adminDataContext.Updates.SingleOrDefault();
                    Assert.NotNull(update);
                }
            }

            [Test]
            public void ProcessTotals_Existing_Updated()
            {
                var queue = this.AddDbrQueue(false);
                var queueItem = new ExtractQueueItem() { ExtractId = Extract.ExtractId, PNR = "" };
                var ret = queue.ProcessTotals(new ExtractQueueItem[] { queueItem });

                using (var adminDataContext = new CprBroker.Providers.DPR.AdminDataContext(DbrDatabase.ConnectionString))
                {
                    adminDataContext.ExecuteCommand("UPDATE DTAJOUR SET DPRAJDTO = DPRAJDTO - 1000");
                }

                // Process the item again
                ret = queue.ProcessTotals(new ExtractQueueItem[] { queueItem });

                using (var adminDataContext = new CprBroker.Providers.DPR.AdminDataContext(DbrDatabase.ConnectionString))
                {
                    var update = adminDataContext.Updates.SingleOrDefault();
                    Assert.AreEqual( Tests.CPRDirect.Utilities.PersonIndividualResponses.First().Value.StartRecord.ProductionDateDecimal, update.DPRAJDTO);
                }
            }
        }
    }
}
