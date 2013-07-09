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
    namespace CurrentCivilStatusTests
    {
        [TestFixture]
        public class ToCivilStatusType
        {
            [Test]
            public void ToCivilStatusType_NoSeparation_StatusDate(
                [Values('D', 'E', 'F', 'G', 'L', 'O', 'P', 'U')]char maritalStatus)
            {
                var status = new CivilStatusWrapper(new CurrentCivilStatusType() { CivilStatusStartDate = DateTime.Today, CivilStatusStartDateUncertainty = ' ', CivilStatusCode = maritalStatus });
                var ret = status.ToCivilStatusType(null);
                Assert.AreEqual(DateTime.Today, ret.TilstandVirkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToCivilStatusType_MarriedWithSeparation_StatusDate(
                [Values('G', 'P')]char maritalStatus)
            {
                var status = new CivilStatusWrapper(new CurrentCivilStatusType() { CivilStatusStartDate = DateTime.Today.AddDays(-1), CivilStatusStartDateUncertainty = ' ', CivilStatusCode = maritalStatus });
                var sep = new CurrentSeparationType() { SeparationStartDate = DateTime.Today, SeparationStartDateUncertainty = ' ' };
                var ret = status.ToCivilStatusType(sep);
                Assert.AreEqual(sep.ToStartTS(), ret.TilstandVirkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToCivilStatusType_OtherThanMarriedWithSeparation_StatusDate(
                [Values('D', 'E', 'F', 'L', 'O', 'U')]char maritalStatus)
            {
                var status = new CivilStatusWrapper(new CurrentCivilStatusType() { CivilStatusStartDate = DateTime.Today.AddDays(-1), CivilStatusStartDateUncertainty = ' ', CivilStatusCode = maritalStatus });
                var sep = new CurrentSeparationType() { SeparationStartDate = DateTime.Today, SeparationStartDateUncertainty = ' ' };
                var ret = status.ToCivilStatusType(sep);
                Assert.AreEqual(status.ToCivilStatusDate(), ret.TilstandVirkning.FraTidspunkt.ToDateTime());
            }
        }

        [TestFixture]
        public class ToSpouses
        {
            [Test]
            public void ToSpouses_InvalidPNR_EmptyArray(
                [Values('U', 'G', 'F', 'E', 'P', 'O', 'L', 'D')]char maritalStatus,
                [Values(1, 20, 130, 12345678)]decimal pnr)
            {
                var civil = new CurrentCivilStatusType() { CivilStatusCode = maritalStatus, PNR = Converters.DecimalToString(pnr, 10) };
                var ret = CivilStatusWrapper.ToSpouses(civil, null, cpr => Guid.NewGuid());
                Assert.IsEmpty(ret);
            }
        }

        [TestFixture]
        public class ToPersonRelationTypeArray
        {
            [Test]
            public void ToPersonRelationTypeArray_Married_CorrectStartDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char civilStatus
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatusCode = civilStatus, CivilStatusStartDate = DateTime.Today, SpousePNR = Utilities.RandomCprNumberString() };
                var ret = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(), civilStatus, Utilities.RandomSingleChar(Utilities.AlphabetChars, civilStatus), Utilities.RandomSingleChar(Utilities.AlphabetChars, civilStatus));
                Assert.AreEqual(DateTime.Today, ret[0].Virkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonRelationTypeArray_Married_NullEndDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char civilStatus,
                [Values(true, false)]bool sameGenderForDead
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatusCode = civilStatus, CivilStatusStartDate = DateTime.Today, SpousePNR = Utilities.RandomCprNumberString() };
                var ret = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(), civilStatus, Utilities.RandomSingleChar(Utilities.AlphabetChars, civilStatus), Utilities.RandomSingleChar(Utilities.AlphabetChars, civilStatus));
                Assert.Null(ret[0].Virkning.TilTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonRelationTypeArray_UnMarried_CorrectEndDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char endedCivilStatus
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatusCode = endedCivilStatus, CivilStatusStartDate = DateTime.Today, SpousePNR = Utilities.RandomCprNumberString() };
                var ret = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(), Utilities.RandomSingleChar(Utilities.AlphabetChars, endedCivilStatus), endedCivilStatus, Utilities.RandomSingleChar(Utilities.AlphabetChars, endedCivilStatus));
                Assert.AreEqual(DateTime.Today, ret[0].Virkning.TilTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonRelationTypeArray_UnMarried_NullStartDate(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char endedCivilStatus,
                [Values(true, false)]bool sameGenderForDead
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatusCode = endedCivilStatus, CivilStatusStartDate = DateTime.Today, SpousePNR = Utilities.RandomCprNumberString() };
                var ret = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(), Utilities.RandomSingleChar(Utilities.AlphabetChars, endedCivilStatus), endedCivilStatus, Utilities.RandomSingleChar(Utilities.AlphabetChars, endedCivilStatus));
                Assert.Null(ret[0].Virkning.FraTidspunkt.ToDateTime());
            }

            [Test]
            public void ToPersonRelationTypeArray_DeadNoPnr_Empty(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char deadCivilStatus
                )
            {
                var civil = new CurrentCivilStatusType() { CivilStatusCode = deadCivilStatus, CivilStatusStartDate = DateTime.Today };
                var ret = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(), Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus), Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus), deadCivilStatus);
                Assert.IsEmpty(ret);
            }

            [Test]
            public void ToPersonRelationTypeArray_DeadWithPnr_NotEmpty(
                [ValueSource(typeof(Utilities), "AlphabetChars")]char deadCivilStatus
                )
            {
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var f = result.Where(r => r.CurrentCivilStatus.CivilStatusCode == 'D' || r.HistoricalCivilStatus.Where(c => c.CivilStatusCode == 'D').Count() > 0).ToArray(); ;
                var civil = new CurrentCivilStatusType() { CivilStatusCode = deadCivilStatus, CivilStatusStartDate = DateTime.Today, PNR = Utilities.RandomCprNumberString(), SpousePNR = Utilities.RandomCprNumberString() };
                var ret1 = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(),
                    Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus),
                    Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus),
                    Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus)
                    );
                var ret2 = CivilStatusWrapper.ToPersonRelationTypeArray(civil, null, pnr => Guid.NewGuid(), Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus), Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus), Utilities.RandomSingleChar(Utilities.AlphabetChars, deadCivilStatus));
                Assert.AreEqual(0, ret1.Length + ret2.Length);
            }

        }
    }
}
