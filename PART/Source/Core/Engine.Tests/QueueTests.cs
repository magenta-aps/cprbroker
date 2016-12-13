using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cprq = CprBroker.Engine.Queues;
using CprBroker.Engine.Queues;
using CprBroker.Data.Queues;

using NUnit.Framework;
using CprBroker.Engine;

namespace CprBroker.Tests.Data
{
    namespace QueueTests
    {
        public class QueueStub : cprq.Queue<QueueItemStub>
        {
            public QueueStub(Guid id)
                : base(id)
            { }

            public override QueueItemStub[] Process(QueueItemStub[] items)
            {
                return _Process(items);
            }

            public Func<QueueItemStub[], QueueItemStub[]> _Process = (items => items);
            public int Count()
            {
                using (var dataContext = new QueueDataContext())
                {
                    return dataContext.QueueItems.Where(qi => qi.QueueId == Impl.QueueId).Count();
                }
            }
        }

        public class QueueItemStub : QueueItemBase
        {
            public string Key = Guid.NewGuid().ToString();
            public override void DeserializeFromKey(string key)
            {
                this.Key = key;
            }
            public override string SerializeToKey()
            {
                return this.Key;
            }
        }

        [TestFixture]
        public class Run
        {
            Guid QueueId;
            [SetUp]
            public void CreateQueue()
            {
                BrokerContext.Current = null;
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
                using (var dataContext = new QueueDataContext())
                {
                    var dbQueue = new DbQueue() { QueueId = Guid.NewGuid(), BatchSize = 10, MaxRetry = 1, TypeName = "shdklahsfdkh" };
                    dataContext.Queues.InsertOnSubmit(dbQueue);
                    dataContext.SubmitChanges();
                    QueueId = dbQueue.QueueId;
                }
            }

            [TearDown]
            public void DeleteQueue()
            {
                using (var dataContext = new QueueDataContext())
                {
                    var dbQueue = dataContext.Queues.Single(q => q.QueueId == this.QueueId);
                    dataContext.Queues.DeleteOnSubmit(dbQueue);
                    dataContext.SubmitChanges();
                }
            }

            [Test]
            public void Run_NewQueue_OK()
            {
                var q = new QueueStub(QueueId);
                q.Enqueue(new QueueItemStub());

                q.RunAll();
            }

            [Test]
            public void Run_NewQueue_AllConsumed(
                [Values(0, 1, 2, 10, 100, 1000)] int count)
            {
                var q = new QueueStub(QueueId);
                var items = new List<QueueItemStub>();
                for (int i = 0; i < count; i++)
                {
                    items.Add(new QueueItemStub());
                }
                q.Enqueue(items.ToArray());

                Assert.AreEqual(count, q.Count());
                q.RunAll();
                Assert.AreEqual(0, q.Count());
            }
        }
    }
}
