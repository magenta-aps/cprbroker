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

            public DateTime? StartTS { get; set; }
            public string Tag { get; set; }
        }

        class HistoryStub1 : IHistoryType
        {
            public HistoryStub1()
            {
                Tag = "1";
            }
            public DateTime? EndTS { get; set; }
            public DateTime? StartTS { get; set; }
            public string Tag { get; set; }
        }

        class CurrentStub2 : ICurrentType
        {
            public CurrentStub2()
            {
                Tag = "2";
            }
            public DateTime? StartTS { get; set; }
            public string Tag { get; set; }
        }

        class HistoryStub2 : IHistoryType
        {
            public HistoryStub2()
            {
                Tag = "2";
            }
            public DateTime? EndTS { get; set; }
            public DateTime? StartTS { get; set; }
            public string Tag { get; set; }
        }

        [TestFixture]
        public class CreateFromData
        {
            [Test]
            public void CreateFromData_OneTag2Intervals_2Intervals()
            {
                var c1 = new CurrentStub1() { StartTS = DateTime.Today };
                var h1 = new HistoryStub1() { StartTS = DateTime.Today.AddDays(-10), EndTS = DateTime.Today.AddDays(-1) };
                var intervals = Interval.CreateFromData(c1, h1);

                Assert.AreEqual(2, intervals.Length);

                Assert.AreEqual(h1.StartTS, intervals[0].StartTS);
                Assert.AreEqual(h1.EndTS, intervals[0].EndTS);

                Assert.AreEqual(c1.StartTS, intervals[1].StartTS);
                Assert.Null(intervals[1].EndTS);
            }

            [Test]
            public void CreateFromData_2Tags1and2Intervals_2Intervals()
            {
                var c1 = new CurrentStub1() { StartTS = DateTime.Today };
                var h1 = new HistoryStub1() { StartTS = DateTime.Today.AddDays(-10), EndTS = DateTime.Today.AddDays(-1) };

                var c2 = new CurrentStub2() { StartTS = DateTime.Today.AddDays(-10) };

                var intervals = Interval.CreateFromData(c2, c1, h1);

                Assert.AreEqual(2, intervals.Length);

                Assert.AreEqual(h1.StartTS, intervals[0].StartTS);
                Assert.AreEqual(h1.EndTS, intervals[0].EndTS);

                Assert.AreEqual(c1.StartTS, intervals[1].StartTS);
                Assert.Null(intervals[1].EndTS);
            }

            [Test]
            public void CreateFromData_2TagsDiffStarts_3Intervals()
            {
                var h1 = new HistoryStub1() { StartTS = DateTime.Today.AddDays(-10), EndTS = DateTime.Today.AddDays(-3) };
                var c1 = new CurrentStub1() { StartTS = DateTime.Today.AddDays(-3) };

                var c2 = new CurrentStub2() { StartTS = DateTime.Today.AddDays(-7) };


                var intervals = Interval.CreateFromData(h1, c1, c2);

                Assert.AreEqual(3, intervals.Length);

                Assert.AreEqual(h1.StartTS, intervals[0].StartTS);
                Assert.AreEqual(c2.StartTS, intervals[0].EndTS);
                Assert.AreEqual(1, intervals[0].Data.Count);

                Assert.AreEqual(c2.StartTS, intervals[1].StartTS);
                Assert.AreEqual(c1.StartTS, intervals[1].EndTS);
                Assert.AreEqual(2, intervals[1].Data.Count);

                Assert.AreEqual(c1.StartTS, intervals[2].StartTS);
                Assert.Null(intervals[2].EndTS);
                Assert.AreEqual(2, intervals[2].Data.Count);
            }
        }

    }
}
