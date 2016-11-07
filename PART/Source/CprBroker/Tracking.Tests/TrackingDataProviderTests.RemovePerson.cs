using CprBroker.PartInterface.Tracking;
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
        public class RemovePerson:PartInterface.TestBase
        {
            [Test]
            public void RemovePerson_EmptyDB_Passes()
            {
                var prov = new TrackingDataProvider();
                prov.RemovePerson(new Schemas.PersonIdentifier() { CprNumber = CprBroker.Tests.PartInterface.Utilities.RandomCprNumber(), UUID = Guid.NewGuid() });
            }
        }
    }
}
