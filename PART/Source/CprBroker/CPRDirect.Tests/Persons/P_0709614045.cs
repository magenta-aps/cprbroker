using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect.Persons
{
    [TestFixture]
    public class P_0709614045 : Person
    {
        [Test]
        public void ToRelationListeType_CorrectSpouses()
        {
            var data = GetData();
            var person = GetPerson();
            var rel = person.ToRelationListeType(pnr => Guid.NewGuid());
            var value = rel.RegistreretPartner;
            Assert.AreEqual(1, value.Length);
            Assert.AreEqual(person.CurrentCivilStatus.CivilStatusStartDate, value[0].Virkning.FraTidspunkt.ToDateTime());

            var spouses = rel.Aegtefaelle;
            Assert.AreEqual(1, spouses.Length);
            Assert.AreEqual(person.HistoricalCivilStatus[0].CivilStatusStartDate, spouses[0].Virkning.TilTidspunkt.ToDateTime());
        }
    }
}
