/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
