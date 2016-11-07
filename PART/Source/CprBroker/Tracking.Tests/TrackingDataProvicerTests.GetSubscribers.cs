using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CprBroker.PartInterface.Tracking;
using CprBroker.Data.Part;
using System.Data.SqlClient;
using CprBroker.EventBroker.Data;
using CprBroker.Engine;

namespace CprBroker.Tests.Tracking
{
    namespace TrackingDataProviderTests
    {
        [TestFixture]
        public class GetSubscribersTests : PartInterface.TestBase
        {
            private readonly SubscriptionType.SubscriptionTypes[] SubscriptionTypes = new SubscriptionType.SubscriptionTypes[] { SubscriptionType.SubscriptionTypes.Birthdate, SubscriptionType.SubscriptionTypes.DataChange };

            [Test]
            public void GetSubscribers_EmptyDB_NonePerPerson([Values(1, 20, 15, 27, 100)]int count)
            {
                var prov = new TrackingDataProvider();
                var uuids = Enumerable.Range(1, count).Select(i => Guid.NewGuid()).ToArray();
                var ret = prov.GetSubscribers(uuids);
                Assert.AreEqual(uuids.Length, ret.Length);
                foreach (var r in uuids.Zip(ret, (a, b) => new { a, b }))
                {
                    Assert.AreEqual(r.a, r.b.UUID);
                    Assert.IsEmpty(r.b.Subscribers);
                }
            }

            [Test]
            public void GetSubscribers_CriteriaSubscription_NonePerPerson(
                [Values(1, 20, 15, 27, 100)]int count,
                [ValueSource(nameof(SubscriptionTypes))]SubscriptionType.SubscriptionTypes subscriptionType)
            {
                using (var dataContext = new EventBrokerDataContext())
                {
                    var sub = AddSubscription(dataContext, new Schemas.Part.SoegObjektType(), false, true, subscriptionType);
                    dataContext.SubmitChanges();
                }
                var prov = new TrackingDataProvider();
                var uuids = Enumerable.Range(1, count).Select(i => Guid.NewGuid()).ToArray();
                var ret = prov.GetSubscribers(uuids);
                Assert.AreEqual(uuids.Length, ret.Length);
                foreach (var r in uuids.Zip(ret, (a, b) => new { a, b }))
                {
                    Assert.AreEqual(r.a, r.b.UUID);
                    Assert.IsEmpty(r.b.Subscribers);
                }
            }

            [Test]
            public void GetSubscribers_AllPersonsSubscription_NonePerPerson(
                [Values(1, 20, 15, 27, 100)]int count,
                [ValueSource(nameof(SubscriptionTypes))]SubscriptionType.SubscriptionTypes subscriptionType)
            {
                using (var dataContext = new EventBrokerDataContext())
                {
                    var sub = AddSubscription(dataContext, null, true, true, subscriptionType);
                    dataContext.SubmitChanges();
                }
                var prov = new TrackingDataProvider();
                var uuids = Enumerable.Range(1, count).Select(i => Guid.NewGuid()).ToArray();
                var ret = prov.GetSubscribers(uuids);
                Assert.AreEqual(uuids.Length, ret.Length);
                foreach (var r in uuids.Zip(ret, (a, b) => new { a, b }))
                {
                    Assert.AreEqual(r.a, r.b.UUID);
                    Assert.IsEmpty(r.b.Subscribers);
                }
            }

            [Test]
            public void GetSubscribers_ExplicitSubscription_Returned(
                [Values(1, 20, 15, 27, 100)]int count,
                [ValueSource(nameof(SubscriptionTypes))]SubscriptionType.SubscriptionTypes subscriptionType)
            {
                using (var dataContext = new EventBrokerDataContext())
                {
                    var sub = AddSubscription(dataContext, null, false, true, subscriptionType);
                    sub.ApplicationId = CprBroker.Utilities.Constants.BaseApplicationId;
                    AddPersons(sub, count);
                    dataContext.SubmitChanges();

                    var prov = new TrackingDataProvider();
                    var uuids = sub.SubscriptionPersons.Select(sp => sp.PersonUuid.Value).ToArray();
                    var ret = prov.GetSubscribers(uuids);
                    Assert.AreEqual(uuids.Length, ret.Length);
                    foreach (var r in uuids.Zip(ret, (a, b) => new { a, b }))
                    {
                        Assert.AreEqual(r.a, r.b.UUID);
                        Assert.AreEqual(1, r.b.Subscribers.Length);
                        Assert.AreEqual(sub.ApplicationId.ToString(), r.b.Subscribers[0].ApplicationId);
                    }
                }
            }
        }
    }
}
