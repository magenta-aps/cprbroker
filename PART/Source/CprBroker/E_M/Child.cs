using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using NUnit.Framework;

namespace CprBroker.Providers.E_M
{
    public partial class Child
    {
        public static PersonFlerRelationType ToPersonFlerRelationType(Child child, Func<string, Guid> cpr2uuidFunc)
        {
            if (child != null)
            {
                return PersonFlerRelationType.Create(cpr2uuidFunc(Converters.ToCprNumber(child.PNR)), null, null);
            }
            return null;
        }

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
}
