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
    namespace RegisteredIntervalTests
    {
        class RegisteredIntervalStub : IRegisteredInterval
        {

            public DateTime? StartTS { get; set; }

            public DateTime? EndTS { get; set; }

            public DateTime? RegistrationDate { get; set; }
        }

        [TestFixture]
        public class MergeIntervals
        {
            private RegisteredIntervalStub[] CreateArray(bool reverse, params RegisteredIntervalStub[] values)
            {
                return reverse ? values.Reverse().ToArray() : values;
            }

            [Test]
            public void MergeIntervals_Normal_2Intervals(
                [Values(true, false)]bool reverse)
            {
                var intNow = new RegisteredIntervalStub() { StartTS = DateTime.Today.AddDays(0), EndTS = DateTime.Today.AddDays(1), RegistrationDate = DateTime.Today.AddDays(0) };
                var intPrv = new RegisteredIntervalStub() { StartTS = DateTime.Today.AddDays(-4), EndTS = DateTime.Today.AddDays(0), RegistrationDate = DateTime.Today.AddDays(-4) };

                var res = RegisteredInterval.MergeIntervals<RegisteredIntervalStub>(CreateArray(reverse, intNow, intPrv));
                Assert.AreEqual(intPrv, res.First());
                Assert.AreEqual(intNow, res.Last());
            }

            [Test]
            public void MergeIntervals_2OpenStartAndEnd_OnlyLatestInterval(
                [Values(true, false)]bool reverse)
            {
                var intNow = new RegisteredIntervalStub() { StartTS = null, EndTS = null, RegistrationDate = DateTime.Today.AddDays(0) };
                var intPrv = new RegisteredIntervalStub() { StartTS = null, EndTS = null, RegistrationDate = DateTime.Today.AddDays(-4) };

                var res = RegisteredInterval.MergeIntervals<RegisteredIntervalStub>(CreateArray(reverse, intNow, intPrv));
                Assert.AreEqual(1, res.Length);
                Assert.AreEqual(intNow, res.First());
            }

            [Test]
            public void MergeIntervals_2_ClosedStart_OpenEnd_1PastAnd1Current(
                [Values(true, false)]bool reverse)
            {
                var intNow = new RegisteredIntervalStub() { StartTS = DateTime.Today.AddDays(0), EndTS = null, RegistrationDate = DateTime.Today.AddDays(0) };
                var intPrv = new RegisteredIntervalStub() { StartTS = DateTime.Today.AddDays(-5), EndTS = null, RegistrationDate = DateTime.Today.AddDays(-4) };

                var res = RegisteredInterval.MergeIntervals<RegisteredIntervalStub>(CreateArray(reverse, intNow, intPrv));
                Assert.AreEqual(2, res.Length);
                Assert.AreEqual(intPrv, res.First());
                Assert.AreEqual(intNow, res.Last());
                Assert.AreEqual(intNow.StartTS.Value, intPrv.EndTS.Value);
            }

            [Test]
            public void MergeIntervals_2_ClosedIntersectedIntervals_LatestStartDateWins(
                [Values(true, false)]bool reverse)
            {
                var intNow = new RegisteredIntervalStub() { StartTS = DateTime.Today.AddDays(0), EndTS = DateTime.Today.AddDays(5), RegistrationDate = DateTime.Today.AddDays(0) };
                var intPrv = new RegisteredIntervalStub() { StartTS = DateTime.Today.AddDays(-5), EndTS = DateTime.Today.AddDays(3), RegistrationDate = DateTime.Today.AddDays(-4) };

                var res = RegisteredInterval.MergeIntervals<RegisteredIntervalStub>(CreateArray(reverse, intNow, intPrv));
                Assert.AreEqual(2, res.Length);
                Assert.AreEqual(intPrv, res.First());
                Assert.AreEqual(intNow, res.Last());
                Assert.AreEqual(intNow.StartTS.Value, intPrv.EndTS.Value);
            }

            [Test]
            public void MergeIntervals_2_ClosedIdenticalIntervals_LatestRegistrationDateWins(
                [Values(true, false)]bool reverse)
            {
                var intNow = new RegisteredIntervalStub() { StartTS = DateTime.Today.AddDays(0), EndTS = DateTime.Today.AddDays(5), RegistrationDate = DateTime.Today.AddDays(0) };
                var intPrv = new RegisteredIntervalStub() { StartTS = DateTime.Today.AddDays(0), EndTS = DateTime.Today.AddDays(5), RegistrationDate = DateTime.Today.AddDays(-4) };

                var res = RegisteredInterval.MergeIntervals<RegisteredIntervalStub>(CreateArray(reverse, intNow, intPrv));
                Assert.AreEqual(1, res.Length);
                Assert.AreEqual(intNow, res.First());
            }

        }

    }
}
