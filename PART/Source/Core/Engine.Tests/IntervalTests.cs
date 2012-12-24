using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Engine;

namespace CprBroker.Tests.Engine
{
    namespace IntervalTests
    {
        class CurrentStub1 : ICurrentType
        {
            public CurrentStub1()
            {
                Tag = "1";
            }

            public DateTime? StartDate { get; set; }
            public string Tag { get; set; }
        }

        class HistoryStub1 : IHistoryType
        {
            public HistoryStub1()
            {
                Tag = "1";
            }
            public DateTime? EndDate { get; set; }
            public DateTime? StartDate { get; set; }
            public string Tag { get; set; }
        }

        class CurrentStub2 : ICurrentType
        {
            public CurrentStub2()
            {
                Tag = "2";
            }
            public DateTime? StartDate { get; set; }
            public string Tag { get; set; }
        }

        class HistoryStub2 : IHistoryType
        {
            public HistoryStub2()
            {
                Tag = "2";
            }
            public DateTime? EndDate { get; set; }
            public DateTime? StartDate { get; set; }
            public string Tag { get; set; }
        }

        [TestFixture]
        public class CreateFromData
        {
            [Test]
            public void CreateFromData_OneTag2Intervals_2Intervals()
            {
                var c1 = new CurrentStub1() { StartDate = DateTime.Today };
                var h1 = new HistoryStub1() { StartDate = DateTime.Today.AddDays(-10), EndDate = DateTime.Today.AddDays(-1) };
                var intervals = Interval.CreateFromData(c1, h1);

                Assert.AreEqual(2, intervals.Length);

                Assert.AreEqual(h1.StartDate, intervals[0].StartTime);
                Assert.AreEqual(h1.EndDate, intervals[0].EndTime);

                Assert.AreEqual(c1.StartDate, intervals[1].StartTime);
                Assert.Null(intervals[1].EndTime);
            }

            [Test]
            public void CreateFromData_2Tags1and2Intervals_2Intervals()
            {
                var c1 = new CurrentStub1() { StartDate = DateTime.Today };
                var h1 = new HistoryStub1() { StartDate = DateTime.Today.AddDays(-10), EndDate = DateTime.Today.AddDays(-1) };

                var c2 = new CurrentStub2() { StartDate = DateTime.Today.AddDays(-10) };

                var intervals = Interval.CreateFromData(c2, c1, h1);

                Assert.AreEqual(2, intervals.Length);

                Assert.AreEqual(h1.StartDate, intervals[0].StartTime);
                Assert.AreEqual(h1.EndDate, intervals[0].EndTime);

                Assert.AreEqual(c1.StartDate, intervals[1].StartTime);
                Assert.Null(intervals[1].EndTime);
            }

            [Test]
            public void CreateFromData_2TagsDiffStarts_3Intervals()
            {
                var h1 = new HistoryStub1() { StartDate = DateTime.Today.AddDays(-10), EndDate = DateTime.Today.AddDays(-3) };
                var c1 = new CurrentStub1() { StartDate = DateTime.Today.AddDays(-3) };

                var c2 = new CurrentStub2() { StartDate = DateTime.Today.AddDays(-7) };


                var intervals = Interval.CreateFromData(h1, c1, c2);

                Assert.AreEqual(3, intervals.Length);

                Assert.AreEqual(h1.StartDate, intervals[0].StartTime);
                Assert.AreEqual(c2.StartDate, intervals[0].EndTime);
                Assert.AreEqual(1, intervals[0].Data.Count);

                Assert.AreEqual(c2.StartDate, intervals[1].StartTime);
                Assert.AreEqual(c1.StartDate, intervals[1].EndTime);
                Assert.AreEqual(2, intervals[1].Data.Count);

                Assert.AreEqual(c1.StartDate, intervals[2].StartTime);
                Assert.Null(intervals[2].EndTime);
                Assert.AreEqual(2, intervals[2].Data.Count);
            }
        }

    }
}
