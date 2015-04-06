using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.ServicePlatform;
using CprBroker.Providers.ServicePlatform.Responses;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.ServicePlatform
{
    namespace StamPlusResponseTests
    {
        [TestFixture]
        public class ToAttributListTests : BaseResponseTests
        {
            public AttributListeType GetAttributes(string pnr)
            {
                var txt = GetResponse(pnr, "Stam+");
                var w = new StamPlusResponse(txt);
                return w.ToAttributListeType();
            }

            public CprBorgerType GetCprBorger(string pnr)
            {
                return GetAttributes(pnr).RegisterOplysning.First().Item as CprBorgerType;
            }
            public AdresseType GetAddress(string pnr)
            {
                return GetCprBorger(pnr).FolkeregisterAdresse;
            }
            public NavnStrukturType GetName(string pnr)
            {
                return GetAttributes(pnr).Egenskab.First().NavnStruktur;
            }

            [Test]
            [TestCase(typeof(DanskAdresseType))]
            //[TestCase(typeof(GroenlandAdresseType))]
            [TestCase(typeof(VerdenAdresseType))]
            public void ToAttributList_HasOneAddressOfType(Type t)
            {
                var matchTypes = this.PNRs
                    .Select(p => GetAddress(p))
                    .Where(a => a != null && t.IsInstanceOfType(a.Item));
                Assert.Greater(matchTypes.Count(), 0);
            }

            [Test]
            [TestCaseSource("PNRs")]
            public void ToAttributList_AddressingNameNotEmpty(string pnr)
            {
                var name = GetName(pnr);
                Assert.IsNotEmpty(name.PersonNameForAddressingName);
            }

            [Test]
            [TestCaseSource("PNRs")]
            public void ToAttributList_NameNotEmpty(string pnr)
            {
                var name = GetName(pnr);
                Assert.False(name.PersonNameStructure.IsEmpty);
            }
        }
    }
}
