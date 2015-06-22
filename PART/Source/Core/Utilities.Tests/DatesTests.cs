using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Utilities;

namespace CprBroker.Tests.Utilities
{
    namespace DatesTests
    {
        [TestFixture]
        public class ToDateTimeOrNull
        {
            [Test]
            public void ToDateTimeOrNull_None_Null()
            {
                var ret = Dates.ToDateTimeOrNull(null);
                Assert.Null(ret);
            }

            [Test]
            public void ToDateTimeOrNull_Date_CorrectMinutes()
            {
                var d = new DateTime(2015, 06, 22, 10, 55, 0);
                var ret = Dates.ToDateTimeOrNull(d.ToString());
                Assert.AreEqual(55, ret.Value.Minute);
            }

            [Test]
            public void ToDateTimeOrNull_DateWithTime_DateTimeThenDate_TimeValue()
            {
                var d = new DateTime(2015, 06, 22, 10, 55, 0);
                var ret = Dates.ToDateTimeOrNull(d.ToString("yyyyMMddHHmm"), "yyyyMMddHHmm", "yyyyMMdd");
                Assert.AreEqual(55, ret.Value.Minute);
            }

            [Test]
            public void ToDateTimeOrNull_DateWithTime_DateThenDateTime_TimeValue()
            {
                var d = new DateTime(2015, 06, 22, 10, 55, 0);
                var ret = Dates.ToDateTimeOrNull(d.ToString("yyyyMMddHHmm"), "yyyyMMdd", "yyyyMMddHHmm");
                Assert.AreEqual(55, ret.Value.Minute);
            }

            [Test]
            public void ToDateTimeOrNull_NowNoTime_DateThenDateTime_NoTimeValue()
            {
                var d = new DateTime(2015, 06, 22, 10, 55, 0);
                var ret = Dates.ToDateTimeOrNull(d.ToString("yyyyMMdd"), "yyyyMMdd", "yyyyMMddHHmm");
                Assert.AreEqual(0, ret.Value.Minute);
            }

            [Test]
            public void ToDateTimeOrNull_WrongInput_Null()
            {
                var ret = Dates.ToDateTimeOrNull("sdkdæl", "yyyyMMdd", "yyyyMMddHHmm");
                Assert.False(ret.HasValue);
            }
            
            [Test]
            public void ToDateTimeOrNull_UnmatchingFormat_Null()
            {
                var d = new DateTime(2015, 06, 22, 10, 55, 0);
                var ret = Dates.ToDateTimeOrNull(d.ToString("yyyyMMdd"), "yyyyMMddHHmm");
                Assert.False(ret.HasValue);
            }
        }
    }
}
