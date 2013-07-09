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

namespace CprBroker.Tests.CPRDirect.Tools
{
    namespace ConvertersTests
    {
        [TestFixture]
        public class DecimalToString
        {
            [Test]
            public void DecimalToString_Misc_CorrectResult(
                [Values(0, 10, 20)] decimal value,
                [Values(15, 7, 9)]int length
                )
            {
                var ss = Converters.DecimalToString(value, length);
                Assert.AreEqual(length, ss.Length);
            }

            [Test]
            [ExpectedException(typeof(ArgumentOutOfRangeException))]
            public void DecimalToString_ShorterLength_CorrectResult(
                [Values(150, 1022, 2213)] decimal value,
                [Values(0, 1, 2)]int length
                )
            {
                var ss = Converters.DecimalToString(value, length);
                Assert.AreEqual(length, ss.Length);
            }
        }

        [TestFixture]
        public class ToDateTime
        {
            [Test]
            public void ToDateTime_BlankFlag_NotNull()
            {
                var ret = Converters.ToDateTime(DateTime.Today, ' ');
                Assert.NotNull(ret);
            }

            [Test]
            public void ToDateTime_OtherFlag_Null(
                [Values('w', '-', ',', '1', 'u', 'M')]char uncertainty)
            {
                var ret = Converters.ToDateTime(DateTime.Today, uncertainty);
                Assert.Null(ret);
            }

            [Test]
            public void ToDateTime_EmptyDate_MiscUncertainty_Null(
                [Values('w', '-', ',', '1', 'u', 'M')]char uncertainty)
            {
                var ret = Converters.ToDateTime(new DateTime(), uncertainty);
                Assert.Null(ret);
            }
            [Test]
            public void ToDateTime_TodayDate_MiscUncertainty_Null(
                [Values('w', '-', ',', '1', 'u', 'M')]char uncertainty)
            {
                var ret = Converters.ToDateTime(DateTime.Today, uncertainty);
                Assert.Null(ret);
            }
        }

        [TestFixture]
        public class ToPnrStringOrNull
        {
            [Test]
            public void ToPnrStringOrNull_InvalidPnr_Null(
                [Values(null, "kjkjkl", "987889789999", "123456789011", "123456", "0012345678", "  12345678")]string pnr)
            {
                var ret = Converters.ToPnrStringOrNull(pnr);
                Assert.Null(ret);
            }

            [Test]
            public void ToPnrStringOrNull_Valid_Correct(
                [Values("1234567890", "0123456789", "123456789")]string pnr)
            {
                var ret = Converters.ToPnrStringOrNull(pnr);
                Assert.AreEqual(decimal.Parse(pnr), decimal.Parse(ret));
            }
        }

    }
}