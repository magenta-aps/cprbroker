using CprBroker.Engine;
using CprBroker.EventBroker.Data;
using CprBroker.PartInterface.Tracking;
using CprBroker.Tests.PartInterface;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.Tracking
{
    namespace TrackingDataProviderTests
    {
        [TestFixture]
        public class PersonHasSubscribers : TestBase
        {
            Subscription AddSubscriptionAndPersons(bool forAll, params Guid[] uuids)
            {
                using (var dataContext = new EventBrokerDataContext())
                {
                    var s = AddSubscription(dataContext, null, forAll, true, SubscriptionType.SubscriptionTypes.DataChange);
                    AddSubscriptionPersons(s, uuids);
                    dataContext.SubmitChanges();
                    return s;
                }
            }

            [SetUp]
            public void InitBrokerContext()
            {
                BrokerContext.Current = null;
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            }

            [Test]
            public void PersonHasSubscribers_None_False()
            {
                var prov = new TrackingDataProvider();
                var uuid = Guid.NewGuid();
                var ret = prov.PersonHasSubscribers(uuid);
                Assert.False(ret);
            }

            [Test]
            public void PersonHasSubscribers_AllPersons_False(
                [Values(true, false)]bool hasCriteria,
                [Values(SubscriptionType.SubscriptionTypes.Birthdate, SubscriptionType.SubscriptionTypes.DataChange)] SubscriptionType.SubscriptionTypes type
                )
            {
                var prov = new TrackingDataProvider();
                var uuid = Guid.NewGuid();
                using (var dataContext = new EventBrokerDataContext())
                {
                    var criteria = hasCriteria ? new Schemas.Part.SoegObjektType() { } : null;
                    var sub = AddSubscription(dataContext, criteria, true, true, type);
                    AddSubscriptionPersons(sub, uuid);
                    dataContext.SubmitChanges();
                }

                var ret = prov.PersonHasSubscribers(uuid);
                Assert.False(ret);
            }

            [Test]
            public void PersonHasSubscribers_RemovedSubscription_False(
                [Values(SubscriptionType.SubscriptionTypes.Birthdate, SubscriptionType.SubscriptionTypes.DataChange)] SubscriptionType.SubscriptionTypes type)
            {
                var prov = new TrackingDataProvider();
                var uuid = Guid.NewGuid();
                using (var dataContext = new EventBrokerDataContext())
                {
                    var sub = AddSubscription(dataContext, null, false, true, type);
                    sub.Deactivated = DateTime.Now;
                    AddSubscriptionPersons(sub, uuid);
                    dataContext.SubmitChanges();
                }

                var ret = prov.PersonHasSubscribers(uuid);
                Assert.False(ret);
            }

            [Test]
            public void PersonHasSubscribers_RemovedSubscriptionPerson_False(
                [Values(SubscriptionType.SubscriptionTypes.Birthdate, SubscriptionType.SubscriptionTypes.DataChange)] SubscriptionType.SubscriptionTypes type)
            {
                var prov = new TrackingDataProvider();
                var uuid = Guid.NewGuid();
                using (var dataContext = new EventBrokerDataContext())
                {
                    var sub = AddSubscription(dataContext, null, false, true, type);
                    var sps = AddSubscriptionPersons(sub, uuid);
                    sps[0].Removed = DateTime.Now;
                    dataContext.SubmitChanges();
                }

                var ret = prov.PersonHasSubscribers(uuid);
                Assert.False(ret);
            }

            [Test]
            public void PersonHasSubscribers_OtherPersonIncluded_False(
                [Values(SubscriptionType.SubscriptionTypes.Birthdate, SubscriptionType.SubscriptionTypes.DataChange)] SubscriptionType.SubscriptionTypes type)
            {
                var prov = new TrackingDataProvider();
                var uuid0 = Guid.NewGuid();
                var uuid = Guid.NewGuid();
                using (var dataContext = new EventBrokerDataContext())
                {
                    var sub = AddSubscription(dataContext, null, false, true, type);
                    var sps = AddSubscriptionPersons(sub, uuid0);
                    dataContext.SubmitChanges();
                }
                var ret = prov.PersonHasSubscribers(uuid);
                Assert.False(ret);
            }

            [Test]
            public void PersonHasSubscribers_PersonIncluded_True(
                [Values(SubscriptionType.SubscriptionTypes.Birthdate, SubscriptionType.SubscriptionTypes.DataChange)] SubscriptionType.SubscriptionTypes type)
            {
                var prov = new TrackingDataProvider();
                var uuid = Guid.NewGuid();
                using (var dataContext = new EventBrokerDataContext())
                {
                    var sub = AddSubscription(dataContext, null, false, true, type);
                    var sps = AddSubscriptionPersons(sub, uuid);
                    dataContext.SubmitChanges();
                }
                var ret = prov.PersonHasSubscribers(uuid);
                Assert.True(ret);
            }
        }
    }
}
