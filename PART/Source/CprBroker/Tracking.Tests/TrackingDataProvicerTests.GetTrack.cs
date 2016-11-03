using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CprBroker.PartInterface.Tracking;
using CprBroker.Data.Part;
using System.Data.SqlClient;
using CprBroker.Data.Applications;
using CprBroker.Engine;

namespace CprBroker.Tests.Tracking
{
    namespace TrackingDataProvicerTests
    {
        [TestFixture]
        public class GetTrackTests_EmptyDatabase : PartInterface.TestBase
        {
            [Test]
            public void GetTrackTests_EmptyDB_NonePerPerson([Values(1, 20, 15, 27, 100)]int count)
            {
                var prov = new TrackingDataProvider();
                var uuids = Enumerable.Range(1, count).Select(i => Guid.NewGuid()).ToArray();
                var ret = prov.GetTrack(uuids, null, null);
                Assert.AreEqual(uuids.Length, ret.Length);
                foreach (var r in uuids.Zip(ret, (a, b) => new { a, b }))
                {
                    Assert.AreEqual(r.a, r.b.UUID);
                    Assert.IsEmpty(r.b.ReadOperations);
                    Assert.Null(r.b.Subscribers);
                    Assert.Null(r.b.LastRead);
                }
            }
        }

        [TestFixture]
        public class GetTrackTests_PersonsAvailable : PartInterface.TestBase
        {
            [SetUp]
            public void InsertPersons()
            {
                Utilities.InsertPersons(CprDatabase.ConnectionString, 200);
            }

            [Test]
            public void GetTrackTests_RecordsNotMatching_NonePerPerson([Values(1, 20, 15, 27, 100)]int count)
            {
                var uuids = Enumerable.Range(1, count).Select(i => Guid.NewGuid()).ToArray();

                var prov = new TrackingDataProvider();
                var ret = prov.GetTrack(uuids, null, null);
                Assert.AreEqual(uuids.Length, ret.Length);
                foreach (var r in uuids.Zip(ret, (a, b) => new { a, b }))
                {
                    Assert.AreEqual(r.a, r.b.UUID);
                    Assert.IsEmpty(r.b.ReadOperations);
                    Assert.Null(r.b.Subscribers);
                    Assert.Null(r.b.LastRead);
                }
            }

            [Test]
            public void GetTrackTests_RecordsMatchingNoUse_NonePerPerson([Values(1, 20, 15, 27, 100)]int count)
            {
                var uuids = Utilities.newUuids;

                var prov = new TrackingDataProvider();
                var ret = prov.GetTrack(uuids, null, null);
                Assert.AreEqual(uuids.Length, ret.Length);
                foreach (var r in uuids.Zip(ret, (a, b) => new { a, b }))
                {
                    Assert.AreEqual(r.a, r.b.UUID);
                    Assert.IsEmpty(r.b.ReadOperations);
                    Assert.Null(r.b.Subscribers);
                    Assert.Null(r.b.LastRead);
                }
            }

            [Test]
            public void GetTrackTests_RecordsMatchingIrrelevantUsage_NonePerPerson([Values(1, 20, 15, 27, 100)]int count)
            {
                var uuids = Utilities.newUuids;
                Utilities.InitBrokerContext();
                Utilities.AddUsage(CprDatabase.ConnectionString, uuids, OperationType.Types.Generic, OperationType.Types.Search);
                var prov = new TrackingDataProvider();
                var ret = prov.GetTrack(uuids, null, null);
                Assert.AreEqual(uuids.Length, ret.Length);
                foreach (var r in uuids.Zip(ret, (a, b) => new { a, b }))
                {
                    Assert.AreEqual(r.a, r.b.UUID);
                    Assert.IsEmpty(r.b.ReadOperations);
                    Assert.Null(r.b.Subscribers);
                    Assert.Null(r.b.LastRead);
                }
            }

            [Test]
            public void GetTrackTests_RecordsMatchingWithUse_UsageFound([Values(1, 20, 15, 27, 100)]int count)
            {
                var uuids = Utilities.newUuids;
                Utilities.InitBrokerContext();
                Utilities.AddUsage(CprDatabase.ConnectionString, uuids, OperationType.Types.Read);
                var prov = new TrackingDataProvider();
                var ret = prov.GetTrack(uuids, null, null);
                Assert.AreEqual(uuids.Length, ret.Length);
                foreach (var r in uuids.Zip(ret, (a, b) => new { a, b }))
                {
                    Assert.AreEqual(r.a, r.b.UUID);
                    Assert.AreEqual(1, r.b.ReadOperations.Length);
                    Assert.NotNull(r.b.LastRead);
                }
            }
        }
    }
}
