using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.DBR;

namespace CprBroker.Tests.DBR
{
    namespace QueueTests
    {
        [TestFixture]
        public class DbrQueueTests : DbrTestBase
        {
            [Test]
            public void RunOneBatch_GoesToDbrDatabase([ValueSource(typeof(PerPerson.PersonBaseTest), "CprNumbers")] string pnr)
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

                using (var dataContext = new System.Data.Linq.DataContext(DbrDatabase.ConnectionString))
                {
                    var sql = "select count(*) from DTTOTAL WHERE PNR = {0}";
                    var c1 = dataContext.ExecuteQuery<int>(sql, pnr).Single();
                    Assert.AreEqual(0, c1);

                    var dbrQueue = CprBroker.Engine.Queues.Queue.AddQueue<DbrQueue>(CprBroker.Providers.CPRDirect.DbrBaseQueue.TargetQueueTypeId, values, 100, 1);
                    var dbrQueueItem = new CprBroker.Providers.CPRDirect.ExtractQueueItem() { ExtractId = extractId, PNR = pnr };
                    dbrQueue.Enqueue(dbrQueueItem);

                    dbrQueue.RunOneBatch();

                    var c2 = dataContext.ExecuteQuery<int>(sql, pnr).Single();
                    Assert.Greater(c2, 0);
                }
            }
        }
    }
}
