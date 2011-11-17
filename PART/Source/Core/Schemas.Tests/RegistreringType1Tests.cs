using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.Schemas
{
    [TestFixture]
    class RegistreringType1Tests
    {
        TilstandListeType CreateTilstandListeType(DateTime date)
        {
            return new TilstandListeType()
            {
                CivilStatus = new CivilStatusType() { TilstandVirkning = TilstandVirkningType.Create(date) },
                LivStatus = new LivStatusType() { TilstandVirkning = TilstandVirkningType.Create(date) }
            };
        }

        RelationListeType CreateRelationListeType(DateTime date)
        {
            return new RelationListeType()
            {
                Aegtefaelle = new PersonRelationType[]{
                    PersonRelationType.Create(Guid.NewGuid(),date,null),
                    PersonRelationType.Create(Guid.NewGuid(),date,null)
                }
            };
        }

        [Test]
        public void CalculateVirkning_TilstandeVirkning_CorrectLength()
        {
            DateTime date = DateTime.Today;
            var registration = new RegistreringType1()
            {
                TilstandListe = CreateTilstandListeType(date)
            };
            registration.CalculateVirkning();
            Assert.AreEqual(2, registration.Virkning.Length);
        }

        [Test]
        public void CalculateVirkning_TilstandeVirkning_CorrectValues()
        {
            DateTime date = DateTime.Today;
            var registration = new RegistreringType1()
            {
                TilstandListe = CreateTilstandListeType(date)
            };
            registration.CalculateVirkning();
            Assert.AreEqual(date, registration.Virkning[0].FraTidspunkt.ToDateTime().Value);
        }

        [Test]
        public void CalculateVirkning_RelationVirkning_CorrectLength()
        {
            DateTime date = DateTime.Today;
            var registration = new RegistreringType1()
            {
                RelationListe = CreateRelationListeType(date)
            };
            registration.CalculateVirkning();
            Assert.AreEqual(2, registration.Virkning.Length);
        }

        [Test]
        public void CalculateVirkning_RelationVirkning_CorrectValues()
        {
            DateTime date = DateTime.Today;
            var registration = new RegistreringType1()
            {
                RelationListe = CreateRelationListeType(date)
            };
            registration.CalculateVirkning();
            Assert.AreEqual(date, registration.Virkning[0].FraTidspunkt.ToDateTime().Value);
        }

    }
}
