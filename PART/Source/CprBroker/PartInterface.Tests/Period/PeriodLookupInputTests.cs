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
using CprBroker.Engine.Period;


namespace CprBroker.Tests.Engine.Period
{
    namespace PeriodLookupInputTests
    {
        [TestFixture]
        public class Validate
        {
            [Test]
            public void Validate_DatesNull_DatesSet()
            {
                var input = new PeriodLookupInput() { EffectDateFrom = null, EffectDateTo = null, UUIDs = new string[] { Guid.NewGuid().ToString() } };
                var ret = input.Validate();
                Assert.AreEqual("200", ret.StatusKode);
                Assert.AreEqual(DateTime.Today, input.EffectDateFrom);
                Assert.AreEqual(DateTime.Today, input.EffectDateTo);
            }

            [Test]
            public void Validate_FromDateNull_Fails()
            {
                var input = new PeriodLookupInput() { EffectDateFrom = null, EffectDateTo = DateTime.Today, UUIDs = new string[] { Guid.NewGuid().ToString() } };
                var ret = input.Validate();
                Assert.AreEqual("400", ret.StatusKode);
                Assert.Null(input.EffectDateFrom);
                Assert.AreEqual(DateTime.Today, input.EffectDateTo);
            }

            [Test]
            public void Validate_ToDateNull_Fails()
            {
                var input = new PeriodLookupInput() { EffectDateFrom = DateTime.Today, EffectDateTo = null, UUIDs = new string[] { Guid.NewGuid().ToString() } };
                var ret = input.Validate();
                Assert.AreEqual("400", ret.StatusKode);
                Assert.AreEqual(DateTime.Today, input.EffectDateFrom);
                Assert.Null(input.EffectDateTo);
            }

            [Test]
            public void Validate_AllDatesToday_OK()
            {
                DateTime date = DateTime.Today.AddDays(-1);
                var input = new PeriodLookupInput() { EffectDateFrom = date, EffectDateTo = date, UUIDs = new string[] { Guid.NewGuid().ToString() } };
                var ret = input.Validate();
                Assert.AreEqual("200", ret.StatusKode);
                Assert.AreEqual(date, input.EffectDateFrom);
                Assert.AreEqual(date, input.EffectDateTo);
            }

            [Test]
            public void Validate_InvalidDatesDates_Fails()
            {
                DateTime date = DateTime.Today.AddDays(-1);
                var input = new PeriodLookupInput() { EffectDateFrom = date.AddDays(1), EffectDateTo = date, UUIDs = new string[] { Guid.NewGuid().ToString() } };
                var ret = input.Validate();
                Assert.AreEqual("400", ret.StatusKode);
                Assert.AreEqual(date.AddDays(1), input.EffectDateFrom);
                Assert.AreEqual(date, input.EffectDateTo);

            }

        }
    }
}
