using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.EventBroker.Subscriptions;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.EventBroker.Subscriptions
{
    namespace SubscriptionDataProviderTests
    {
        [TestFixture]
        public class GetActiveSubscriptionListsTests : Notifications.NotificationTestBase
        {
            [SetUp]
            public new void Setup()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            }

            FileShareChannelType CreateChannel()
            {
                string path = "c:\\Notif\\Temp";
                return new FileShareChannelType() { Path = path };
            }

            [Test]
            public void GetActiveSubscriptionLists_None_None()
            {
                var prov = new SubscriptionDataProvider();
                var ret = prov.GetActiveSubscriptionsList();
                Assert.IsEmpty(ret);
            }

            [Test]
            public void GetActiveSubscriptionLists_Deleted_None()
            {
                var prov = new SubscriptionDataProvider();
                var sub = prov.Subscribe(CreateChannel(), null);
                prov.Unsubscribe(new Guid(sub.SubscriptionId));

                var ret = prov.GetActiveSubscriptionsList();
                Assert.IsEmpty(ret);
            }

            [Test]
            public void GetActiveSubscriptionLists_Birthdate_Found()
            {
                var prov = new SubscriptionDataProvider();
                var sub = prov.SubscribeOnBirthdate(CreateChannel(), null, 0, null);

                var ret = prov.GetActiveSubscriptionsList();
                Assert.IsNotEmpty(ret);
                Assert.AreEqual(sub.SubscriptionId, ret.First().SubscriptionId);
            }

            [Test]
            public void GetActiveSubscriptionLists_Change_Found()
            {
                var prov = new SubscriptionDataProvider();
                var sub = prov.Subscribe(CreateChannel(), null);

                var ret = prov.GetActiveSubscriptionsList();
                Assert.AreEqual(1,ret.Length);
                Assert.AreEqual(sub.SubscriptionId, ret.First().SubscriptionId);
            }

            [Test]
            public void GetActiveSubscriptionLists_OneDeletedOneNot_OneFound()
            {
                var prov = new SubscriptionDataProvider();
                var delSub = prov.Subscribe(CreateChannel(), null);
                prov.Unsubscribe(new Guid(delSub.SubscriptionId));

                var actSub = prov.Subscribe(CreateChannel(), null);

                var ret = prov.GetActiveSubscriptionsList();
                Assert.AreEqual(1, ret.Length);
                Assert.AreEqual(actSub.SubscriptionId, ret.First().SubscriptionId);
            }
        }
    }
}
