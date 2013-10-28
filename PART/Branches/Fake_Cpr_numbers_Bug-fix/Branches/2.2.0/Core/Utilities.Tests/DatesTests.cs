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
using CprBroker.Utilities;

namespace CprBroker.Tests.Utilities
{
    [TestFixture]
    public class DatesTests
    {
        #region DateRangeInclude

        private DateTime[] StartDates = new DateTime[] { new DateTime(2010, 1, 1), new DateTime(2010, 12, 31) };
        private DateTime[] EffectDates = new DateTime[] { new DateTime(2011, 1, 1), new DateTime(2011, 12, 31) };
        private DateTime[] EndDates = new DateTime[] { new DateTime(2012, 2, 28), new DateTime(2012, 12, 31) };

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void DateRangeIncludes_Reversed_ThrowsException(
            [ValueSource("EndDates")] DateTime? startDate,
            [ValueSource("StartDates")] DateTime? endDate,
            [ValueSource("EffectDates")] DateTime effectDate,
            [Values(true, false)] bool allowOpen)
        {
            CprBroker.Utilities.Dates.DateRangeIncludes(startDate, endDate, effectDate, allowOpen);
        }

        [Test]
        public void DateRangeIncludes_Correct_ReturnsTrue(
            [ValueSource("StartDates")] DateTime? startDate,
            [ValueSource("EndDates")] DateTime? endDate,
            [ValueSource("EffectDates")] DateTime effectDate,
            [Values(true, false)] bool allowOpen)
        {
            var result = CprBroker.Utilities.Dates.DateRangeIncludes(startDate, endDate, effectDate, allowOpen);
            Assert.True(result);
        }

        [Test]
        public void DateRangeIncludes_AfterEnd_ReturnsFalse(
            [ValueSource("StartDates")] DateTime? startDate,
            [ValueSource("EndDates")] DateTime? endDate,
            [Values(true, false)] bool allowOpen)
        {
            var effectDate = endDate.Value.AddDays(1);
            var result = CprBroker.Utilities.Dates.DateRangeIncludes(startDate, endDate, effectDate, allowOpen);
            Assert.False(result);
        }

        [Test]
        public void DateRangeIncludes_BeforeStart_ReturnsFalse(
            [ValueSource("StartDates")] DateTime? startDate,
            [ValueSource("EndDates")] DateTime? endDate,
            [Values(true, false)] bool allowOpen)
        {
            var effectDate = startDate.Value.AddDays(-1);
            var result = CprBroker.Utilities.Dates.DateRangeIncludes(startDate, endDate, effectDate, allowOpen);
            Assert.False(result);
        }

        [Test]
        public void DateRangeIncludes_NullStartWNotAllowed_ReturnsFalse(
            [ValueSource("EndDates")] DateTime? endDate,
            [ValueSource("EffectDates")] DateTime effectDate
            )
        {
            var result = CprBroker.Utilities.Dates.DateRangeIncludes(null, endDate, effectDate, false);
            Assert.False(result);
        }

        [Test]
        public void DateRangeIncludes_NullStartWAllowed_ReturnsTrue(
            [ValueSource("EndDates")] DateTime? endDate,
            [ValueSource("EffectDates")] DateTime effectDate
            )
        {
            var result = CprBroker.Utilities.Dates.DateRangeIncludes(null, endDate, effectDate, true);
            Assert.True(result);
        }

        [Test]
        public void DateRangeIncludes_NullEndWAllowed_ReturnsTrue(
            [ValueSource("StartDates")] DateTime? startDate,
            [ValueSource("EffectDates")] DateTime effectDate
            )
        {
            var result = CprBroker.Utilities.Dates.DateRangeIncludes(startDate, null, effectDate, true);
            Assert.True(result);
        }

        [Test]
        public void DateRangeIncludes_NullAllWAllowed_ReturnsTrue(
            [ValueSource("EffectDates")] DateTime effectDate
            )
        {
            var result = CprBroker.Utilities.Dates.DateRangeIncludes(null, null, effectDate, true);
            Assert.True(result);
        }

        [Test]
        public void DateRangeIncludes_NullAllWNotAllowed_ReturnsFalse(
            [ValueSource("EffectDates")] DateTime effectDate
            )
        {
            var result = CprBroker.Utilities.Dates.DateRangeIncludes(null, null, effectDate, false);
            Assert.False(result);
        }


        #endregion
    }
}
