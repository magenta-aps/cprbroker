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
using CprBroker.Data.DataProviders;

namespace CprBroker.Tests.Data
{
    namespace BudgetIntervalTests
    {
        [TestFixture]
        public class EffectiveLastCheckedTime
        {
            [Test]
            public void EffectiveLastCheckedTime_Null_MinDate()
            {
                var bi = new BudgetInterval();
                var ret = bi.EffectiveLastCheckedTime;
                Assert.AreEqual(DateTime.MinValue, ret);
            }

            [Test]
            public void EffectiveLastCheckedTime_Value_Returned(
                [Values(1, 22, -8789, 0)]int dayDiff)
            {
                DateTime now = DateTime.Now.AddDays(dayDiff);
                var bi = new BudgetInterval() { LastChecked = now };
                var ret = bi.EffectiveLastCheckedTime;
                Assert.AreEqual(now, ret);
            }
        }

        [TestFixture]
        public class CanRunAt
        {
            [Test]
            public void CanRunAt_Now_NeverRun_True(
                [Values(0, 10000, 25695454)]int interval)
            {
                var bi = new BudgetInterval() { IntervalMillisecods = interval };
                var ret = bi.CanRunAt(DateTime.Now);
                Assert.True(ret);
            }

            [Test]
            public void CanRunAt_Now_RunNow_CanRunYesterday_False(
                [Values(0, 10000, 25695454)]int interval)
            {
                var bi = new BudgetInterval() { IntervalMillisecods = interval, LastChecked = DateTime.Now };
                var ret = bi.CanRunAt(DateTime.Now.AddDays(-1));
                Assert.False(ret);
            }

            [Test]
            public void CanRunAt_LessTimePassed_False(
                [Values(1000)]int interval,
                [Values(10)] int allowancePercentage,
                [Values(0, 100, 899)] int checkTimeOffset)
            {
                var bi = new BudgetInterval() { IntervalMillisecods = interval, LastChecked = DateTime.Now };
                var ret = bi.CanRunAt(bi.LastChecked.Value.AddMilliseconds(checkTimeOffset), allowancePercentage);
                Assert.False(ret);
            }

            [Test]
            public void CanRunAt_OKTimePassed_True(
                [Values(1000)]int interval,
                [Values(10)] int allowancePercentage,
                [Values(900, 1000, 1100, 1285744)] int checkTimeOffset)
            {
                var bi = new BudgetInterval() { IntervalMillisecods = interval, LastChecked = DateTime.Now };
                var ret = bi.CanRunAt(bi.LastChecked.Value.AddMilliseconds(checkTimeOffset), allowancePercentage);
                Assert.True(ret);
            }

            [Test]
            [ExpectedException(typeof(ArgumentOutOfRangeException))]
            public void CanRunAt_InvalidAllowance_Exception(
                [Values(1000)]int interval,
                [Values(-100, -1, 101)] int allowancePercentage,
                [Values(1000)] int checkTimeOffset)
            {
                var bi = new BudgetInterval() { IntervalMillisecods = interval, LastChecked = DateTime.Now };
                var ret = bi.CanRunAt(bi.LastChecked.Value.AddMilliseconds(checkTimeOffset), allowancePercentage);
            }
        }

        [TestFixture]
        public class SuggestedStartTime
        {
            [Test]
            public void SuggestedStartTime_NeverRun_NowMinusInterval(
                [Values(19, 100, 87636)]int interval)
            {
                var bi = new BudgetInterval() { LastChecked = null, IntervalMillisecods = interval };
                var now = DateTime.Now;
                var ret = bi.SuggestedStartTime(now);
                Assert.AreEqual((double)interval, (now - ret).TotalMilliseconds);
            }

            [Test]
            public void SuggestedStartTime_RunToday_Today(
                [Values(19, 100, 87636, int.MaxValue)]int interval,
                [Values(545456,0,87895465)]int nowOffset)
            {
                var now = DateTime.Now;
                var bi = new BudgetInterval() { LastChecked = now, IntervalMillisecods = interval };
                var ret = bi.SuggestedStartTime(now.AddMilliseconds(nowOffset));

                Assert.AreEqual(now, ret);
            }
        }
    }
}
