using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
//using CprBroker.NUnitTester.Events ;

namespace CprBroker.NUnitTester
{
    [TestFixture]
    public class EventTest : BaseTest
    {
        public void Validate(Events.StandardReturType ret)
        {
            Assert.IsNotNull(ret);
            base.Validate(ret.StatusKode, ret.FejlbeskedTekst);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void T100_Dequeue(int maxCount)
        {
            var result = TestRunner.EventsService.DequeueDataChangeEvents(TestRunner.EventsApplicationHeader, maxCount);
            Assert.IsNotNull(result);
            Validate(result.StandardRetur);
            Assert.IsNotNull(result.Item);
            Assert.LessOrEqual(result.Item.Length, maxCount);
            foreach (var ev in result.Item)
            {
                Assert.IsNotNull(ev);
                Assert.AreNotEqual(Guid.Empty, ev.EventId);
                Assert.AreNotEqual(Guid.Empty, ev.PersonUuid);
            }
        }
    }
}
