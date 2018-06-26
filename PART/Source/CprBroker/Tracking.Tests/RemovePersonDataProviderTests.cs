using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CprBroker.PartInterface.Tracking;
using CprBroker.Slet;
using CprBroker.Engine;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas;
using CprBroker.EventBroker.Data;
using System.Data.SqlClient;

namespace CprBroker.Tests.Tracking
{
    namespace RemovePersonDataProviderTests
    {
        [TestFixture]
        public class RemovePersonTests : PartInterface.TestBase
        {
            [SetUp]
            public void InitBrokerContext()
            {
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            }

            [Test]
            public void RemovePerson_EmptyDB_Passes()
            {
                var prov = new RemovePersonDataProvider();
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
                var prov = new RemovePersonDataProvider();
                prov.RemovePerson(pId);
                Assert.AreEqual(0, countItems(pnr));
                Assert.Greater(countItems(null), 0);
            }

            [Test]
            public void EnqueuePersonForRemoval([Values(1, 20, 15, 27, 100)]int count)
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(CprDatabase.ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("If NOT EXISTS (SELECT * FROM dbo.[Queue] WHERE TypeName like 'CprBroker.PartInterface.Tracking.CleanupQueue%') INSERT INTO dbo.[Queue](TypeId, TypeName, BatchSize, MaxRetry) VALUES(400, 'CprBroker.PartInterface.Tracking.CleanupQueue, CprBroker.PartInterface.Tracking', 100, 100)", conn);
                    cmd.ExecuteNonQuery();
                }

                var prov = new RemovePersonDataProvider();
                var uuids = Enumerable.Range(1, count).Select(i => Guid.NewGuid()).ToArray();

                foreach (Guid uuid in uuids)
                {
                    Assert.True(prov.EnqueuePersonForRemoval(new Schemas.PersonIdentifier() { CprNumber = CprBroker.Tests.PartInterface.Utilities.RandomCprNumber(), UUID = uuid }));
                }

                using (var conn = new System.Data.SqlClient.SqlConnection(CprDatabase.ConnectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM dbo.[Queue] WHERE TypeName like 'CprBroker.PartInterface.Tracking.CleanupQueue%'", conn);
                    cmd.ExecuteNonQuery();
                }
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
                var prov = new RemovePersonDataProvider();
                prov.RemovePerson(pId);
                Assert.AreEqual(0, countItems(pnr));
                Assert.Greater(countItems(null), 0);
            }

            [Test]
            public void RemovePerson_SubscriptionPersonExists_Removed(
                [Values(EventBroker.Data.SubscriptionType.SubscriptionTypes.Birthdate, EventBroker.Data.SubscriptionType.SubscriptionTypes.DataChange)] EventBroker.Data.SubscriptionType.SubscriptionTypes type,
                [Values(true, false)]bool forAll,
                [Values(true, false)]bool isReady
                )
            {
                var pnr = PartInterface.Utilities.RandomCprNumber();
                var uuid = Guid.NewGuid();
                Func<Guid?, int> countItems = (id) =>
                {
                    using (var dc = new EventBrokerDataContext())
                    {
                        if (id.HasValue)
                            return dc.SubscriptionPersons.Where(ei => ei.PersonUuid == id).Count();
                        else
                            return dc.SubscriptionPersons.Count();
                    }
                };

                using (var dataContext = new EventBrokerDataContext())
                {
                    var sub = AddSubscription(dataContext, new Schemas.Part.SoegObjektType(), forAll: forAll, ready: isReady, type: type);
                    sub.SubscriptionPersons.Add(new SubscriptionPerson() { Created = DateTime.Now, PersonUuid = uuid, Removed = DateTime.Now, SubscriptionId = sub.SubscriptionId, SubscriptionPersonId = Guid.NewGuid() });
                    dataContext.SubscriptionPersons.InsertAllOnSubmit(sub.SubscriptionPersons);
                    dataContext.SubmitChanges();
                }
                Assert.Greater(countItems(null), 0);
                Assert.Greater(countItems(uuid), 0);
                var pId = new PersonIdentifier() { CprNumber = pnr, UUID = uuid };
                var prov = new RemovePersonDataProvider();
                prov.RemovePerson(pId);
                Assert.AreEqual(0, countItems(uuid));
            }

            class IPutSubscriptionDataProviderStub : IPutSubscriptionDataProvider
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
                IPutSubscriptionDataProviderStub.RemoveSubscriptionCalled = false;
                IPutSubscriptionDataProviderStub._IsSharingSubscriptions = true;
                RegisterDataProviderType<IPutSubscriptionDataProviderStub>();
                var pId = new PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() };
                var prov = new RemovePersonDataProvider();
                prov.RemovePerson(pId);
                Assert.False(IPutSubscriptionDataProviderStub.RemoveSubscriptionCalled);
            }

            [Test]
            public void RemovePerson_UnSharedSubscription_SubscriptionRemoved(
                [Values(6, 18, 35, 67, 70, 77)]int pnrIndex)
            {
                var pnr = CPRDirect.Utilities.PNRs[pnrIndex];
                IPutSubscriptionDataProviderStub.RemoveSubscriptionCalled = false;
                IPutSubscriptionDataProviderStub._IsSharingSubscriptions = false;
                RegisterDataProviderType<IPutSubscriptionDataProviderStub>();
                var pId = new PersonIdentifier() { CprNumber = pnr, UUID = Guid.NewGuid() };
                var prov = new RemovePersonDataProvider();
                prov.RemovePerson(pId);
                Assert.True(IPutSubscriptionDataProviderStub.RemoveSubscriptionCalled);
            }
        }
    }
}
