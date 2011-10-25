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
            AllDates = PastDates.Union(new DateTime?[] { today }).Union(FutureDates).ToArray();
            AllDatesWithNull = new DateTime?[] { today }.Union(AllDates).ToArray();
        }

        DateTime?[] PastDates = null;
        DateTime?[] FutureDates = null;

        DateTime?[] PastDatesWithNull = null;
        DateTime?[] FutureDatesWithNull = null;
        DateTime?[] AllDates = null;
        DateTime?[] AllDatesWithNull = null;


        #region Create
        [Test]
        [Combinatorial]
        public void Create_Valid_NotNull(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate,
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var result = VirkningType.Create(fromDate, toDate);
            Assert.NotNull(result);
        }

        [Test]
        [Combinatorial]
        public void Create_Valid_EqualFromDate(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate,
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var result = VirkningType.Create(fromDate, toDate);
            Assert.AreEqual(fromDate, result.FraTidspunkt.ToDateTime());
        }

        [Test]
        [Combinatorial]
        public void Create_Valid_EqualToDate(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate,
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var result = VirkningType.Create(fromDate, toDate);
            Assert.AreEqual(toDate, result.TilTidspunkt.ToDateTime());
        }

        [Test]
        [Combinatorial]
        public void Create_Valid_AktoerRefNull(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate,
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var result = VirkningType.Create(fromDate, toDate);
            Assert.Null(result.AktoerRef);
        }

        [Test]
        [Combinatorial]
        public void Create_Valid_CommentTextNull(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate,
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var result = VirkningType.Create(fromDate, toDate);
            Assert.IsNullOrEmpty(result.CommentText);
        }

        [Test]
        [Combinatorial]
        [ExpectedException(typeof(ArgumentException))]
        public void Create_InconsistentDates_ThrowsException(
            [ValueSource("FutureDates")] DateTime? fromDate,
            [ValueSource("PastDates")] DateTime? toDate)
        {
            VirkningType.Create(fromDate, toDate);
        }
        #endregion

        #region Compose
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Compose_Null_ThrowsException()
        {
            var result = VirkningType.Compose(null as VirkningType[]);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Compose_NullElement_ThrowsException()
        {
            var result = VirkningType.Compose(new VirkningType[] { null });
        }

        [Test]
        public void Compose_Valid_DoubleOpen()
        {
            var result = VirkningType.Compose(VirkningType.Create(null, null), VirkningType.Create(null, null));
            Assert.True(VirkningType.IsDoubleOpen(result));
        }

        [Test]
        [Combinatorial]
        public void Compose_ValidSingle_EqualsInput(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate,
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var virkning = VirkningType.Create(fromDate, toDate);
            var result = VirkningType.Compose(virkning);
            Assert.NotNull(result);
        }

        [Test]
        [Combinatorial]
        public void Compose_ValidSingle_FraTidspunktEqualsInput(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate,
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var virkning = VirkningType.Create(fromDate, toDate);
            var result = VirkningType.Compose(virkning);
            Assert.AreEqual(virkning.FraTidspunkt.ToDateTime(), result.FraTidspunkt.ToDateTime());
        }

        [Test]
        [Combinatorial]
        public void Compose_ValidSingle_TilTidspunktEqualsInput(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate,
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var virkning = VirkningType.Create(fromDate, toDate);
            var result = VirkningType.Compose(virkning);
            Assert.AreEqual(virkning.TilTidspunkt.ToDateTime(), result.TilTidspunkt.ToDateTime());
        }

        [Test]
        public void Compose_ValidArray_CorrectFraTidspunkt(
            [ValueSource("FutureDatesWithNull")] DateTime? toDate)
        {
            var input = PastDates.Select(pd => VirkningType.Create(pd, toDate)).ToArray();
            var result = VirkningType.Compose(input);
            Assert.AreEqual(PastDates.Min(), result.FraTidspunkt.ToDateTime());
        }

        [Test]
        public void Compose_ValidArray_CorrectTilTidspunkt(
            [ValueSource("PastDatesWithNull")] DateTime? fromDate)
        {
            var input = FutureDates.Select(pd => VirkningType.Create(fromDate, pd)).ToArray();
            var result = VirkningType.Compose(input);
            Assert.AreEqual(FutureDates.Max(), result.TilTidspunkt.ToDateTime());
        }
        #endregion

        #region IsDoubleOpen
        [Test]
        public void IsDoubleOpen_Null_ReturnsTrue()
        {
            var result = VirkningType.IsDoubleOpen(null);
            Assert.True(result);
        }

        [Test]
        [TestCaseSource("AllDates")]
        public void IsDoubleOpen_ToSpecified_ReturnsFalse(
            DateTime? toDate)
        {
            var result = VirkningType.IsDoubleOpen(VirkningType.Create(null, toDate));
            Assert.False(result);
        }

        [Test]
        [TestCaseSource("AllDates")]
        public void IsDoubleOpen_FromSpecified_ReturnsFalse(
            DateTime? fromDate)
        {
            var result = VirkningType.IsDoubleOpen(VirkningType.Create(fromDate, null));
            Assert.False(result);
        }

        [Test]
        [Combinatorial]
        public void IsDoubleOpen_ClosedValues_ReturnsFalse(
            [ValueSource("PastDates")] DateTime? fromDate,
            [ValueSource("FutureDates")] DateTime? toDate)
        {
            var result = VirkningType.IsDoubleOpen(VirkningType.Create(fromDate, toDate));
            Assert.False(result);
        }
        #endregion
    }
}
