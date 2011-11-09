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
