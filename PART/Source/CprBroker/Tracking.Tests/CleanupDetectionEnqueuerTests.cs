using CprBroker.Data.Applications;
using CprBroker.Data.Part;
using CprBroker.Data.Queues;
using CprBroker.Engine;
using CprBroker.Engine.Queues;
using CprBroker.PartInterface.Tracking;
using CprBroker.Tests.PartInterface;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CprBroker.Tests.Tracking
{
    namespace CleanupDetectionEnqueuerTests
    {
        [TestFixture]
        public class CanRunCleanup : TestBase
        {
            [SetUp]
            public void InitBrokerContext()
            {
                BrokerContext.Current = null;
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            }

            [Test]
            public void CanRunCleanup_None_False()
            {
                var det = new CleanupDetectionEnqueuer();
                var ret = det.CanRunCleanup();
                Assert.False(ret);
            }

            [Test]
            public void CanRunCleanup_NewOperations_False()
            {
                BrokerContext.Current.RegisterOperation(Data.Applications.OperationType.Types.Generic, Guid.NewGuid().ToString());
                var det = new CleanupDetectionEnqueuer();
                var ret = det.CanRunCleanup();
                Assert.False(ret);
            }

            [Test]
            public void CanRunCleanup_OldOperations_True()
            {
                using (var dataContext = new ApplicationDataContext())
                {
                    var act = new Activity()
                    {
                        ActivityId = Guid.NewGuid(),
                        ApplicationId = BrokerContext.Current.ApplicationId.Value,
                        StartTS = DateTime.Now - SettingsUtilities.MaxInactivePeriod - TimeSpan.FromDays(1),
                        UserId = "",
                        MethodName = "",
                        UserToken = "",
                    };
                    dataContext.Activities.InsertOnSubmit(act);

                    act.Operations.Add(new Operation()
                    {
                        OperationTypeId = (int)OperationType.Types.Generic,
                        OperationId = Guid.NewGuid(),
                        ActivityId = act.ActivityId,
                        OperationKey = Guid.NewGuid().ToString()
                    });

                    dataContext.SubmitChanges();
                }

                var det = new CleanupDetectionEnqueuer();
                var ret = det.CanRunCleanup();
                Assert.True(ret);
            }
        }

        [TestFixture]
        public class PerformTimerAction : CprBroker.Tests.PartInterface.TestBase
        {
            [SetUp]
            public void Setup()
            {
                Utilities.InitBrokerContext();
                CprBroker.Engine.Queues.Queue.AddQueue<CleanupQueue>(CleanupQueue.QueueTypeId, new Dictionary<string, string>(), 1000, 1);
            }

            [Test]
            public void PerformTimerAction_Data_Enqueued([Values(10, 100, 1000, 10000)]int count)
            {
                BrokerContext.Current.RegisterOperation(OperationType.Types.Generic, "123");
                using (var dc = new ApplicationDataContext(CprDatabase.ConnectionString))
                {
                    dc.Operations.First().Activity.StartTS = DateTime.Now.AddYears(-1);
                    dc.SubmitChanges();
                }

                var persons = Utilities.NewPersonIdentifiers(count);
                using (var dc = new PartDataContext(CprDatabase.ConnectionString))
                {
                    dc.PersonMappings.InsertAllOnSubmit(persons.Select(p => new PersonMapping() { CprNumber = p.CprNumber, UUID = p.UUID.Value }));
                    dc.Persons.InsertAllOnSubmit(persons.Select(p => new Person() { UUID = p.UUID.Value, UserInterfaceKeyText = p.CprNumber }));
                    var registrations = persons.Select(p => new PersonRegistration() { UUID = p.UUID.Value, BrokerUpdateDate = DateTime.Now, LifecycleStatusId = 1, PersonRegistrationId = Guid.NewGuid(), RegistrationDate = DateTime.Now, Contents = null });
                    dc.PersonRegistrations.InsertAllOnSubmit(registrations);
                    dc.SubmitChanges();
                }

                var startTS = DateTime.Now;
                var enq = new CleanupDetectionEnqueuerStub() { BatchSize = Math.Min(1000, count - 1) };
                enq._PerformTimerAction();
                Console.WriteLine("Time consumed <{0}>", DateTime.Now - startTS);
                using (var qdc = new QueueDataContext(CprDatabase.ConnectionString))
                {
                    var c = qdc.QueueItems.Count();
                    Assert.AreEqual(count, c);
                }
            }

            class CleanupDetectionEnqueuerStub : CleanupDetectionEnqueuer
            {
                public void _PerformTimerAction()
                {
                    PerformTimerAction();
                }
            }
        }
    }
}
