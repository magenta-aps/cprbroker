using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace IndividualResponse.StatesTests
    {
        [TestFixture]
        public class ToTilstandListeType
        {
            [Test]
            public void ToTilstandListeType_NotNull()
            {
                var resp = new IndividualResponseType() { PersonInformation = new PersonInformationType() { Status = 1 }, CurrentCivilStatus = new CurrentCivilStatusType() { CivilStatusCode = 'U' } };
                var result = resp.ToTilstandListeType();
                Assert.NotNull(result);
                Assert.NotNull(result.CivilStatus);
                Assert.NotNull(result.LivStatus);
                Assert.Null(result.LokalUdvidelse);
            }
        }
    }
}
