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
using CprBroker.Providers.ServicePlatform;
using CprBroker.Providers.ServicePlatform.Responses;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.ServicePlatform
{
    namespace StamPlusResponseTests
    {
        public class StamPlusTests : BaseResponseTests
        {
            public StamPlusResponse GetResponse(string pnr)
            {
                var txt = GetResponse(pnr, "Stam+");
                var w = new StamPlusResponse(txt);
                return w;
            }
        }

        public class StamPlusTests_Concrete : StamPlusTests
        {
            [Test]
            [TestCaseSource("PNRs")]
            public void Response_OneRow(string pnr)
            {
                var resp = GetResponse(pnr);
                Assert.AreEqual(1, resp.RowItems.Count());
            }
        }

        [TestFixture]
        public class ToAttributListTests : StamPlusTests
        {
            public AttributListeType GetAttributes(string pnr)
            {
                return GetResponse(pnr).ToAttributListeType();
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

            bool HasNoName(string pnr)
            {
                return new string[] { "0101005038", "3006980014" }.Contains(pnr);
            }

            [Test]
            [TestCaseSource("PNRs")]
            public void ToAttributList_AddressingNameNotEmpty(string pnr)
            {
                if (HasNoName(pnr))
                    return;
                var name = GetName(pnr);
                Assert.IsNotEmpty(name.PersonNameForAddressingName);
            }

            [Test]
            [TestCaseSource("PNRs")]
            public void ToAttributList_NameNotEmpty(string pnr)
            {
                if (HasNoName(pnr))
                    return;
                var name = GetName(pnr);
                Assert.False(name.PersonNameStructure.IsEmpty);
            }
        }

        [TestFixture]
        public class ToLivStatusTypeTests : StamPlusTests
        {
            [Test]
            [TestCaseSource("PNRs")]
            public void ToLivStatusType_NotNull(string pnr)
            {
                var liv = GetResponse(pnr).RowItems.First().ToLivStatusType();
                Assert.NotNull(liv);
            }

            [Test]
            public void ToLivStatusType_AtLeastOneHasStartDate()
            {
                var liv = this.PNRs
                    .Select(pnr => GetResponse(pnr).RowItems.First().ToLivStatusType().TilstandVirkning.ToVirkningType().FraTidspunkt.ToDateTime())
                    .Where(d => d.HasValue)
                    .Count();

                Assert.Greater(liv, 0);
            }

            [Test]
            [TestCase(LivStatusKodeType.Doed)]
            [TestCase(LivStatusKodeType.Foedt)]
            [TestCase(LivStatusKodeType.Forsvundet)]
            [TestCase(LivStatusKodeType.Prenatal, Ignore = true)]
            public void ToLivStatusType_AtLeastOneOfType(LivStatusKodeType statusCode)
            {
                var liv = this.PNRs
                    .Select(pnr => GetResponse(pnr).RowItems.First().ToLivStatusType().LivStatusKode)
                    .Where(code => code == statusCode)
                    .Count();

                Console.WriteLine("{0} {1}", statusCode, liv);
                Assert.Greater(liv, 0);
            }
        }

    }
}
