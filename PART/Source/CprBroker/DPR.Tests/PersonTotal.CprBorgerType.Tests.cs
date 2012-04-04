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
using CprBroker.Providers.DPR;
using CprBroker.Utilities;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.DPR.PersonTotalTests
{
    [TestFixture]
    public class ToCprBorgerType : BaseTests
    {

        [Test]
        public void ToCprBorgerType_Normal_NotNull()
        {
            var personTotal = new PersonTotalStub();
            var result = personTotal.ToCprBorgerType(null, null);
            Assert.NotNull(result);
        }

        [Test]
        public void ToCprBorgerType_Normal_CorrectPnr(
            [ValueSource("RandomCprNumbers5")]decimal cprNumber)
        {
            var personTotal = new PersonTotalStub() { PNR = cprNumber };
            var result = personTotal.ToCprBorgerType(null, null);
            Assert.AreEqual(cprNumber, decimal.Parse(result.PersonCivilRegistrationIdentifier));
        }

        [Test]
        public void ToCprBorgerType_Normal_NullAdresseNoteTekst()
        {
            var personTotal = new PersonTotalStub();
            var result = personTotal.ToCprBorgerType(null, null);
            Assert.IsNullOrEmpty(result.AdresseNoteTekst);
        }

        [Test]
        public void ToCprBorgerType_NoAddress_NullAddress()
        {
            var personTotal = new PersonTotalStub();
            var result = personTotal.ToCprBorgerType(null, null);
            Assert.Null(result.FolkeregisterAdresse);
        }

        [Test]
        public void ToCprBorgerType_WithAddress_AddressNotNull()
        {
            var personTotal = new PersonTotalStub();
            var result = personTotal.ToCprBorgerType(null, new PersonAddress());
            Assert.NotNull(result.FolkeregisterAdresse);
        }

        [Test]
        public void ToCprBorgerType_DirProtection_CorrectResult(
            [Values(null, '1')]char? dirProtection)
        {
            var personTotal = new PersonTotalStub() { DirectoryProtectionMarker = dirProtection };
            var result = personTotal.ToCprBorgerType(null, null);
            Assert.AreEqual(personTotal.ToDirectoryProtectionIndicator(), result.ForskerBeskyttelseIndikator);
        }

        [Test]
        public void ToCprBorgerType_Nationality_CorrectResult(
            [ValueSource("RandomCountryCodes5")]decimal countryCode)
        {
            var personTotal = new PersonTotalStub();
            var result = personTotal.ToCprBorgerType(new Nationality() { CountryCode = countryCode }, null);
            Assert.AreEqual(countryCode, decimal.Parse(result.PersonNationalityCode.Value));
        }

        [Test]
        public void ToCprBorgerType_CivilRegState_CorrectPNRValidity(
            [ValueSource("AllCivilRegistrationStates")] decimal status)
        {
            var personTotal = new PersonTotal() { Status = status };
            var result = personTotal.ToCprBorgerType(null, null);
            Assert.AreEqual(personTotal.ToCivilRegistrationValidityStatusIndicator(), result.PersonNummerGyldighedStatusIndikator);
        }

        [Test]
        public void ToCprBorgerType_Church_CorrectMembership(
            [Values(null, '1')] char? christianMark)
        {
            var personTotal = new PersonTotalStub() { ChristianMark = christianMark };
            var result = personTotal.ToCprBorgerType(null, null);
            Assert.AreEqual(personTotal.ToChurchMembershipIndicator(), result.FolkekirkeMedlemIndikator);
        }

        [Test]
        public void ToCprBorgerType_Normal_TelephoneNumberProtectionIndicatorIsFalse()
        {
            // TODO: Add more cases for values of other data protection types
            var personTotal = new PersonTotalStub();
            var result = personTotal.ToCprBorgerType(null, null);
            Assert.False(result.TelefonNummerBeskyttelseIndikator);
        }

    }
}
