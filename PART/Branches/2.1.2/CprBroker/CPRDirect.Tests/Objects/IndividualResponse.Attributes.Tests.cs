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

namespace CprBroker.Tests.CPRDirect.Objects
{
    namespace IndividualResponse.Attributes
    {
        [TestFixture]
        public class ToBirthDate
        {
            [Test]
            [ExpectedException]
            public void ToBirthDate_NullInformation_Exception()
            {
                var info = new IndividualResponseType();
                info.ToBirthDate();
            }

            [Test]
            public void ToBirthDate_Birthdate_OK()
            {
                var info = new IndividualResponseType() { PersonInformation = new PersonInformationType() { Birthdate = DateTime.Today, BirthdateUncertainty = ' ' } };
                var ret = info.ToBirthDate();
                Assert.AreEqual(DateTime.Today, ret);
            }

            [Test]
            public void ToBirthDate_NoBirthdate_FromPnr()
            {
                var info = new IndividualResponseType()
                {
                    PersonInformation = new PersonInformationType()
                    {
                        PNR = DateTime.Today.ToString("ddMMyy4111")
                    }
                };
                var ret = info.ToBirthDate();
                Assert.AreEqual(DateTime.Today, ret);
            }
        }

        [TestFixture]
        public class GetFolkeregisterAdresseSource
        {
            [Test]
            public void GetFolkeregisterAdresseSource_Empty_Dummy()
            {
                var db = new IndividualResponseType();
                var ret = db.GetFolkeregisterAdresseSource(true);
                Assert.IsInstanceOf<DummyAddressSource>(ret);
            }

            [Test]
            public void GetFolkeregisterAdresseSource_ClearAddress_Dummy()
            {
                var db = new IndividualResponseType() { ClearWrittenAddress = new ClearWrittenAddressType() { MunicipalityCode = 0, PostCode = 0, StreetCode = 0 } };
                var ret = db.GetFolkeregisterAdresseSource(true);
                Assert.IsInstanceOf<DummyAddressSource>(ret);
            }

            [Test]
            public void GetFolkeregisterAdresseSource_ClearAddressAndAddress_Dummy()
            {
                var db = new IndividualResponseType() { ClearWrittenAddress = new ClearWrittenAddressType() { MunicipalityCode = 0, PostCode = 0, StreetCode = 0 }, CurrentAddressInformation = new CurrentAddressInformationType() };
                var ret = db.GetFolkeregisterAdresseSource(true);
                Assert.IsInstanceOf<DummyAddressSource>(ret);
            }

            [Test]
            public void GetFolkeregisterAdresseSource_ClearAddressWithAndAddress_AddressWrapper()
            {
                var db = new IndividualResponseType() { ClearWrittenAddress = new ClearWrittenAddressType() { MunicipalityCode = 0, PostCode = 12, StreetCode = 0 }, CurrentAddressInformation = new CurrentAddressInformationType() }; ;
                var ret = db.GetFolkeregisterAdresseSource(true);
                Assert.IsInstanceOf<CurrentAddressWrapper>(ret);
            }

            [Test]
            public void GetFolkeregisterAdresseSource_ClearAddressWithEmptyDeparture_Dummy()
            {
                var db = new IndividualResponseType() { ClearWrittenAddress = new ClearWrittenAddressType() { MunicipalityCode = 0, PostCode = 12, StreetCode = 0 }, CurrentDepartureData = new CurrentDepartureDataType() }; ;
                var ret = db.GetFolkeregisterAdresseSource(true);
                Assert.IsInstanceOf<DummyAddressSource>(ret);
            }

            [Test]
            public void GetFolkeregisterAdresseSource_ClearAddressWithDeparture_Departure()
            {
                var db = new IndividualResponseType() { ClearWrittenAddress = new ClearWrittenAddressType() { MunicipalityCode = 0, PostCode = 12, StreetCode = 0 }, CurrentDepartureData = new CurrentDepartureDataType() { ForeignAddress1 = "DDD" } };
                var ret = db.GetFolkeregisterAdresseSource(true);
                Assert.IsInstanceOf<CurrentDepartureDataType>(ret);
            }

        }

        [TestFixture]
        public class ToFoedestedNavn
        {
            [Test]
            public void ToFoedestedNavn_NameWithNoDateOrHistory_Null()
            {
                var pnr = Utilities.RandomCprNumberString();
                var res = new IndividualResponseType()
                {
                    PersonInformation = new PersonInformationType() { PNR = pnr, Birthdate = CprBroker.Utilities.Strings.PersonNumberToDate(pnr) },
                    CurrentNameInformation = new CurrentNameInformationType() { PNR = pnr, FirstName_s = "1", LastName = "2" }
                };
                var ret = res.ToFoedestedNavn();
                Assert.IsNullOrEmpty(ret);
            }

            [Test]
            public void ToFoedestedNavn_NameWithDate_NotNull(
                [Values(0, 1, 2, 10, 14)]int dayOffset)
            {
                string pnr = Utilities.RandomCprNumberString();
                DateTime birthDate = CprBroker.Utilities.Strings.PersonNumberToDate(pnr).Value;
                DateTime nameDate = birthDate.AddDays(dayOffset);
                var res = new IndividualResponseType()
                {
                    PersonInformation = new PersonInformationType() { PNR = pnr, Birthdate = birthDate },
                    CurrentNameInformation = new CurrentNameInformationType() { PNR = pnr, FirstName_s = "1", LastName = "2", NameStartDate = nameDate }
                };

                var ret = res.ToFoedestedNavn();
                Assert.IsNotNullOrEmpty(ret);
            }

            [Test]
            public void ToFoedestedNavn_NameWithFarDate_Null(
                [Values(15, 20, 30)]int dayOffset)
            {
                string pnr = Utilities.RandomCprNumberString();
                DateTime birthDate = CprBroker.Utilities.Strings.PersonNumberToDate(pnr).Value;
                DateTime nameDate = birthDate.AddDays(dayOffset);
                var res = new IndividualResponseType()
                {
                    PersonInformation = new PersonInformationType() { PNR = pnr, Birthdate = birthDate },
                    CurrentNameInformation = new CurrentNameInformationType() { PNR = pnr, FirstName_s = "1", LastName = "2", NameStartDate = nameDate }
                };

                var ret = res.ToFoedestedNavn();
                Assert.IsNullOrEmpty(ret);
            }

        }
    }
}