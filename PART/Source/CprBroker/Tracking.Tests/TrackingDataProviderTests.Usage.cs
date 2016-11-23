using CprBroker.Engine;
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
    namespace TrackingDataProviderTests
    {
        [TestFixture]
        public class HasPersonUsage : TestBase
        {
            [SetUp]
            [Test]
            public void InitBrokerContext()
            {
                BrokerContext.Current = null;
                BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            }

            [Test]
            public void HasPersonUsage_None_False()
            {
                var prov = new TrackingDataProvider();
                var uuid = Guid.NewGuid();
                var ret = prov.PersonHasUsage(uuid, null, null);
                Assert.False(ret);
            }

            [Test]
            public void HasPersonUsage_OldUsage_False()
            {
                var prov = new TrackingDataProvider();
                var uuid = Guid.NewGuid();
                BrokerContext.Current.RegisterOperation(Data.Applications.OperationType.Types.Read, uuid.ToString());
                var ret = prov.PersonHasUsage(uuid, DateTime.Now.AddMinutes(1), null);
                Assert.False(ret);
            }

            [Test]
            public void HasPersonUsage_IrrelevantOperationUsage_False()
            {
                var prov = new TrackingDataProvider();
                var uuid = Guid.NewGuid();
                BrokerContext.Current.RegisterOperation(Data.Applications.OperationType.Types.Generic, uuid.ToString());
                var ret = prov.PersonHasUsage(uuid, null, null);
                Assert.False(ret);
            }

            [Test]
            public void HasPersonUsage_OtherKeysUsage_False()
            {
                var prov = new TrackingDataProvider();
                var uuid0 = Guid.NewGuid();
                var uuid = Guid.NewGuid();
                BrokerContext.Current.RegisterOperation(Data.Applications.OperationType.Types.Read, uuid0.ToString());
                var ret = prov.PersonHasUsage(uuid, null, null);
                Assert.False(ret);
            }

            [Test]
            public void HasPersonUsage_RelevantUsage_True(
                [Range(0, 10)]int dummy)
            {
                var prov = new TrackingDataProvider();
                var uuid = Guid.NewGuid();

                BrokerContext.Current.RegisterOperation(Data.Applications.OperationType.Types.Read, uuid.ToString());
                var ret = prov.PersonHasUsage(uuid, null, null);
                Assert.True(ret);
            }
        }

    }
}
