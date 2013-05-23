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
    namespace PersonInformationTests
    {
        [TestFixture]
        public class LoadAll_
        {
            [Test]
            public void LoadAll()
            {
                var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var ss = all
                    .AsQueryable()
                    .GroupBy(p => new { Value = p.PersonInformation.Birthdate.HasValue, Certain = p.PersonInformation.BirthdateUncertainty })
                    .Select(g => new { Value = g.Key.Value, Certainty = g.Key.Certain, Data = g.ToArray() })
                    .ToArray();
                object dd = "";
            }
        }

        [TestFixture]
        public class ToBirthdate
        {
            [Test]
            public void ToBirthdate_Empty_Null()
            {
                var inf = new PersonInformationType();
                var ret = inf.ToBirthdate();
                Assert.Null(ret);
            }

            [Test]
            public void ToBirthdate_UncertainValue_Null(
                [Values('s', '2', '4', 'S')] char uncertainty)
            {
                var inf = new PersonInformationType() { Birthdate = DateTime.Today, BirthdateUncertainty = uncertainty };
                var ret = inf.ToBirthdate();
                Assert.Null(ret);
            }

            [Test]
            public void ToBirthdate_Value_OK()
            {
                var inf = new PersonInformationType() { Birthdate = DateTime.Today };
                var ret = inf.ToBirthdate();
                Assert.AreEqual(DateTime.Today, ret);
            }

            [Test]
            public void ToBirthDate_NoBirthdate_FromPnr()
            {
                var info = new PersonInformationType()
                    {
                        PNR = DateTime.Today.ToString("ddMMyy4111")
                    };
                var ret = info.ToBirthdate(true);
                Assert.AreEqual(DateTime.Today, ret);
            }
        }

        [TestFixture]
        public class ToStatusDate
        {
            [Test]
            public void ToStatusDate_Empty_Null()
            {
                var inf = new PersonInformationType();
                var ret = inf.ToStatusDate();
                Assert.Null(ret);
            }

            [Test]
            public void ToStatusDate_UncertainValue_Null(
                [Values('s', '2', '4', 'S')] char uncertainty)
            {
                var inf = new PersonInformationType() { StatusStartDate = DateTime.Today, StatusDateUncertainty = uncertainty };
                var ret = inf.ToStatusDate();
                Assert.Null(ret);
            }

            [Test]
            public void ToStatusDate_Value_OK()
            {
                var inf = new PersonInformationType() { StatusStartDate = DateTime.Today };
                var ret = inf.ToStatusDate();
                Assert.AreEqual(DateTime.Today, ret.Value);
            }
        }

        [TestFixture]
        public class ToPnr
        {
            [Test]
            public void ToPnr_Normal_OK(
                [Values("1234567890", "123456789")]string pnr)
            {
                var inf = new PersonInformationType() { PNR = pnr };
                var ret = inf.ToPnr();
                Assert.IsNotNullOrEmpty(ret);
            }

            [Test]
            public void ToPnr_Invalid_Emypty(
                [Values("12345678", "0", "224123", "12345", "123")]string pnr)
            {
                var inf = new PersonInformationType() { PNR = pnr };
                var ret = inf.ToPnr();
                Assert.IsNullOrEmpty(ret);
            }
        }

        [TestFixture]
        public class ToPersonGenderCodeType
        {
            [Test]
            public void PersonGenderCodeType_M_Male(
                [Values('M', 'm')]char gender)
            {
                var info = new PersonInformationType() { Gender = gender };
                var ret = info.ToPersonGenderCodeType();
                Assert.AreEqual(PersonGenderCodeType.male, ret);
            }

            [Test]
            public void PersonGenderCodeType_K_Female(
                [Values('K', 'k')]char gender)
            {
                var info = new PersonInformationType() { Gender = gender };
                var ret = info.ToPersonGenderCodeType();
                Assert.AreEqual(PersonGenderCodeType.female, ret);
            }

            [Test]
            [ExpectedException]
            public void PersonGenderCodeType_Other_Exception(
                [Values('s', ' ', '2')]char gender)
            {
                var info = new PersonInformationType() { Gender = gender };
                var ret = info.ToPersonGenderCodeType();
            }
        }

        [TestFixture]
        public class ToLivStatusType
        {
            [Test]
            public void ToLivStatusType_StatusWDate_Born(
                [Values(1, 3, 5, 50, 60)]decimal status)
            {
                var inf = new PersonInformationType() { Status = status, Birthdate = DateTime.Today };
                var res = inf.ToLivStatusType();
                Assert.AreEqual(LivStatusKodeType.Foedt, res.LivStatusKode);
            }

            [Test]
            public void ToLivStatusType_StatusWithoutDate_Prenatal(
                [Values(1, 3, 5, 50, 60)]decimal status)
            {
                var inf = new PersonInformationType() { Status = status };
                var res = inf.ToLivStatusType();
                Assert.AreEqual(LivStatusKodeType.Prenatal, res.LivStatusKode);
            }

            [Test]
            public void ToLivStatusType_Status_HasVirkning(
                [Values(1, 3, 5, 50, 60)]decimal status)
            {
                var inf = new PersonInformationType() { Status = status };
                var res = inf.ToLivStatusType();
                Assert.NotNull(res.TilstandVirkning);
            }

            [Test]
            public void ToLivStatusType_Status_EmptyDate(
                [Values(1, 3, 5, 50, 60)]decimal status)
            {
                var inf = new PersonInformationType() { Status = status };
                var res = inf.ToLivStatusType();
                Assert.Null(res.TilstandVirkning.FraTidspunkt.ToDateTime());
            }


        }
    }
}
