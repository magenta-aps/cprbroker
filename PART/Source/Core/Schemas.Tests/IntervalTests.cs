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
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.Schemas
{
    namespace IntervalTests
    {
        class CurrentStub1 : ITimedType
        {
            public CurrentStub1()
            {
                Tag = (DataTypeTags)1;
            }

            public DateTime? _StartTS;
            public DateTime? ToStartTS() { return _StartTS; }
            public DateTime? ToEndTS() { return null; }
            public DataTypeTags Tag { get; set; }
            public IRegistrationInfo Registration { get; set; }
        }

        class HistoryStub1 : ITimedType
        {
            public HistoryStub1()
            {
                Tag = (DataTypeTags)1;
            }
            public DateTime? _EndTS;
            public DateTime? ToEndTS() { return _EndTS; }
            public DateTime? _StartTS;
            public DateTime? ToStartTS() { return _StartTS; }
            public DataTypeTags Tag { get; set; }
            public IRegistrationInfo Registration { get; set; }
        }

        class CurrentStub2 : ITimedType
        {
            public CurrentStub2()
            {
                Tag = (DataTypeTags)2;
            }
            public DateTime? _StartTS;
            public DateTime? ToStartTS() { return _StartTS; }
            public DateTime? ToEndTS() { return null; }
            public DataTypeTags Tag { get; set; }
            public IRegistrationInfo Registration { get; set; }
        }

        class HistoryStub2 : ITimedType
        {
            public HistoryStub2()
            {
                Tag = (DataTypeTags)2;
            }
            public DateTime? _EndTS;
            public DateTime? ToEndTS() { return _EndTS; }
            public DateTime? _StartTS;
            public DateTime? ToStartTS() { return _StartTS; }
            public DataTypeTags Tag { get; set; }
            public IRegistrationInfo Registration { get; set; }
        }

        [TestFixture]
        public class CreateFromData
        {
            [Test]
            public void CreateFromData_OneTag2Intervals_2Intervals()
            {
                var c1 = new CurrentStub1() { _StartTS = DateTime.Today };
                var h1 = new HistoryStub1() { _StartTS = DateTime.Today.AddDays(-10), _EndTS = DateTime.Today.AddDays(-1) };
                var intervals = Interval.CreateFromData(c1, h1);

                Assert.AreEqual(2, intervals.Length);

                Assert.AreEqual(h1._StartTS, intervals[0].StartTS);
                Assert.AreEqual(h1._EndTS, intervals[0].EndTS);

                Assert.AreEqual(c1._StartTS, intervals[1].StartTS);
                Assert.Null(intervals[1].EndTS);
            }

            [Test]
            public void CreateFromData_2Tags1and2Intervals_2Intervals()
            {
                var c1 = new CurrentStub1() { _StartTS = DateTime.Today };
                var h1 = new HistoryStub1() { _StartTS = DateTime.Today.AddDays(-10), _EndTS = DateTime.Today.AddDays(-1) };

                var c2 = new CurrentStub2() { _StartTS = DateTime.Today.AddDays(-10) };

                var intervals = Interval.CreateFromData(c2, c1, h1);

                Assert.AreEqual(2, intervals.Length);

                Assert.AreEqual(h1._StartTS, intervals[0].StartTS);
                Assert.AreEqual(h1._EndTS, intervals[0].EndTS);

                Assert.AreEqual(c1._StartTS, intervals[1].StartTS);
                Assert.Null(intervals[1].EndTS);
            }

            [Test]
            public void CreateFromData_2TagsDiffStarts_3Intervals()
            {
                var h1 = new HistoryStub1() { _StartTS = DateTime.Today.AddDays(-10), _EndTS = DateTime.Today.AddDays(-3) };
                var c1 = new CurrentStub1() { _StartTS = DateTime.Today.AddDays(-3) };

                var c2 = new CurrentStub2() { _StartTS = DateTime.Today.AddDays(-7) };


                var intervals = Interval.CreateFromData(h1, c1, c2);

                Assert.AreEqual(3, intervals.Length);

                Assert.AreEqual(h1.ToStartTS(), intervals[0].StartTS);
                Assert.AreEqual(c2.ToStartTS(), intervals[0].EndTS);
                Assert.AreEqual(1, intervals[0].Data.Count);

                Assert.AreEqual(c2.ToStartTS(), intervals[1].StartTS);
                Assert.AreEqual(c1.ToStartTS(), intervals[1].EndTS);
                Assert.AreEqual(2, intervals[1].Data.Count);

                Assert.AreEqual(c1.ToStartTS(), intervals[2].StartTS);
                Assert.Null(intervals[2].EndTS);
                Assert.AreEqual(2, intervals[2].Data.Count);
            }
        }

    }
}
