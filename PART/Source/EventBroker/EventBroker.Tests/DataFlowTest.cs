using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.EventBroker.Data;
using CprBroker.EventBroker.Backend;
using System.Threading;
using CprBroker.Tests.PartInterface;

namespace CprBroker.EventBroker.Tests
{
    [TestFixture]
    public class DataFlowTest : TestBase
    {
        [Test]
        public void DataChange_SubForAll_Enqueued()
        {
            using (var dataContext = new EventBrokerDataContext(ConnectionString))
            {
                var sub = AddSubscription(dataContext, null, true, true, SubscriptionType.SubscriptionTypes.DataChange);
                var change = AddChanges(dataContext, 1);
                dataContext.SubmitChanges();
            }
            var backEnd = new BackendService();
            backEnd.StartQueues();

            Thread.Sleep(1000);
            using (var dataContext = new EventBrokerDataContext(ConnectionString))
            {
                var notif = dataContext.EventNotifications.SingleOrDefault();
                Assert.NotNull(notif);
            }
        }
    }
}
