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
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void T100_Dequeue(int maxCount)
        {
            var result = TestRunner.EventsService.DequeueDataChangeEvents(maxCount);
            Assert.IsNotNull(result);
            Assert.LessOrEqual(result.Length, maxCount);
            foreach (var ev in result)
            {
                Assert.IsNotNull(ev);
                Assert.AreNotEqual(Guid.Empty, ev.EventId);
                Assert.AreNotEqual(Guid.Empty, ev.PersonUuid);
            }
        }
    }
}
