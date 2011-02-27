using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnitTester.CPRAdministrationWS;

namespace NUnitTester
{
    [NUnit.Framework.TestFixture]
    public class TestAdminSubscription
    {
        private string appToken = "1de09be5-8382-4dc1-a89b-8f86fc67f97a";
        private string userToken = "peter";
        string[] PersonCivilRegistrationIdentifiers = new string[]
            {
            "123",
            "1234",
            "12345"
            };

        private NUnitTester.CPRAdministrationWS.CPRAdministrationWS CreateAdminWebService()
        {
            CPRAdministrationWS.CPRAdministrationWS ws = new NUnitTester.CPRAdministrationWS.CPRAdministrationWS();
            ws.ApplicationHeaderValue = new NUnitTester.CPRAdministrationWS.ApplicationHeader()
            {
                ApplicationToken = appToken,
                UserToken = userToken
            };
            return ws;
        }

        [NUnit.Framework.Test]
        public void SubscribeAndDeleteSubscription()
        {
            PersonCivilRegistrationIdentifiers = null;

            CPRAdministrationWS.CPRAdministrationWS admin = CreateAdminWebService();
            CPRAdministrationWS.GPACChannelType channel = new NUnitTester.CPRAdministrationWS.GPACChannelType();
            channel.NotifyType = 1;
            channel.ObjectType = 2;
            channel.ServiceUrl = "http://notifyws/notify.asmx";
            channel.SourceUri = "publsrc://abcd";
            CPRAdministrationWS.ChangeSubscriptionType changeSubscription = admin.Subscribe(channel, PersonCivilRegistrationIdentifiers);
            Assert.IsNotNull(changeSubscription);
            Assert.IsInstanceOf<CPRAdministrationWS.GPACChannelType>(changeSubscription.NotificationChannel);
            //Assert.AreEqual(changeSubscription.PersonCivilRegistrationIdentifiers.Length, PersonCivilRegistrationIdentifiers.Length);

            bool deleteResult = admin.Unsubscribe(new Guid(changeSubscription.SubscriptionId));
            Assert.IsTrue(deleteResult);
        }

        [NUnit.Framework.Test]
        public void SubscribeAndRemoveOnBirthdate()
        {
            PersonCivilRegistrationIdentifiers = null;
            int years = 2;
            int days = 5;


            CPRAdministrationWS.CPRAdministrationWS admin = CreateAdminWebService();

            CPRAdministrationWS.FileShareChannelType channel = new NUnitTester.CPRAdministrationWS.FileShareChannelType();
            channel.Path = "c:\\";

            CPRAdministrationWS.BirthdateSubscriptionType birthdateSubscription = admin.SubscribeOnBirthdate(channel, years, days, PersonCivilRegistrationIdentifiers);
            Assert.IsNotNull(birthdateSubscription);

            bool removeResult = admin.RemoveBirthDateSubscription(new Guid(birthdateSubscription.SubscriptionId));
            Assert.IsTrue(removeResult);
        }


        [NUnit.Framework.Test]
        public void GetActiveSubsciptionsList()
        {
            CPRAdministrationWS.CPRAdministrationWS admin = CreateAdminWebService();
            CPRAdministrationWS.SubscriptionType[] list = admin.GetActiveSubsciptionsList();
            Assert.IsNotNull(list);
        }

        [NUnit.Framework.Test]
        public void TestAllPossibleSubscriptions()
        {
            string[][] personIdentifiers = new string[][] { PersonCivilRegistrationIdentifiers, null };
            ChannelBaseType[] channels = new ChannelBaseType[]{
                new GPACChannelType(){ ServiceUrl="http://notifyws/notify.asmx", SourceUri="publsrc://abcd", ObjectType=1, NotifyType=2},
                new WebServiceChannelType(){ WebServiceUrl="http://notifyws/notify.asmx"},
                new FileShareChannelType(){ Path="c:\\"}
            };
            CPRAdministrationWS.CPRAdministrationWS admin = CreateAdminWebService();

            KeyValuePair<Func<ChannelBaseType, string[], SubscriptionType>, Func<Guid, bool>>[] allMethods = new KeyValuePair<Func<ChannelBaseType, string[], SubscriptionType>, Func<Guid, bool>>[] { 
                new KeyValuePair<Func<ChannelBaseType, string[], SubscriptionType>, Func<Guid,bool>>(
                    (ChannelBaseType channel,string[] cprNumbers)=>admin.Subscribe(channel,cprNumbers),
                    (Guid subId)=>admin.Unsubscribe(subId)
                        ),
                new KeyValuePair<Func<ChannelBaseType, string[], SubscriptionType>, Func<Guid,bool>>(
                    (ChannelBaseType channel,string[] cprNumbers)=>admin.SubscribeOnBirthdate(channel,null,0, cprNumbers),
                    (Guid subId)=>admin.RemoveBirthDateSubscription(subId)
                    )
            };



            foreach (string[] pis in personIdentifiers)
            {
                foreach (ChannelBaseType channel in channels)
                {
                    foreach (var methods in allMethods)
                    {
                        SubscriptionType sub = methods.Key(channel, pis);
                        Assert.IsNotNull(sub);
                        Assert.IsInstanceOf(channel.GetType(), sub.NotificationChannel);

                        bool deleteResult = methods.Value(new Guid(sub.SubscriptionId));
                        Assert.IsTrue(deleteResult);
                    }
                }
            }
        }
    }
}
