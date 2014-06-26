using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Data.Linq;
using CprBroker.EventBroker.Data;
using CprBroker.Schemas.Part;
using CprBroker.Data.Part;

namespace CprBroker.Tests.PartInterface
{
    [TestFixture]
    public class TestBase
    {
        public class DatabaseInfo
        {
            public string DbName;
            public string ConnectionString;
            public string MasterConnectionString;
            public KeyValuePair<string, string>[] Lookups;
        }

        public List<DatabaseInfo> Databases = new List<DatabaseInfo>();
        public DatabaseInfo CprDatabase;
        public DatabaseInfo EventDatabase;

        [TestFixtureSetUp]
        public void CreateDatabases()
        {
            CprDatabase = CreateDatabase("CprBrrokerTest_",
                CprBrokerWixInstallers.Properties.ResourcesExtensions.AllCprBrokerDatabaseObjectsSql,
                CprBrokerWixInstallers.Properties.ResourcesExtensions.Lookups
                );

            EventDatabase = CreateDatabase("EventBrokerTest_",
                CprBroker.Installers.EventBrokerInstallers.Properties.ResourcesExtensions.AllEventBrokerDatabaseObjectsSql,
                CprBroker.Installers.EventBrokerInstallers.Properties.ResourcesExtensions.Lookups);

            CprBroker.Config.ConfigManager.Current.Settings["CprBrokerConnectionString"] = CprDatabase.ConnectionString;
            CprBroker.Config.ConfigManager.Current.Settings["EventBrokerConnectionString"] = EventDatabase.ConnectionString;

            CprBroker.Config.ConfigManager.Current.Commit();
        }

        public DatabaseInfo CreateDatabase(string prefix, string ddl, KeyValuePair<string, string>[] lookups)
        {
            var db = new DatabaseInfo();
            Databases.Add(db);

            db.DbName = prefix + CprBroker.Utilities.Strings.NewRandomString(7);
            db.MasterConnectionString = "Data Source=localhost\\sqlexpress; integrated security=sspi;";
            db.ConnectionString = string.Format("Data Source=localhost\\sqlexpress; integrated security=sspi; initial catalog={0}", db.DbName);
            db.Lookups = lookups;

            // Create DB
            using (var conn = new System.Data.SqlClient.SqlConnection(db.MasterConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("CREATE DATABASE " + db.DbName + "", conn);
                cmd.ExecuteNonQuery();
            }
            CprBroker.Installers.DatabaseCustomAction.ExecuteDDL(ddl, db.ConnectionString);            

            return db;
        }

        [TestFixtureTearDown]
        public void DeleteDatabases()
        {
            foreach (var db in Databases)
            {
                string kill =
                    string.Format("declare @kill varchar(8000) = ''; select @kill=@kill+'kill '+convert(varchar(5),spid)+';'    from master..sysprocesses where dbid=db_id('{0}');exec (@kill); ", db.DbName)
                    + "\r\nGO\r\n"
                    + "DROP DATABASE " + db.DbName + "";
                CprBroker.Installers.DatabaseCustomAction.ExecuteDDL(kill, db.MasterConnectionString);
            }
        }

        [SetUp]
        public void InsertLookups()
        {
            foreach (var db in Databases)
            {
                CprBroker.Installers.DatabaseCustomAction.InsertLookups(db.Lookups, db.ConnectionString);
            }
        }

        [TearDown]
        public void DeleteAllData()
        {
            foreach (var db in Databases)
            {
                using (var dataContext = new DataContext(db.ConnectionString))
                {
                    var tables = dataContext.ExecuteQuery<string>("select name from sys.tables").ToList();
                    var deletedTables = new List<string>();
                    while (deletedTables.Count < tables.Count)
                    {
                        tables
                            .Except(deletedTables.ToArray())
                            .ToList()
                            .ForEach(t =>
                            {
                                try
                                {
                                    dataContext.ExecuteCommand("delete " + t);
                                    deletedTables.Add(t);
                                }
                                catch { }
                            });

                    }
                }
            }
        }

        public Subscription AddSubscription(EventBrokerDataContext dataContext, SoegObjektType criteria, bool forAll, bool ready, EventBroker.Data.SubscriptionType.SubscriptionTypes type)
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
                Deactivated = null,
                LastCheckedUUID = null,

                // Control
                Criteria = criteria == null ? null : System.Xml.Linq.XElement.Load(new System.IO.StringReader(CprBroker.Utilities.Strings.SerializeObject(criteria))),
                IsForAllPersons = forAll,
                Ready = ready,
                SubscriptionTypeId = (int)type
            };
            dataContext.Subscriptions.InsertOnSubmit(sub);
            return sub;
        }

        public SubscriptionPerson[] AddPersons(Subscription sub, int count)
        {
            var ret = new SubscriptionPerson[count];
            for (int i = 0; i < count; i++)
            {
                ret[i] = new SubscriptionPerson()
                {
                    Created = DateTime.Now,
                    PersonUuid = Guid.NewGuid(),
                    Removed = null,
                    SubscriptionPersonId = Guid.NewGuid(),
                    //SubscriptionId = ??, Subscription = ??
                };
            }
            sub.SubscriptionPersons.AddRange(ret);
            return ret;
        }

        public DataChangeEvent[] AddChanges(EventBrokerDataContext dataContext, int count)
        {
            var ret = new DataChangeEvent[count];
            for (int i = 0; i < count; i++)
            {
                ret[i] = new DataChangeEvent()
                {
                    DataChangeEventId = Guid.NewGuid(),
                    DueDate = DateTime.Now,
                    PersonRegistrationId = Guid.NewGuid(),
                    PersonUuid = Guid.NewGuid(),
                    ReceivedDate = DateTime.Now,
                    //ReceivedOrder = ??, SubscriptionCriteriaMatches = ??
                };
            }
            dataContext.DataChangeEvents.InsertAllOnSubmit(ret);
            return ret;
        }

        public DataChangeEvent[] AddChanges(EventBrokerDataContext dataContext, params PersonRegistration[] regs)
        {
            var ret = AddChanges(dataContext, regs.Length);
            for (int i = 0; i < regs.Length; i++)
            {
                ret[i].PersonUuid = regs[i].UUID;
                ret[i].PersonRegistrationId = regs[i].PersonRegistrationId;
            }
            dataContext.DataChangeEvents.InsertAllOnSubmit(ret);
            return ret;
        }

    }
}
