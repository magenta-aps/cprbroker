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
using NUnit.Framework.Constraints;
using CprBroker.NUnitTester.Part;

namespace CprBroker.NUnitTester
{
    class PeriodTests : PartBaseTest
    {
        string[] GetUuids(string[] pnrs)
        {
            List<string> uuids = new List<string>();
            foreach (var pnr in pnrs)
            {
                var uuid = TestRunner.PartService.GetUuid(pnr);
                Validate(uuid);
                uuids.Add(uuid.UUID);
            }
            return uuids.ToArray();
        }

        string GetUuid(string pnr)
        {
            return GetUuids(new string[] { pnr })[0];
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void ReadSnapshot(string pnr)
        {
            var uuid = GetUuid(pnr);
            var person = TestRunner.PartService.ReadSnapshot(new LaesOejebliksbilledeInputType() { UUID = uuid, VirkningDato = DateTime.Today });
            ValidateSingleReturn(person);
            Validate(new Guid(uuid), person, TestRunner.PartService);
            Assert.AreEqual(1, (person.LaesResultat.Item as FiltreretOejebliksbilledeType).AttributListe.Egenskab.Length);
            Assert.AreEqual(1, (person.LaesResultat.Item as FiltreretOejebliksbilledeType).AttributListe.RegisterOplysning.Length);
        }

        [Test]
        [TestCaseSource(typeof(TestData), TestData.CprNumbersFieldName)]
        public void ReadPeriod(string pnr)
        {
            var uuid = GetUuid(pnr);
            var person = TestRunner.PartService.ReadPeriod(new LaesPeriodInputType() { UUID = uuid, VirkningFraDato = DateTime.Today.AddYears(-5), VirkningTilDato = DateTime.Today });
            ValidateSingleReturn(person);
            Validate(new Guid(uuid), person, TestRunner.PartService);
            Assert.AreEqual(1, (person.LaesResultat.Item as FiltreretOejebliksbilledeType).AttributListe.Egenskab.Length);
            Assert.AreEqual(1, (person.LaesResultat.Item as FiltreretOejebliksbilledeType).AttributListe.RegisterOplysning.Length);
        }

        [Test]
        public void ListSnapshot()
        {
            var uuids = GetUuids(TestData.cprNumbers);
            var person = TestRunner.PartService.ListSnapshot(new ListOejebliksbilledeInputType { UUID = uuids, VirkningDato = DateTime.Today });
            ValidateListReturn(person);
            for (int i = 0; i < uuids.Length; i++)
            {
                var o = person.LaesResultat[i];
                Validate(new Guid(uuids[i]), person.LaesResultat[i], TestRunner.PartService);
                Assert.AreEqual(1, (o.Item as FiltreretOejebliksbilledeType).AttributListe.Egenskab.Length);
                Assert.AreEqual(1, (o.Item as FiltreretOejebliksbilledeType).AttributListe.RegisterOplysning.Length);
            }
        }

        [Test]
        public void ListPeriod()
        {
            var uuids = GetUuids(TestData.cprNumbers);
            var person = TestRunner.PartService.ListPeriod(new ListPeriodInputType { UUID = uuids, VirkningFraDato = DateTime.Today.AddYears(-5), VirkningTilDato = DateTime.Today });
            ValidateListReturn(person);
            for (int i = 0; i < uuids.Length; i++)
            {
                var o = person.LaesResultat[i];
                Validate(new Guid(uuids[i]), person.LaesResultat[i], TestRunner.PartService);
                Assert.AreEqual(1, (o.Item as FiltreretOejebliksbilledeType).AttributListe.Egenskab.Length);
                Assert.AreEqual(1, (o.Item as FiltreretOejebliksbilledeType).AttributListe.RegisterOplysning.Length);
            }
        }

        public void ValidateSingleReturn(LaesOutputType ret)
        {
            Assert.IsNotNull(ret);
            System.Console.WriteLine("Error message: " + ret.StandardRetur.FejlbeskedTekst);
            base.Validate(ret.StandardRetur.StatusKode, ret.StandardRetur.FejlbeskedTekst);
        }

        public void ValidateListReturn(ListOutputType1 ret)
        {
            Assert.IsNotNull(ret);
            System.Console.WriteLine("Error message: " + ret.StandardRetur.FejlbeskedTekst);
            base.Validate(ret.StandardRetur.StatusKode, ret.StandardRetur.FejlbeskedTekst);
        }
    }
}
