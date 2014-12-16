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
using CprBroker.EventBroker.Notifications;

namespace CprBroker.Tests.EventBroker
{
    namespace DataChangeEventPullerTests
    {
        [TestFixture]
        public class PerformTimerAction : CprBroker.Tests.PartInterface.TestBase
        {

            public class DataChangeEventPullerStub : DataChangeEventPuller
            {
                public void PerformTimeAction_Public()
                {
                    base.PerformTimerAction();
                }
            }

            [SetUp]
            public void InitBrokerContext()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
            }

            public Data.Events.DataChangeEvent[] CreateSample(int count)
            {
                using (var sourceContext = new CprBroker.Data.Events.DataChangeEventDataContext(this.CprDatabase.ConnectionString))
                {
                    var soucreEvents = new List<CprBroker.Data.Events.DataChangeEvent>();
                    Random r = new Random();
                    for (int i = 0; i < count; i++)
                    {
                        soucreEvents.Add(new Data.Events.DataChangeEvent() { ReceivedDate = DateTime.Now.AddSeconds(r.Next(1, count * 10)), DataChangeEventId = Guid.NewGuid(), PersonRegistrationId = Guid.NewGuid(), PersonUuid = Guid.NewGuid() });
                    }
                    sourceContext.DataChangeEvents.InsertAllOnSubmit(soucreEvents);
                    sourceContext.SubmitChanges();
                    return soucreEvents.ToArray();
                }
            }

            [Test]
            public void PerformTimerAction_NEvents_SourceCleared([Values(0, 50, 100, 500, 1000, 10000)]int count)
            {
                CreateSample(count);
                var puller = new DataChangeEventPullerStub();
                puller.PerformTimeAction_Public();

                using (var sourceContext = new CprBroker.Data.Events.DataChangeEventDataContext(this.CprDatabase.ConnectionString))
                {
                    var c = sourceContext.DataChangeEvents.Count();
                    Assert.AreEqual(0, c);
                }
            }

            [Test]
            public void PerformTimerAction_NEvents_TargetHasNEvents([Values(0, 50, 100, 500, 1000, 10000)]int count)
            {
                CreateSample(count);
                var puller = new DataChangeEventPullerStub();
                puller.PerformTimeAction_Public();

                using (var targetContext = new CprBroker.EventBroker.Data.EventBrokerDataContext(this.EventDatabase.ConnectionString))
                {
                    var c = targetContext.DataChangeEvents.Count();
                    Assert.AreEqual(count, c);
                }
            }

            [Test]
            public void PerformTimerAction_NEvents_CorrectTime([Values(50, 100, 500, 1000, 10000)]int count)
            {
                var sourceEvents = CreateSample(count);
                using (var puller = new DataChangeEventPullerStub())
                {
                    puller.PerformTimeAction_Public();
                }

                var firstSource = sourceEvents.OrderBy(e => e.ReceivedDate).First();

                using (var targetContext = new CprBroker.EventBroker.Data.EventBrokerDataContext(this.EventDatabase.ConnectionString))
                {
                    var targets = targetContext.DataChangeEvents.OrderBy(e => e.ReceivedOrder).ToArray();
                    var firstTarget = targetContext.DataChangeEvents.OrderBy(e => e.ReceivedOrder).First();

                    Assert.AreEqual(
                        new DateTime(firstSource.ReceivedDate.Ticks - firstSource.ReceivedDate.Ticks % TimeSpan.TicksPerSecond),
                        new DateTime(firstTarget.DueDate.Ticks - firstTarget.DueDate.Ticks % TimeSpan.TicksPerSecond)
                        );
                }
            }
        }
    }
}
