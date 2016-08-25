using CprBroker.Data.Applications;
using CprBroker.Engine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.Engine
{
    namespace BrokerContextTests
    {
        [TestFixture]
        public class RegisterOperation
        {
            [Test]
            public void RegisterOperation_Read_Found([Values(0, 1, 10, 100, 200, 500)]int count)
            {
                BrokerContext.Initialize(Utilities.AppToken, "test", true);
                BrokerContext.Current.RegisterOperation(OperationType.Types.Read, Utilities.RandomGuidStrings(count));
                using (var dataContext = new ApplicationDataContext())
                {
                    var c = dataContext.Operations.Count(op => op.ActivityId == BrokerContext.Current.ActivityId);
                    Assert.AreEqual(count, c);
                }
            }
        }
    }
}
