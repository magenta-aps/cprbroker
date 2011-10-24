using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.E_M;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.E_M
{
    [TestFixture]
    public class Tests
    {
        private Dictionary<string, Guid> uuids = new Dictionary<string, Guid>();
        [TearDown]
        public void ClearUUIDList()
        {
            uuids.Clear();
        }
        Guid ToUuid(string cprNumber)
        {
            var ret = Guid.NewGuid();
            uuids[cprNumber] = ret;
            return ret;
        }
        decimal?[] TestCprNumbers
        {
            get { return UnitTests.RandomCprNumbers(10); }
        }

        [Test]
        [TestCaseSource("TestCprNumbers")]
        public void TestToPersonFlerRelationType(decimal? cprNumber)
        {
            var result = Child.ToPersonFlerRelationType(new Child() { PNR = cprNumber }, ToUuid);
            var stringCprNumber = Converters.ToCprNumber(cprNumber);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ReferenceID);
            Assert.IsTrue(uuids.ContainsKey(stringCprNumber));
            Assert.AreEqual(uuids[stringCprNumber].ToString(), result.ReferenceID.Item);
        }

    }
}
