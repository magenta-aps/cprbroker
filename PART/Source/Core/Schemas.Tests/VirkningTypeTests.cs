using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas.Part;
using NUnit.Framework;

namespace CprBroker.Tests.Schemas
{
    [TestFixture]
    class VirkningTypeTests
    {

        public VirkningTypeTests()
        {
            DateTime today = DateTime.Today;
            PastDates = new DateTime?[] { today.AddYears(-3), today.AddMonths(-3), today.AddDays(-3), today.AddHours(-3), today.AddMinutes(-3), today.AddSeconds(-3), today.AddMilliseconds(-3) };
            FutureDates = new DateTime?[] { today.AddYears(3), today.AddMonths(3), today.AddDays(3), today.AddHours(3), today.AddMinutes(3), today.AddSeconds(3), today.AddMilliseconds(3) };
            PastDatesWithNull = new DateTime?[] { null, today }.Union(PastDates).ToArray();
            FutureDatesWithNull = new DateTime?[] { null, today }.Union(FutureDates).ToArray();
        }

        DateTime?[] PastDates = null;
        DateTime?[] FutureDates = null;

        DateTime?[] PastDatesWithNull = null;
        DateTime?[] FutureDatesWithNull = null;


        [Test]
        [Combinatorial]
        public void Compose_Valid_NotNull(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate,
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var result = VirkningType.Create(fromDate, toDate);
            Assert.NotNull(result);
        }

        [Test]
        [Combinatorial]
        public void Compose_Valid_EqualFromDate(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate,
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var result = VirkningType.Create(fromDate, toDate);
            Assert.AreEqual(fromDate, result.FraTidspunkt.ToDateTime());
        }

        [Test]
        [Combinatorial]
        public void Compose_Valid_EqualToDate(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate,
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var result = VirkningType.Create(fromDate, toDate);
            Assert.AreEqual(toDate, result.TilTidspunkt.ToDateTime());
        }

        [Test]
        [Combinatorial]
        public void Compose_Valid_AktoerRefNull(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate,
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var result = VirkningType.Create(fromDate, toDate);
            Assert.Null(result.AktoerRef);
        }

        [Test]
        [Combinatorial]
        public void Compose_Valid_CommentTextNull(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate,
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var result = VirkningType.Create(fromDate, toDate);
            Assert.IsNullOrEmpty(result.CommentText);
        }

        [Test]
        [Combinatorial]
        [ExpectedException(typeof(ArgumentException))]
        public void Compose_InconsistentDates_ThrowsException(
            [ValueSource("FutureDates")] DateTime? fromDate,
            [ValueSource("PastDates")] DateTime? toDate)
        {
            VirkningType.Create(fromDate, toDate);
        }

    }
}
