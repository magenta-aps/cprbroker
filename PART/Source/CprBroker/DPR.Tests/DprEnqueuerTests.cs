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
using CprBroker.Providers.DPR;
using CprBroker.Providers.DPR.Queues;
using CprBroker.Tests.PartInterface;

namespace CprBroker.Tests.DPR
{
    namespace DprEnqueuerTests
    {
        public class DprUpdateQueueStub : CprBroker.Engine.Queues.Queue<DprUpdateQueueItem>
        {
            public List<DprUpdateQueueItem> Elements = new List<DprUpdateQueueItem>();
            public override void Enqueue(DprUpdateQueueItem[] items, CprBroker.Engine.Queues.Semaphore semaphore = null)
            {
                Elements.AddRange(items);
            }

            public override void Remove(DprUpdateQueueItem[] items)
            {
                Elements = Elements.Except(items).ToList();
            }

            public override DprUpdateQueueItem[] Process(DprUpdateQueueItem[] items)
            {
                throw new NotImplementedException();
            }
        }

        [TestFixture]
        public class CopyChanges : TestBase
        {
            public override void CreateDatabases()
            {
                base.CreateDatabases();
                Databases.Add(CreateDatabase("dprdpr_", Providers.DPR.Properties.Resources.CreateTrackingTables, new KeyValuePair<string, string>[] { }));
            }

            public override void InitLogging()
            {
                base.InitLogging();
            }

            [Test]
            public void CopyChanges_None_OK()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), "");
                var source = new DprEnqueuer();
                var prov = new DprDatabaseDataProvider() { ConnectionString = Databases.Last().ConnectionString };

                var target = new DprUpdateQueueStub();
                source.CopyChanges(prov, Guid.NewGuid(), target);
                Assert.IsEmpty(target.Elements);
            }

            [Test]
            public void CopyChanges_Two_Two()
            {
                CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.BaseApplicationToken.ToString(), ""); 
                var source = new DprEnqueuer();
                var prov = new DprDatabaseDataProvider() { ConnectionString = Databases.Last().ConnectionString };
                using (var dataContext = new Providers.DPR.Queues.UpdatesDataContext(prov.ConnectionString))
                {
                    dataContext.T_DPRUpdateStagings.InsertOnSubmit(new T_DPRUpdateStaging() { DPRTable = "", PNR = 123, CreateTS = DateTime.Now.AddDays(-1) });
                    dataContext.T_DPRUpdateStagings.InsertOnSubmit(new T_DPRUpdateStaging() { DPRTable = "", PNR = 456, CreateTS = DateTime.Now.AddDays(-1) });
                    dataContext.SubmitChanges();
                }
                var target = new DprUpdateQueueStub();
                source.CopyChanges(prov, Guid.NewGuid(), target);
                Assert.AreEqual(2, target.Elements.Count);
            }
        }
    }
}
