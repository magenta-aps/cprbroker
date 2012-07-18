using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect.Persons
{
    [TestFixture]
    public class P_0702614147 : Person
    {
        [Test]
        public void ToSpouses_OK()
        {
            var data = GetData();
            var person = GetPerson();
            var value = person.ToRelationListeType(pnr => Guid.NewGuid()).Aegtefaelle;
            Assert.AreEqual(1, value.Length);
            Assert.AreEqual(person.HistoricalCivilStatus[0].CivilStatusStartDate.Value, value[0].Virkning.FraTidspunkt.ToDateTime());
            Assert.AreEqual(person.HistoricalCivilStatus[0].CivilStatusEndDate.Value, value[0].Virkning.TilTidspunkt.ToDateTime());
        }

        [Test]
        public void ToRegisteredPartners_Zero()
        {
            var data = GetData();
            var person = GetPerson();
            var value = person.ToRelationListeType(pnr => Guid.NewGuid()).RegistreretPartner;
            Assert.AreEqual(0, value.Length);
        }
    }
}
