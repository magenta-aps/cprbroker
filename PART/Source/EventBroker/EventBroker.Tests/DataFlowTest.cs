using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.EventBroker.Data;
using CprBroker.EventBroker.Backend;
using System.Threading;
using CprBroker.Tests.PartInterface;

namespace CprBroker.Tests.EventBroker
{
    [TestFixture]
    public class DataFlowTest : TestBase
    {
        [TestFixtureSetUp]
        public void InitBackendTasks()
        {
            base.InitBackendTasks(TimeSpan.FromMilliseconds(100));
        }

        [Test]
        [Ignore("Too big for integration tests")]
        public void DataChange_SubscriptionForAll_Sent()
        {
            using (var dataContext = new EventBrokerDataContext(EventDatabase.ConnectionString))
            {
                var sub = AddSubscription(dataContext, null, true, true, SubscriptionType.SubscriptionTypes.DataChange);
                var change = AddChanges(dataContext, 1);
                dataContext.SubmitChanges();
            }
            using (var backEnd = new BackendService())
            {
                backEnd.StartTasks();

                Thread.Sleep(3000);
            }

            using (var dataContext = new EventBrokerDataContext(EventDatabase.ConnectionString))
            {
                var notif = dataContext.EventNotifications.SingleOrDefault();
                Assert.NotNull(notif);
            }
        }
    }
}
