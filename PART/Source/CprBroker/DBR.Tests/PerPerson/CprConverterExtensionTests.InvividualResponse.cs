using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas.Part;
using CprBroker.DBR;
using CprBroker.DBR.Extensions;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.DBR.PerPerson
{
    namespace CprConverterExtension.InvividualResponse
    {
        [TestFixture]
        public class ToIntervalAdjustedIndividualResponse
        {
            [Test]
            public void ToIntervalAdjustedIndividualResponse_Dead_Cleared()
            {
                var resp = new IndividualResponseType();
                resp.PersonInformation = new PersonInformationType() { Status = 90 };
                resp.ClearWrittenAddress = new ClearWrittenAddressType() { PNR = resp.PersonInformation.PNR, AddressingName = "ABC", StreetCode = 123 };

                var converted = resp.ToIntervalAdjustedIndividualResponse();
                Assert.Null(converted.GetFolkeregisterAdresseSource(false));
            }
        }
    }
}
