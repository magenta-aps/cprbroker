using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.DBR;

namespace CprBroker.Tests.DBR
{
    public class DbrTestBase : CprBroker.Tests.PartInterface.TestBase
    {
        public DatabaseInfo DbrDatabase;
        public Random Random = new Random();

        [SetUp]
        public void InitApplication()
        {
            CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "Test user");
        }

        public DbrQueue AddDbrQueue(bool addPort)
        {
            var dic = new Dictionary<string, string>();
            if (addPort)
                dic["Port"] = Random.Next(1000, 10000).ToString();
            return CprBroker.Engine.Queues.Queue.AddQueue<DbrQueue>(1, dic, 1, 1);
        }

        public CprBroker.Providers.DPR.DprDatabaseDataProvider CreateDprProvider(DbrQueue q)
        {
            var dpr = new CprBroker.Providers.DPR.DprDatabaseDataProvider();
            dpr.ConfigurationProperties = new Dictionary<string, string>();
            dpr.ConfigurationProperties["Port"] = q.Port.ToString();
            dpr.ConfigurationProperties["Address"] = "localhost";
            dpr.ConfigurationProperties["TCP Read Timeout (ms)"] = "1000";
            return dpr;
        }

        public override void CreateDatabases()
        {
            base.CreateDatabases();
            DbrDatabase = CreateDatabase("DBR_", Properties.Resources.CreateDbr, new KeyValuePair<string, string>[] { });
        }

    }
}
