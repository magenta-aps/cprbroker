using CprBroker.Engine;
using CprBroker.PartInterface.Tracking;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CprBroker.Tests.Tracking
{
    namespace CleanupQueueTests
    {
        [TestFixture]
        public class Process : PartInterface.TestBase
        {
            [SetUp]
            public void InitBrokerContext()
            {
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            }

            [Test]
            public void Process_RegistrationExists_Removed(
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
                var queue = new CleanupQueue();
                var succeeded = queue.Process(new CleanupQueueItem[] { new CleanupQueueItem() { removePersonItem = new Slet.RemovePersonItem(pId.UUID.Value, pId.CprNumber) } });
                Assert.AreEqual(1, succeeded.Length);
                Assert.AreEqual(countItems(pnr), 0);
                Assert.Greater(countItems(null), 0);
            }

            class CleaunupQueueStub : CleanupQueue
            {

                public Func<CleanupQueueItem, CleanupQueueItem> _ProcessItem = null;
                public override CleanupQueueItem ProcessItem(BrokerContext brokerContext, TrackingDataProvider prov, CleanupQueueItem queueItem, DateTime fromDate, DateTime dbrFromDate, int[] excludedMunicipalityCodes)
                {
                    if (_ProcessItem == null)
                        return base.ProcessItem(brokerContext, prov, queueItem, fromDate, dbrFromDate, excludedMunicipalityCodes);
                    else
                        return _ProcessItem(queueItem);
                }

                public Func<CleanupQueueItem, CleanupQueueItem> _ProcessItemWithMutex = null;
                public override CleanupQueueItem ProcessItemWithMutex(BrokerContext brokerContext, TrackingDataProvider prov, CleanupQueueItem queueItem)
                {
                    if (_ProcessItemWithMutex == null)
                        return base.ProcessItemWithMutex(brokerContext, prov, queueItem);
                    else
                        return _ProcessItemWithMutex(queueItem);
                }
            }

            [Test]
            public void Process_SameKey_Waits(
                [Values(2, 4, 10)]int c,
                [Values(100, 500, 350)]int waitMilliseconds)
            {
                int calls = 0;
                var queue = new CleaunupQueueStub()
                {
                    _ProcessItem = (qi) =>
                    {
                        Thread.Sleep(waitMilliseconds);
                        Interlocked.Increment(ref calls);
                        return qi;
                    }
                };
                var uuid = Guid.NewGuid();
                var pnr = Tests.PartInterface.Utilities.RandomCprNumber();
                var items = Enumerable.Range(0, c).Select(i => new CleanupQueueItem() { removePersonItem = new Slet.RemovePersonItem(uuid, pnr) }).ToArray();
                var start = DateTime.Now;
                queue.Process(items);
                var end = DateTime.Now;
                var spent = end - start;
                var expected = TimeSpan.FromMilliseconds(waitMilliseconds * c);
                Assert.AreEqual(c, calls);
                Assert.GreaterOrEqual(spent, expected);
                Assert.LessOrEqual(spent, expected + TimeSpan.FromMilliseconds(1000));
            }

            [Test]
            public void Process_NewKey_NoWaits(
                [Values(2, 4, 10)]int c,
                [Values(100, 500, 350)]int waitMilliseconds)
            {
                int calls = 0;
                var queue = new CleaunupQueueStub()
                {
                    _ProcessItem = (qi) =>
                    {
                        Thread.Sleep(waitMilliseconds);
                        Interlocked.Increment(ref calls);
                        return qi;
                    }
                };
                var items = Enumerable.Range(0, c).Select(i => new CleanupQueueItem() { removePersonItem = new Slet.RemovePersonItem(Guid.NewGuid(), PartInterface.Utilities.RandomCprNumber()) }).ToArray();
                var start = DateTime.Now;
                queue.Process(items);
                var end = DateTime.Now;
                var spent = end - start;
                var expected = TimeSpan.FromMilliseconds(waitMilliseconds);
                Assert.AreEqual(c, calls);
                Assert.GreaterOrEqual(spent, expected);
                Assert.LessOrEqual(spent, expected + TimeSpan.FromMilliseconds(1000));
            }
        }
    }
}
