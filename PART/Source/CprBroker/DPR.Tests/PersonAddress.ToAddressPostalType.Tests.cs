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

namespace CprBroker.Tests.DPR.PersonAddressTests
{
    [TestFixture]
    public class ToAddressPostalType : BaseTests
    {
        [Test]
        public void ToAddressPostalType_Normal_NotNull()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToAddressPostalType(null);
            Assert.NotNull(result);
        }

        [Test]
        public void ToAddressPostalType_City_CorrectDistrictSubdivisionIdentifier(
            [ValueSource("RandomStrings5")]string cityName)
        {
            var personAddress = new PersonAddressStub() { Town = cityName };
            var result = personAddress.ToAddressPostalType(null);
            Assert.AreEqual(cityName, result.DistrictSubdivisionIdentifier);
        }

        [Test]
        public void ToAddressPostalType_Normal_DistrictSubdivisionIdentifierNull()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToAddressPostalType(null);
            Assert.Null(result.DistrictSubdivisionIdentifier);
        }

        [Test]
        public void ToAddressPostalType_Normal_CorrectFloor(
            [ValueSource("RandomStrings5")]string floor)
        {
            var personAddress = new PersonAddressStub() { Floor = floor };
            var result = personAddress.ToAddressPostalType(null);
            Assert.AreEqual(floor, result.FloorIdentifier);
        }

        [Test]
        public void ToAddressPostalType_Normal_MailDeliverySublocationIdentifierNull()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToAddressPostalType(null);
            Assert.Null(result.MailDeliverySublocationIdentifier);
        }

        [Test]
        public void ToAddressPostalType_Normal_CorrectPostCodeIdentifier(
            [ValueSource("RandomDecimals5")]decimal postCode)
        {
            var personAddress = new PersonAddressStub() { PostCode = postCode };
            var result = personAddress.ToAddressPostalType(null);
            Assert.AreEqual(postCode, decimal.Parse(result.PostCodeIdentifier));
        }

        [Test]
        public void ToAddressPostalType_Normal_PostOfficeBoxIdentifierNull()
        {
            var personAddress = new PersonAddressStub();
            var result = personAddress.ToAddressPostalType(null);
            Assert.Null(result.PostOfficeBoxIdentifier);
        }

        [Test]
        public void ToAddressPostalType_Normal_CorrectStreetBuildingIdentifier(
            [ValueSource("RandomHouseNumbers5")]string houseNumber)
        {
            var personAddress = new PersonAddressStub() { HouseNumber = houseNumber };
            var result = personAddress.ToAddressPostalType(null);
            Assert.AreEqual(houseNumber, result.StreetBuildingIdentifier);
        }

        [Test]
        public void ToAddressPostalType_Normal_CorrectStreetName(
            [ValueSource("RandomStrings5")]string streetName)
        {
            var personAddress = new PersonAddressStub() { StreetAddressingName = streetName };
            var result = personAddress.ToAddressPostalType(null);
            Assert.AreEqual(streetName, result.StreetName);
        }

        [Test]
        public void ToAddressPostalType_Normal_CorrectStreetAddressingName(
            [ValueSource("RandomStrings5")]string streetName)
        {
            var personAddress = new PersonAddressStub() { StreetAddressingName = streetName };
            var result = personAddress.ToAddressPostalType(null);
            Assert.AreEqual(streetName, result.StreetNameForAddressingName);
        }

        [Test]
        public void ToAddressPostalType_Normal_CorrectSuiteIdentifier(
            [ValueSource("RandomStrings5")]string door)
        {
            var personAddress = new PersonAddressStub() { DoorNumber = door };
            var result = personAddress.ToAddressPostalType(null);
            Assert.AreEqual(door, result.SuiteIdentifier);
        }
    }
}
