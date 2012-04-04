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
    public class ToUdenlandskBorgerType : BaseTests
    {

        #region ToUdenlandskBorgerType
        [Test]
        public void ToUdenlandskBorgerType_Normal_NotNull(
            [ValueSource("RandomCprNumbers5")]decimal cprNumber)
        {
            var personTotal = new PersonTotal() { PNR = cprNumber };
            var result = personTotal.ToUdenlandskBorgerType(new Nationality());
            Assert.NotNull(result);
        }

        [Test]
        public void ToUdenlandskBorgerType_Normal_CorrectPnr(
            [ValueSource("RandomCprNumbers5")]decimal cprNumber)
        {
            var personTotal = new PersonTotal() { PNR = cprNumber };
            var result = personTotal.ToUdenlandskBorgerType(new Nationality());
            Assert.AreEqual(cprNumber, decimal.Parse(result.PersonCivilRegistrationReplacementIdentifier));
        }

        [Test]
        public void ToUdenlandskBorgerType_Normal_CorrectNationality(
            [ValueSource("RandomCountryCodes5")]decimal countryCode)
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerType(new Nationality() { CountryCode = countryCode });
            Assert.AreEqual(countryCode, decimal.Parse(result.PersonNationalityCode[0].Value));
        }

        [Test]
        public void ToUdenlandskBorgerType_Normal_NullBirthCountry()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerType(new Nationality());
            Assert.Null(result.FoedselslandKode);
        }

        [Test]
        public void ToUdenlandskBorgerType_Normal_NullPersonIdentificator()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerType(new Nationality());
            Assert.IsNullOrEmpty(result.PersonIdentifikator);
        }

        [Test]
        public void ToUdenlandskBorgerType_Normal_EmptyLanguages()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerType(new Nationality());
            Assert.IsEmpty(result.SprogKode);
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void ToUdenlandskBorgerType_NullNationality_Exception(
            [ValueSource("RandomCprNumbers5")]decimal cprNumber)
        {
            var personTotal = new PersonTotal() { PNR = cprNumber };
            personTotal.ToUdenlandskBorgerType(null);
        }


        #endregion

        #region ToUdenlandskBorgerTypeVirvning
        [Test]
        public void ToUkendtBorgerTypVirvninge_Empty_NotNull()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.NotNull(result);
        }

        [Test]
        public void ToUkendtBorgerTypVirvninge_Empty_EmptyResultFra()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.Null(result.FraTidspunkt.ToDateTime());
        }

        [Test]
        public void ToUkendtBorgerTypVirvninge_Empty_EmptyResultTil()
        {
            var personTotal = new PersonTotal();
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.Null(result.TilTidspunkt.ToDateTime());
        }

        [Test]
        public void ToUdenlandskBorgerTypeVirvning_Normal_CorrectDate(
            [ValueSource("RandomDecimalDates5")]decimal statusDate)
        {
            var personTotal = new PersonTotal() { StatusDate = statusDate };
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.AreEqual(Providers.DPR.Utilities.DateFromDecimal(statusDate), result.FraTidspunkt.Item);
        }

        [Test]
        public void ToUdenlandskBorgerTypeVirvning_Normal_EmptyResultTil(
            [ValueSource("RandomDecimalDates5")]decimal statusDate)
        {
            var personTotal = new PersonTotal() { StatusDate = statusDate };
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.Null(result.TilTidspunkt.ToDateTime());
        }

        [Test]
        public void ToUdenlandskBorgerTypeVirvning_MiscDates_CorrectDate(
            [ValueSource("RandomDecimalDates5")]decimal statusDate,
            [ValueSource("RandomDecimalDates5")]decimal otherDate)
        {
            var personTotal = new PersonTotal() { StatusDate = statusDate, AddressDate = otherDate, DateOfBirth = otherDate, MaritalStatusDate = otherDate, MunicipalityArrivalDate = otherDate, MunicipalityLeavingDate = otherDate, PaternityDate = otherDate, PersonalSelectionDate = otherDate, UnderGuardianshipDate = otherDate, VotingDate = otherDate };
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.AreEqual(Providers.DPR.Utilities.DateFromDecimal(statusDate), result.FraTidspunkt.Item);
        }

        [Test]
        public void ToUdenlandskBorgerTypeVirvning_MiscDates_EmptyResultTil(
            [ValueSource("RandomDecimalDates5")]decimal statusDate,
            [ValueSource("RandomDecimalDates5")]decimal otherDate)
        {
            var personTotal = new PersonTotal() { StatusDate = statusDate, AddressDate = otherDate, DateOfBirth = otherDate, MaritalStatusDate = otherDate, MunicipalityArrivalDate = otherDate, MunicipalityLeavingDate = otherDate, PaternityDate = otherDate, PersonalSelectionDate = otherDate, UnderGuardianshipDate = otherDate, VotingDate = otherDate };
            var result = personTotal.ToUdenlandskBorgerTypeVirkning();
            Assert.Null(result.TilTidspunkt.ToDateTime());
        }
        #endregion
    }
}
