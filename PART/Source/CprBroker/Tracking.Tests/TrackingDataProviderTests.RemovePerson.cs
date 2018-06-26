using CprBroker.Engine;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using CprBroker.Schemas;
using CprBroker.Slet;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.Tracking
{
    namespace TrackingDataProviderTests
    {
        [TestFixture]
        public class TrackingDataProviderTests2 : PartInterface.TestBase
        {
            DatabaseInfo _DbrDatabase;

            public override void CreateDatabases()
            {
                base.CreateDatabases();

                _DbrDatabase = CreateDatabase("DBR_Test_",
                    CprBroker.DBR.Properties.Resources.CreateDbrTables,
                    new KeyValuePair<string, string>[] { }
                );
            }

            [Test]
            public void RemovePerson_DBRExists_Removed(
               [Values(12, 38, 52, 68, 72)]int pnrIndex)
            {
                var pnr = CPRDirect.Utilities.PNRs[pnrIndex];

                using (var dc = new DPRDataContext(_DbrDatabase.ConnectionString))
                {
                    var person = CprBroker.Tests.CPRDirect.Persons.Person.GetPerson(pnr);
                    CprBroker.DBR.CprConverter.AppendPerson(person, dc);
                    dc.SubmitChanges();
                }

                Func<int> countItems = () =>
                {
                    using (var dc = new DPRDataContext(_DbrDatabase.ConnectionString))
                    {
                        return dc.PersonTotals.Where(t => t.PNR == decimal.Parse(pnr)).Count();
                    }
                };
                Assert.AreEqual(1, countItems());
                var pId = new PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() };
                var prov = new RemovePersonDataProvider();

                var queue = new DBR.DbrQueue()
                {
                    ConfigurationProperties = new Dictionary<string, string>(),
                    ConnectionString = _DbrDatabase.ConnectionString
                };
                CprBroker.Engine.Queues.Queue.AddQueue<DBR.DbrQueue>(DbrBaseQueue.TargetQueueTypeId, queue.ConfigurationProperties, 1, 1);
                prov.RemovePerson(pId);
                Assert.AreEqual(0, countItems());
            }
        }
    }
}
