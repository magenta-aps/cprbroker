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
    [TestFixture]
    class EnqueueDataChangeEventNotifications
    {
        public EnqueueDataChangeEventNotifications()
        {
            DbName = "EventBrokerTest_" + CprBroker.Utilities.Strings.NewRandomString(7);
            MasterConnectionString = "Data Source=localhost\\sqlexpress; integrated security=sspi;";
            ConnectionString = string.Format("Data Source=localhost\\sqlexpress; integrated security=sspi; initial catalog={0}", DbName);
        }

        string DbName;
        string ConnectionString;
        string MasterConnectionString;

        [SetUp]
        public void Setup()
        {
            // Create DB
            using (var conn = new System.Data.SqlClient.SqlConnection(MasterConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("CREATE DATABASE " + DbName + "", conn);
                cmd.ExecuteNonQuery();
            }
            CprBroker.Installers.DatabaseCustomAction.ExecuteDDL(CprBroker.Installers.EventBrokerInstallers.Properties.ResourcesExtensions.AllEventBrokerDatabaseObjectsSql, ConnectionString);
            CprBroker.Installers.DatabaseCustomAction.InsertLookups(CprBroker.Installers.EventBrokerInstallers.Properties.ResourcesExtensions.Lookups, ConnectionString);
        }

        [TearDown]
        public void TearDown()
        {
            string kill =
                string.Format("declare @kill varchar(8000) = ''; select @kill=@kill+'kill '+convert(varchar(5),spid)+';'    from master..sysprocesses where dbid=db_id('{0}');exec (@kill); ", DbName)
                + "\r\nGO\r\n"
                + "DROP DATABASE " + DbName + "";
            CprBroker.Installers.DatabaseCustomAction.ExecuteDDL(kill, MasterConnectionString);
        }

        [Test]
        public void EnqueueDataChangeEventNotifications_LatestReceivedOrder_OneNotif()
        {
            /*
             * ready
             * IsForAll
             * TypeId
             */
            using (var dataContext = new EventBrokerDataContext(ConnectionString))
            {
                var sub = new Subscription()
                {
                    // Child
                    BirthdateSubscription = null,
                    DataSubscription = new DataSubscription(),

                    // Ids
                    ApplicationId = Guid.NewGuid(),
                    SubscriptionId = Guid.NewGuid(),

                    // Other
                    Created = DateTime.Now,

                    // Control
                    Criteria = null,
                    Deactivated = null,
                    IsForAllPersons = true,
                    LastCheckedUUID = null,
                    Ready = true,
                    SubscriptionTypeId = (int)SubscriptionType.SubscriptionTypes.DataChange
                };
                dataContext.Subscriptions.InsertOnSubmit(sub);

                var changes = new DataChangeEvent[] { 
                    new DataChangeEvent(){ DataChangeEventId = Guid.NewGuid(), DueDate = DateTime.Now, PersonRegistrationId=Guid.NewGuid(), PersonUuid = Guid.NewGuid(), ReceivedDate = DateTime.Now,
                        //ReceivedOrder = ??, SubscriptionCriteriaMatches = ??
                    },
                    new DataChangeEvent(){ DataChangeEventId = Guid.NewGuid(), DueDate = DateTime.Now, PersonRegistrationId=Guid.NewGuid(), PersonUuid = Guid.NewGuid(), ReceivedDate = DateTime.Now,
                        //ReceivedOrder = ??, SubscriptionCriteriaMatches = ??
                    }
                };
                dataContext.DataChangeEvents.InsertAllOnSubmit(changes);
                dataContext.SubmitChanges();

                // Now call SP
                dataContext.EnqueueDataChangeEventNotifications(DateTime.Now, 1, sub.SubscriptionTypeId);

                // Validate
                var eventsCount = dataContext.EventNotifications.Count();
                Assert.AreEqual(1, eventsCount);
            }
        }
    }
}
