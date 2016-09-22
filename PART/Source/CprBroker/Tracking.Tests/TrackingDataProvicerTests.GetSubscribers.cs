using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CprBroker.PartInterface.Tracking;
using CprBroker.Data.Part;
using System.Data.SqlClient;

namespace CprBroker.Tests.Tracking
{
    namespace TrackingDataProvicerTests
    {
        [TestFixture]
        public class GetSubscribersTests : PartInterface.TestBase
        {
            [Test]
            public void GetSubscribers_EmptyDB_NonePerPerson()
            {

            }

            [Test]
            public void GetSubscribers_CriteriaSubscription_NonePerPerson()
            { }

            [Test]
            public void GetSubscribers_AllPersonsSubscription_NonePerPerson()
            { }

            [Test]
            public void GetSubscribers_ExplicitSubscription_Returned(
                [Values(true, false)]bool isChangeSubscription
                )
            { }
        }
    }
}
