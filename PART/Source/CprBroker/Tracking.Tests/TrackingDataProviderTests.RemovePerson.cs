using CprBroker.Engine;
using CprBroker.PartInterface.Tracking;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using CprBroker.Schemas;
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
        public class RemovePerson : PartInterface.TestBase
        {
            [SetUp]
            public void InitBrokerContext()
            {
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            }

            [Test]
            public void RemovePerson_EmptyDB_Passes()
            {
                var prov = new TrackingDataProvider();
                prov.RemovePerson(new Schemas.PersonIdentifier() { CprNumber = CprBroker.Tests.PartInterface.Utilities.RandomCprNumber(), UUID = Guid.NewGuid() });
            }

            [Test]
            public void RemovePerson_ExtractExists_Removed(
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
                var prov = new TrackingDataProvider();
                prov.RemovePerson(pId);
                Assert.AreEqual(countItems(pnr), 0);
                Assert.Greater(countItems(null), 0);
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
                var prov = new TrackingDataProvider();
                prov.RemovePerson(pId);
                Assert.AreEqual(countItems(pnr), 0);
                Assert.Greater(countItems(null), 0);
            }

            class DP : IPutSubscriptionDataProvider
            {
                public static bool _IsSharingSubscriptions { get; set; }
                public bool IsSharingSubscriptions { get { return _IsSharingSubscriptions; } set { _IsSharingSubscriptions = value; } }
                public Version Version { get { throw new NotImplementedException(); } }
                public bool IsAlive() { throw new NotImplementedException(); }
                public bool PutSubscription(PersonIdentifier personIdentifier) { throw new NotImplementedException(); }
                public static bool RemoveSubscriptionCalled = false;
                public bool RemoveSubscription(PersonIdentifier personIdentifier)
                {
                    RemoveSubscriptionCalled = true;
                    return true;
                }
            }

            [Test]
            public void RemovePerson_SharedSubscription_SubscriptionNotRemoved(
                [Values(6, 18, 35, 67, 70, 77)]int pnrIndex)
            {
                var pnr = CPRDirect.Utilities.PNRs[pnrIndex];
                DP.RemoveSubscriptionCalled = false;
                DP._IsSharingSubscriptions = true;
                RegisterDataProviderType<DP>();
                var pId = new PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() };
                var prov = new TrackingDataProvider();
                prov.RemovePerson(pId);
                Assert.False(DP.RemoveSubscriptionCalled);
            }

            [Test]
            public void RemovePerson_UnSharedSubscription_SubscriptionRemoved(
                [Values(6, 18, 35, 67, 70, 77)]int pnrIndex)
            {
                var pnr = CPRDirect.Utilities.PNRs[pnrIndex];
                DP.RemoveSubscriptionCalled = false;
                DP._IsSharingSubscriptions = false;
                RegisterDataProviderType<DP>();
                var pId = new PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() };
                var prov = new TrackingDataProvider();
                prov.RemovePerson(pId);
                Assert.True(DP.RemoveSubscriptionCalled);
            }
        }

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
                var prov = new TrackingDataProvider();

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
