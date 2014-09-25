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
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.CPRDirect.Objects
{
    namespace ClearWrittenAddressTests
    {
        [TestFixture]
        public class Misc
        {
            [Test]
            public void CheckAddresses()
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var groupings = result.GroupBy(
                    person => new
                    {
                        //HasClearAddress = person.ClearWrittenAddress != null,
                        ValidClearAddress = !person.ClearWrittenAddress.IsEmpty,    // { related to each other }
                        CurrentAddress = person.CurrentAddressInformation != null,  // { related to each other }
                        ContactAddress = person.ContactAddress != null,
                        ForeignAddress = person.CurrentDepartureData != null,
                        ValidForeignAddress = person.CurrentDepartureData != null && !person.CurrentDepartureData.IsEmpty,
                        Status = person.PersonInformation.Status
                    })
                    //.Where(person => !person.Key.ValidClearAddress && !person.Key.ValidForeignAddress)
                    .OrderBy(g => g.Key.Status)
                    .Select(g => new { Key = g.Key, Value = g.ToArray() })
                    .ToArray();



                /* Findings
                 * If ClearWrittenAddress is empty, then CurrentAddressInformation is null (and vice versa)
                 * CurrentDepartureData can only contain value if CurrentAddressInformation is null
                 * Both CurrentAddressInformation and CurrentDepartureData is null if Status is 50,60,70 0r 90
                 * CurrentDepartureData (if not null) never contains empty values
                 */

                var p = result
                    .GroupBy(r => new { Empty = r.ClearWrittenAddress.IsEmpty })
                    .Select(g => new { Key = g.Key, Values = g.ToArray() })
                    .ToArray();
                object o = "";

            }

        }

        [TestFixture]
        public class ToAddressPostalType
        {
            [Test]
            public void ToAddressPostalType_Normal_DanishCountryCode()
            {
                var db = new ClearWrittenAddressType() { PostCode = 0 };
                var ret = db.ToAddressPostalType();
                Assert.AreEqual("5100", ret.CountryIdentificationCode.Value);
            }
        }

        [TestFixture]
        public class IsEmpty
        {
            [Test]
            public void IsEmpty_Empty_True()
            {
                var db = new ClearWrittenAddressType() { PostCode = 0, MunicipalityCode = 0, StreetCode = 0 };
                var ret = db.IsEmpty;
                Assert.True(ret);
            }

            [Test]
            public void IsEmpty_StreetCode_False()
            {
                var db = new ClearWrittenAddressType() { PostCode = 0, MunicipalityCode = 0, StreetCode = 10 };
                var ret = db.IsEmpty;
                Assert.False(ret);
            }
        }

        [TestFixture]
        public class ToDanskAdresseType
        {
            [Test]
            public void ToDanskAdresseType_PostCodeValueOrNull_Ukendt(
                [Values(0, 10)] int postCode)
            {
                var db = new ClearWrittenAddressType() { PostCode = postCode, MunicipalityCode = 0, StreetCode = 0 };
                var ret = db.ToDanskAdresseType();
                Assert.AreEqual(db.IsEmpty, ret.UkendtAdresseIndikator);
            }
        }

        [TestFixture]
        public class ToAdresseType
        {
            [Test]
            public void ToAdresseType_Empty_DanishAddress()
            {
                var db = new ClearWrittenAddressType() { PostCode = 0, MunicipalityCode = 0, StreetCode = 0 };
                var ret = db.ToAdresseType();
                Assert.IsInstanceOf<DanskAdresseType>(ret.Item);
            }

            [Test]
            public void ToAdresseType_HighKomKod_GreenlandicAddress()
            {
                var db = new ClearWrittenAddressType() { PostCode = 0, MunicipalityCode = 999, StreetCode = 0 };
                var ret = db.ToAdresseType();
                Assert.IsInstanceOf<GroenlandAdresseType>(ret.Item);
            }
        }
    }
}
