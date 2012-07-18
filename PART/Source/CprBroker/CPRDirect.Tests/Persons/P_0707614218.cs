using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect.Persons
{
    [TestFixture]
    public class P_0707614218 : Person
    {
        [Test]
        public void ToRelationListeType_CorrectSpouseCounts()
        {
            var data = GetData();
            var person = GetPerson();
            var rel = person.ToRelationListeType(pnr => Guid.NewGuid());
            var partners = rel.RegistreretPartner;
            var spouses = rel.Aegtefaelle;


            Assert.AreEqual(2, spouses.Length);
            Assert.AreEqual(0, partners.Length);
        }
    }
}
