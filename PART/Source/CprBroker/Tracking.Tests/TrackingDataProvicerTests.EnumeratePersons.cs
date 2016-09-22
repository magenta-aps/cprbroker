using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CprBroker.PartInterface.Tracking;
using CprBroker.Data.Part;
using System.Data.SqlClient;

namespace CprBroker.Tests.Tracking
{
    namespace TrackingDataProvicerTests
    {
        [TestFixture]
        public class EnumeratePersons_EmptyDB : PartInterface.TestBase
        {
            [Test]
            public void EnumeratePersons_EmptyDB_None(
                [Values(0, 10, 100, 1000)] int startIndex,
                [Values(0, 10, 100, 200, 1000)] int maxCount
                )
            {
                var prov = new TrackingDataProvider();
                var uuids = prov.EnumeratePersons(startIndex, maxCount);
                Assert.IsEmpty(uuids);
            }
        }

        [TestFixture]
        public class EnumeratePersons_PersonsAvailable : PartInterface.TestBase
        {
            [SetUp]
            public void InsertPersons()
            {
                Utilities.InsertPersons(CprDatabase.ConnectionString, 1000);
                // Database warm-up
                var prov = new TrackingDataProvider();
                var uuids = prov.EnumeratePersons(1, 1);
            }
            [Test]
            [Sequential]
            public void EnumeratePersons_PersonsAvailable_500ms(
                [Values(1, 10, 100)] int startIndex,
                [Values(1, 10, 100)] int maxCount
                )
            {
                DateTime start = DateTime.Now;
                var prov = new TrackingDataProvider();
                var uuids = prov.EnumeratePersons(startIndex, maxCount);
                Assert.LessOrEqual((DateTime.Now - start).TotalMilliseconds, 500, "Timeout 500 ms exceeded");
            }

            [Test]
            [Sequential]
            public void EnumeratePersons_PersonsAvailable_CorrectCount(
                [Values(1, 10, 100)] int startIndex,
                [Values(1, 10, 100)] int maxCount
                )
            {
                var prov = new TrackingDataProvider();
                var uuids = prov.EnumeratePersons(startIndex, maxCount);
                Assert.AreEqual(maxCount, uuids.Length);
            }

            [Test]
            [Combinatorial]
            public void EnumeratePersons_PersonsAvailable_CorrectUuids(
                [Values(1, 10, 100)] int startIndex,
                [Values(1, 10, 100)] int maxCount
                )
            {
                var prov = new TrackingDataProvider();
                var uuids = prov.EnumeratePersons(startIndex, maxCount);
                var expectedUuids = Utilities.newUuids
                    .Select(id => id.ToString().Split('-'))
                    .OrderBy(id => id[4])
                    .ThenBy(id => id[3])
                    .Select(id => string.Join("-", id))
                    .Select(id => new Guid(id))
                    .Skip(startIndex).Take(maxCount);
                foreach (var id in expectedUuids.Zip(uuids, (a, b) => new { a, b }))
                {
                    Assert.AreEqual(id.a, id.b);
                }
            }
        }
        
    }
}
