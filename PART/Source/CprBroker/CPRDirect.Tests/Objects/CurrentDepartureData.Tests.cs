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
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect.Objects
{
    namespace CurrentDepartureDataTests
    {
        [TestFixture]
        public class IsEmpty
        {
            [Test]
            public void IsEmpty_Nothing_True()
            {
                var db = new CurrentDepartureDataType();
                var ret = db.IsEmpty;
                Assert.True(ret);
            }

            [Test]
            public void IsEmpty_Dates_True()
            {
                var db = new CurrentDepartureDataType() { ExitDate = DateTime.Today, PNR = "1234567890" };
                var ret = db.IsEmpty;
                Assert.True(ret);
            }

            [Test]
            public void IsEmpty_Lines_False()
            {
                var db = new CurrentDepartureDataType() { ForeignAddress1 = "DDD" };
                var ret = db.IsEmpty;
                Assert.False(ret);
            }
        }

        [TestFixture]
        public class ToCountryIdentificationCodeType
        {
            [Test]
            public void ToCountryIdentificationCodeType_Code_imk(
                [Values(1, 3, 56)]int countryCode)
            {
                var db = new CurrentDepartureDataType() { ExitCountryCode = countryCode };
                var ret = db.ToCountryIdentificationCode();
                Assert.AreEqual(_CountryIdentificationSchemeType.imk, ret.scheme);
            }

            [Test]
            public void ToCountryIdentificationCodeType_Code_CorrectCode(
                [Values(1, 3, 56)]int countryCode)
            {
                var db = new CurrentDepartureDataType() { ExitCountryCode = countryCode };
                var ret = db.ToCountryIdentificationCode();
                Assert.AreEqual(countryCode, decimal.Parse(ret.Value));
            }
        }

        [TestFixture]
        public class ToAdresseType
        {
            [Test]
            public void ToAdresseType_Country_VerdenAdress()
            {
                var db = new CurrentDepartureDataType() { ExitCountryCode = 22 };
                var ret = db.ToAdresseType();
                Assert.IsInstanceOf<VerdenAdresseType>(ret.Item);
            }
        }

        [TestFixture]
        public class ToVerdenAdresseType
        {
            [Test]
            public void ToVerdenAdresseType_Empty_ForeignNotNull()
            {
                var db = new CurrentDepartureDataType() { ExitCountryCode = 1 };
                var ret = db.ToVerdenAdresseType();
                Assert.NotNull(ret.ForeignAddressStructure);
            }

            [Test]
            public void ToVerdenAdresseType_EmptyAndValues_CorrectUkendt(
                [Values(null, "line1")]string line1)
            {
                var db = new CurrentDepartureDataType() { ForeignAddress1 = line1, ExitCountryCode = 1 };
                var ret = db.ToVerdenAdresseType();
                Assert.AreEqual(db.IsEmpty, ret.UkendtAdresseIndikator);
            }
        }
    }
}
