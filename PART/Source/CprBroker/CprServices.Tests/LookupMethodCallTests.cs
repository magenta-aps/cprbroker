using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CprServices;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.CprServices
{
    namespace LookupMethodCallTests
    {
        public class BaseTests
        {
            public static string[] LookupResponses
            {
                get { return new string[] { Properties.Resources.ADRESSE3_Response_OK }; }
            }
        }
        [TestFixture]
        public class Adresse3Tests
        {
            [Test]
            [Ignore("Lookup is not implemented officially yet")]
            [TestCaseSource(typeof(BaseTests), "LookupResponses")]
            public void ParseResponse_OK_HasPostCode(string response)
            {
                var call = new SearchMethodCall();
                var p = call.ParseResponse(response, false).First();
                var reg = p.ToRegistreringType1();
                Assert.IsNotEmpty(((reg.AttributListe.RegisterOplysning.First().Item as CprBorgerType).FolkeregisterAdresse.Item as DanskAdresseType).AddressComplete.AddressPostal.PostCodeIdentifier);
            }
        }
    }
}
