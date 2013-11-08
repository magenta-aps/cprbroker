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
using CprBroker.Schemas.Part;
using NUnit.Framework;

namespace CprBroker.Tests.Schemas
{
    namespace VirkningTypeTests
    {
        public class CommonVirkningTypeTests
        {
            public CommonVirkningTypeTests()
            {
                DateTime today = DateTime.Today;
                PastDates = new DateTime?[] { today.AddYears(-3), today.AddMonths(-3), today.AddDays(-3), today.AddHours(-3), today.AddMinutes(-3), today.AddSeconds(-3), today.AddMilliseconds(-3) };
                FutureDates = new DateTime?[] { today.AddYears(3), today.AddMonths(3), today.AddDays(3), today.AddHours(3), today.AddMinutes(3), today.AddSeconds(3), today.AddMilliseconds(3) };
                PastDatesWithNull = new DateTime?[] { null, today }.Union(PastDates).ToArray();
                FutureDatesWithNull = new DateTime?[] { null, today }.Union(FutureDates).ToArray();
                AllDates = PastDates.Union(new DateTime?[] { today }).Union(FutureDates).ToArray();
                AllDatesWithNull = new DateTime?[] { today }.Union(AllDates).ToArray();
            }

            protected DateTime?[] PastDates = null;
            protected DateTime?[] FutureDates = null;

            protected DateTime?[] PastDatesWithNull = null;
            protected DateTime?[] FutureDatesWithNull = null;
            protected DateTime?[] AllDates = null;
            protected DateTime?[] AllDatesWithNull = null;
        }

        #region Create
        [TestFixture]
        public class Create : CommonVirkningTypeTests
        {
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
        }
        #endregion

        #region Compose
        [TestFixture]
        public class Compose : CommonVirkningTypeTests
        {
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
        }
        #endregion

        #region IsDoubleOpen
        [TestFixture]
        public class IsDoubleOpen : CommonVirkningTypeTests
        {
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
        }
        #endregion

        #region Intersects
        [TestFixture]
        public class Intersects
        {
            [Test]
            public void Intersects_AllNulls_True()
            {
                var v1 = VirkningType.Create(null, null);
                var v2 = VirkningType.Create(null, null);
                var ret = v1.Intersects(v2);
                Assert.True(ret);
            }

            [Test]
            public void Intersects_OneNulls_True()
            {
                var v1 = VirkningType.Create(DateTime.Today, DateTime.Today.AddDays(1));
                var v2 = VirkningType.Create(null, null);

                Assert.True(v1.Intersects(v2));
                Assert.True(v2.Intersects(v1));
            }

            [Test]
            public void Intersects_OneNullsOneHalfOpen_True()
            {
                var v1 = VirkningType.Create(DateTime.Today, null);
                var v2 = VirkningType.Create(null, null);

                Assert.True(v1.Intersects(v2));
                Assert.True(v2.Intersects(v1));
            }

            [Test]
            public void Intersects_TwoOpenEnd_True()
            {
                var v1 = VirkningType.Create(DateTime.Today, null);
                var v2 = VirkningType.Create(DateTime.Today.AddDays(-1), null);

                Assert.True(v1.Intersects(v2));
                Assert.True(v2.Intersects(v1));
            }

            [Test]
            public void Intersects_TwoHalfOpenOneEndsBeforeOtherStart_False()
            {
                var v1 = VirkningType.Create(null, DateTime.Today.AddDays(-1));
                var v2 = VirkningType.Create(DateTime.Today.AddDays(1), null);

                Assert.False(v1.Intersects(v2));
                Assert.False(v2.Intersects(v1));
            }

            [Test]
            public void Intersects_OneClosedOneHalfOpen_True()
            {
                var v1 = VirkningType.Create(DateTime.Today, DateTime.Today.AddDays(2));
                var v2 = VirkningType.Create(DateTime.Today.AddDays(1), null);

                Assert.True(v1.Intersects(v2));
                Assert.True(v2.Intersects(v1));
            }

            [Test]
            public void Intersects_OneClosedOneHalfOpenAfter_False()
            {
                var v1 = VirkningType.Create(DateTime.Today, DateTime.Today.AddDays(2));
                var v2 = VirkningType.Create(DateTime.Today.AddDays(3), null);

                Assert.False(v1.Intersects(v2));
                Assert.False(v2.Intersects(v1));
            }

            [Test]
            public void Intersects_OneClosedOneHalfOpenBefore_False()
            {
                var v1 = VirkningType.Create(DateTime.Today, DateTime.Today.AddDays(2));
                var v2 = VirkningType.Create(null, DateTime.Today.AddDays(-1));

                Assert.False(v1.Intersects(v2));
                Assert.False(v2.Intersects(v1));
            }

            [Test]
            public void Intersects_TwoClosedIntersecting_True()
            {
                var v1 = VirkningType.Create(DateTime.Today.AddDays(0), DateTime.Today.AddDays(2));
                var v2 = VirkningType.Create(DateTime.Today.AddDays(1), DateTime.Today.AddDays(4));

                Assert.True(v1.Intersects(v2));
                Assert.True(v2.Intersects(v1));
            }

            [Test]
            public void Intersects_TwoClosedNotIntersecting_True()
            {
                var v1 = VirkningType.Create(DateTime.Today.AddDays(0), DateTime.Today.AddDays(2));
                var v2 = VirkningType.Create(DateTime.Today.AddDays(3), DateTime.Today.AddDays(4));

                Assert.False(v1.Intersects(v2));
                Assert.False(v2.Intersects(v1));
            }
        }
        #endregion
    }
}

