﻿using System;
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
    namespace TrackingDataProviderTests
    {
        [TestFixture]
        public class GetPersonUsageTests_EmptyDatabase : PartInterface.TestBase
        {
            [Test]
            public void GetPersonUsageTests_EmptyDB_NonePerPerson([Values(1, 20, 15, 27, 100)]int count)
            {
                var prov = new TrackingDataProvider();
                var uuids = Enumerable.Range(1, count).Select(i => Guid.NewGuid()).ToArray();
                var ret = prov.GetPersonUsage(uuids, null, null);
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
        public class GetPersonUsageTests_PersonsAvailable : PartInterface.TestBase
        {
            Guid[] InsertedUuids;

            [SetUp]
            public void InsertPersons()
            {
                InsertedUuids = Utilities.InsertPersons(CprDatabase.ConnectionString, 200);
            }

            [Test]
            public void GetPersonUsageTests_RecordsNotMatching_NonePerPerson([Values(1, 20, 15, 27, 100)]int count)
            {
                var uuids = Enumerable.Range(1, count).Select(i => Guid.NewGuid()).ToArray();

                var prov = new TrackingDataProvider();
                var ret = prov.GetPersonUsage(uuids, null, null);
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
            public void GetPersonUsageTests_RecordsMatchingNoUse_NonePerPerson([Values(1, 20, 15, 27, 100)]int count)
            {
                var uuids = InsertedUuids;

                var prov = new TrackingDataProvider();
                var ret = prov.GetPersonUsage(uuids, null, null);
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
            public void GetPersonUsageTests_RecordsMatchingIrrelevantUsage_NonePerPerson([Values(1, 20, 15, 27, 100)]int count)
            {
                var uuids = InsertedUuids;
                Utilities.InitBrokerContext();
                Utilities.AddUsage(CprDatabase.ConnectionString, uuids, OperationType.Types.Generic, OperationType.Types.Search);
                var prov = new TrackingDataProvider();
                var ret = prov.GetPersonUsage(uuids, null, null);
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
            public void GetPersonUsageTests_RecordsMatchingWithUse_UsageFound([Values(1, 20, 15, 27, 100)]int count)
            {
                var uuids = InsertedUuids;
                Utilities.InitBrokerContext();
                Utilities.AddUsage(CprDatabase.ConnectionString, uuids, OperationType.Types.Read);
                var prov = new TrackingDataProvider();
                var ret = prov.GetPersonUsage(uuids, null, null);
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
