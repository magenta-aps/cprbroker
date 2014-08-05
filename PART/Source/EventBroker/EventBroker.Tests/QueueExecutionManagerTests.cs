using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.EventBroker.Notifications;

namespace CprBroker.EventBroker.Tests
{
    namespace QueueExecutionManagerTests
    {
        [TestFixture]
        public class SyncTasks : CprBroker.Tests.PartInterface.TestBase
        {
            public class QueueItemStub : CprBroker.Engine.Queues.QueueItemBase
            {
                public override void DeserializeFromKey(string key)
                {
                    throw new NotImplementedException();
                }
                public override string SerializeToKey()
                {
                    throw new NotImplementedException();
                }
            }

            public class QueueStub : CprBroker.Engine.Queues.Queue<QueueItemStub>
            {
                public override QueueItemStub[] Process(QueueItemStub[] items)
                {
                    throw new NotImplementedException();
                }
            }

            [SetUp]
            public void InitApplication()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "Test user");
            }

            [Test]            
            public void SyncTasks_DefaultQueues_SomeTasks(
                [Range(1,100)]int passNo)
            {
                using (var manager = new QueueExecutionManager())
                {
                    manager.SyncTasks();
                    var tasks = manager.GetCurrentTaskExecuters();                    
                    Assert.IsNotEmpty(tasks);
                }
            }

            [Test]
            public void SyncTasks_OneQueue_OneMoreTask(
                [Range(1,100)]int passNo)
            {

                using (var manager = new QueueExecutionManager())
                {
                    manager.SyncTasks();
                    var c1 = manager.GetCurrentTaskExecuters().Length;

                    CprBroker.Engine.Queues.Queue.AddQueue<QueueStub>(1, new Dictionary<string, string>(), 1, 1);
                    manager.SyncTasks();
                    var c2 = manager.GetCurrentTaskExecuters().Length;
                    
                    Assert.AreEqual(c1 + 1, c2);
                }
            }
        }
    }
}
