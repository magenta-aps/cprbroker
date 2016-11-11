using CprBroker.Data.Part;
using CprBroker.Engine;
using CprBroker.Engine.Local;
using CprBroker.Engine.Part;
using CprBroker.Providers.DPR;
using CprBroker.Providers.DPR.Queues;
using CprBroker.Schemas;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.DPR
{
    namespace DprUpdateQueueTests
    {
        [TestFixture]
        public class Process : PartInterface.TestBase
        {
            class UUidDataProvider : IPartPersonMappingDataProvider
            {
                static object _MapLock = new object();
                public static Dictionary<string, Guid> _Map;

                public Version Version { get { throw new NotImplementedException(); } }
                public bool IsAlive() { throw new NotImplementedException(); }

                public static Guid? _GetPersonUuid(string cprNumber)
                {
                    lock (_MapLock)
                    {
                        if (_Map == null)
                        {
                            using (var dc = new PartDataContext())
                            {
                                _Map = dc.PersonMappings.ToDictionary(pm => pm.CprNumber, pm => pm.UUID);
                            }
                        }
                        if (!_Map.ContainsKey(cprNumber))
                        {
                            _Map[cprNumber] = Guid.NewGuid();
                            UpdateDatabase.UpdatePersonUuid(cprNumber, _Map[cprNumber]);
                        }
                        return _Map[cprNumber];
                    }
                }

                public Guid? GetPersonUuid(string cprNumber)
                {
                    return _GetPersonUuid(cprNumber);
                }
                public Guid?[] GetPersonUuidArray(string[] cprNumberArray)
                {
                    return cprNumberArray.Select(cpr => _GetPersonUuid(cpr)).ToArray();
                }
            }

            DatabaseInfo _DprDatabase;
            DprDatabaseDataProvider _DprProvider;
            Guid _DprProviderId;
            public override void CreateDatabases()
            {
                base.CreateDatabases();
                _DprDatabase = CreateDatabase("dpr_test_", CprBroker.DBR.Properties.Resources.CreateDbrTables, new KeyValuePair<string, string>[] { });
            }

            [SetUp]
            public void DoSetup()
            {
                UUidDataProvider._Map = null;

                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");

                _DprProvider = new DprDatabaseDataProvider()
                {
                    ConfigurationProperties = new Dictionary<string, string>(),
                    ConnectionString = _DprDatabase.ConnectionString,
                    AutoUpdate = true
                };

                var dbProv = AddDataProvider<DprDatabaseDataProvider>(_DprProvider);
                _DprProviderId = dbProv.DataProviderId;
                RegisterDataProviderType<CprBroker.Providers.Local.DatabaseDataProvider>();
            }

            void AddPerson(string pnr, bool part, bool dpr)
            {
                var person = Tests.CPRDirect.Persons.Person.GetPerson(pnr);
                if (part)
                {
                    var pId = new PersonIdentifier() { CprNumber = pnr, UUID = UUidDataProvider._GetPersonUuid(pnr) };
                    UpdateDatabase.UpdatePersonRegistration(pId, person.ToRegistreringType1((c) => UUidDataProvider._GetPersonUuid(c).Value));
                }

                if (dpr)
                {
                    using (var dataContext = new DPRDataContext(_DprDatabase.ConnectionString))
                    {
                        DBR.CprConverter.AppendPerson(person, dataContext);
                        dataContext.SubmitChanges();
                    }
                }
            }

            [Test]
            public void Process_NonExisting_Fails(
                [ValueSource(typeof(Tests.CPRDirect.Utilities), nameof(Tests.CPRDirect.Utilities.PNRs))]string pnr)
            {
                AddPerson(pnr, false, true);
                var queue = new DprUpdateQueue();
                var ret = queue.Process(new DprUpdateQueueItem[] { new DprUpdateQueueItem() { DataProviderId = _DprProviderId, Pnr = decimal.Parse(pnr) } });
                Assert.IsEmpty(ret);
            }

            [Test]
            public void Process_Existing_Succeeds(
                [ValueSource(typeof(Tests.CPRDirect.Utilities), nameof(Tests.CPRDirect.Utilities.PNRs))]string pnr)
            {
                AddPerson(pnr, true, true);
                var queue = new DprUpdateQueue();
                var ret = queue.Process(new DprUpdateQueueItem[] { new DprUpdateQueueItem() { DataProviderId = _DprProviderId, Pnr = decimal.Parse(pnr) } });
                Assert.IsNotEmpty(ret);
            }

        }
    }
}
