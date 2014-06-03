using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.EventBroker;
using CprBroker.EventBroker.Data;
using System.IO;
using System.Data.SqlClient;

namespace CprBroker.EventBroker.Tests
{
    namespace EnqueueDataChangeEventNotifications
    {
        [TestFixture]
        public class ForAllPersons : TestBase
        {

            [Test]
            public void EnqueueDataChangeEventNotifications_LatestReceivedOrder_OneNotif([Range(2, 10)]int changeCount)
            {
                using (var dataContext = new EventBrokerDataContext(ConnectionString))
                {
                    var sub = AddSubscription(dataContext, null, true, true, SubscriptionType.SubscriptionTypes.DataChange);
                    var changes = AddChanges(dataContext, changeCount);
                    dataContext.SubmitChanges();

                    var minOrder = dataContext.DataChangeEvents.Select(dce => dce.ReceivedOrder).Min();
                    // Now call SP
                    dataContext.EnqueueDataChangeEventNotifications(DateTime.Now, minOrder, sub.SubscriptionTypeId);

                    // Validate
                    var eventsCount = dataContext.EventNotifications.Count();
                    Assert.AreEqual(1, eventsCount);
                }
            }

            [Test]
            public void EnqueueDataChangeEventNotifications_UnreadySubscription_Zero([Range(2, 10)]int changeCount)
            {
                using (var dataContext = new EventBrokerDataContext(ConnectionString))
                {
                    var sub = AddSubscription(dataContext, null, true, false, SubscriptionType.SubscriptionTypes.DataChange);
                    var changes = AddChanges(dataContext, changeCount);
                    dataContext.SubmitChanges();

                    // Now call SP
                    dataContext.EnqueueDataChangeEventNotifications(DateTime.Now, int.MaxValue, sub.SubscriptionTypeId);

                    // Validate
                    var eventsCount = dataContext.EventNotifications.Count();
                    Assert.AreEqual(0, eventsCount);
                }
            }

            [Test]
            public void EnqueueDataChangeEventNotifications_DeactivatedSubscription_Zero([Range(2, 10)]int changeCount)
            {
                using (var dataContext = new EventBrokerDataContext(ConnectionString))
                {
                    var sub = AddSubscription(dataContext, null, true, true, SubscriptionType.SubscriptionTypes.DataChange);
                    sub.Deactivated = DateTime.Today;
                    var changes = AddChanges(dataContext, changeCount);
                    dataContext.SubmitChanges();

                    // Now call SP
                    dataContext.EnqueueDataChangeEventNotifications(DateTime.Now, int.MaxValue, sub.SubscriptionTypeId);

                    // Validate
                    var eventsCount = dataContext.EventNotifications.Count();
                    Assert.AreEqual(0, eventsCount);
                }
            }

            [Test]
            public void EnqueueDataChangeEventNotifications_MismatchedSubscriptionType_Zero([Range(2, 10)]int changeCount)
            {
                using (var dataContext = new EventBrokerDataContext(ConnectionString))
                {
                    var sub = AddSubscription(dataContext, null, true, true, SubscriptionType.SubscriptionTypes.DataChange);
                    var changes = AddChanges(dataContext, changeCount);
                    dataContext.SubmitChanges();

                    // Now call SP
                    dataContext.EnqueueDataChangeEventNotifications(DateTime.Now, int.MaxValue, sub.SubscriptionTypeId + 55);

                    // Validate
                    var eventsCount = dataContext.EventNotifications.Count();
                    Assert.AreEqual(0, eventsCount);
                }
            }

        }

        [TestFixture]
        public class SpecificPersons : TestBase
        {
            [Test]
            public void MatchingUuids_OnePushed(
                [Random(1, 100, 3)] int personCount,
                [Random(1, 100, 3)] int changeCount
                )
            {
                using (var dataContext = new EventBrokerDataContext(ConnectionString))
                {
                    var sub = AddSubscription(dataContext, null, false, true, SubscriptionType.SubscriptionTypes.DataChange);
                    var persons = AddPersons(sub, personCount);
                    var changes = AddChanges(dataContext, changeCount);
                    changes[0].PersonUuid = persons[0].PersonUuid.Value;
                    dataContext.SubmitChanges();


                    dataContext.EnqueueDataChangeEventNotifications(DateTime.Now, int.MaxValue, sub.SubscriptionTypeId);

                    var events = dataContext.EventNotifications.Count();
                    Assert.AreEqual(1, events);
                }
            }

            [Test]
            public void NonMatchingUuids_NotPushed(
                [Random(1, 100, 3)] int personCount,
                [Random(1, 100, 3)] int changeCount
                )
            {

                using (var dataContext = new EventBrokerDataContext(ConnectionString))
                {
                    var sub = AddSubscription(dataContext, null, false, true, SubscriptionType.SubscriptionTypes.DataChange);
                    var persons = AddPersons(sub, personCount);
                    var changes = AddChanges(dataContext, changeCount);                    
                    dataContext.SubmitChanges();


                    dataContext.EnqueueDataChangeEventNotifications(DateTime.Now, int.MaxValue, sub.SubscriptionTypeId);

                    var events = dataContext.EventNotifications.Count();
                    Assert.AreEqual(0, events);
                }
            }
        }
    }
}
