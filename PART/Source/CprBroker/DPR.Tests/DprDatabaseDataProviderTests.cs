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
using System.Data.SqlClient;

using CprBroker.Providers.DPR;
using CprBroker.Providers.DPR.Queues;

namespace CprBroker.Tests.DPR
{
    namespace DprDatabaseDataProviderTests
    {
        [TestFixture]
        public class ToString
        {
            [Test]
            public void ToString_New_NotEmpty()
            {
                var prov = new DprDatabaseDataProvider();
                var ret = prov.ToString();
                Assert.IsNotEmpty(ret);
            }

            [Test]
            public void ToString_New_HasDbLocation()
            {
                var prov = new DprDatabaseDataProvider() { ConnectionString = "data source=DS; database=DB" };
                var ret = prov.ToString();
                Assert.GreaterOrEqual(ret.IndexOf(@"DS\DB"), 0);
            }

            [Test]
            public void ToString_NoDiversion_NoAddressPort()
            {
                var prov = new DprDatabaseDataProvider() { ConnectionString = "data source=DS; database=DB", Address = "adr", Port = 123, DisableDiversion = true };
                var ret = prov.ToString();
                Assert.AreEqual(-1, ret.IndexOf(@"adr"));
                Assert.AreEqual(-1, ret.IndexOf(@"123"));
            }

            [Test]
            public void ToString_HasDiversion_NoAddressPort()
            {
                var prov = new DprDatabaseDataProvider() { ConnectionString = "data source=DS; database=DB", Address = "adr", Port = 123, DisableDiversion = false };
                var ret = prov.ToString();
                Assert.GreaterOrEqual(ret.IndexOf(@"adr:123"), 0);
            }
        }

        public abstract class ChangesTest : PartInterface.TestBase
        {
            public override void InitLogging()
            {
                // Do nothing
            }

            public override void CreateDatabases()
            {
                Databases.Add(CreateDatabase("dprdpr_", Providers.DPR.Properties.Resources.CreateTrackingTables, new KeyValuePair<string, string>[] { }));
            }
        }

        [TestFixture]
        public class GetChanges : ChangesTest
        {
            [Test]
            public void GetChanges_None_Zero()
            {
                var prov = new DprDatabaseDataProvider() { ConnectionString = Databases[0].ConnectionString };
                var changes = prov.GetChanges(100, new TimeSpan());
                Assert.IsEmpty(changes);
            }

            [Test]
            public void GetChanges_ThreeWOneTooNew_Two()
            {
                var prov = new DprDatabaseDataProvider() { ConnectionString = Databases[0].ConnectionString };
                using (var dataContext = new Providers.DPR.Queues.UpdatesDataContext(prov.ConnectionString))
                {
                    dataContext.T_DPRUpdateStagings.InsertOnSubmit(new Providers.DPR.Queues.T_DPRUpdateStaging() { CreateTS = DateTime.Now.AddDays(-1), DPRTable = "" });
                    dataContext.T_DPRUpdateStagings.InsertOnSubmit(new Providers.DPR.Queues.T_DPRUpdateStaging() { CreateTS = DateTime.Now.AddDays(-1), DPRTable = "" });
                    dataContext.T_DPRUpdateStagings.InsertOnSubmit(new Providers.DPR.Queues.T_DPRUpdateStaging() { CreateTS = DateTime.Now.AddDays(0), DPRTable = "" });
                    dataContext.SubmitChanges();
                }
                var changes = prov.GetChanges(100, TimeSpan.FromMinutes(1)).ToArray();

                Assert.AreEqual(2, changes.Length);
            }

            [Test]
            public void GetChanges_ThreeWOneRequested_One()
            {
                var prov = new DprDatabaseDataProvider() { ConnectionString = Databases[0].ConnectionString };
                using (var dataContext = new Providers.DPR.Queues.UpdatesDataContext(prov.ConnectionString))
                {
                    dataContext.T_DPRUpdateStagings.InsertOnSubmit(new Providers.DPR.Queues.T_DPRUpdateStaging() { CreateTS = DateTime.Now.AddDays(-1), DPRTable = "" });
                    dataContext.T_DPRUpdateStagings.InsertOnSubmit(new Providers.DPR.Queues.T_DPRUpdateStaging() { CreateTS = DateTime.Now.AddDays(-1), DPRTable = "" });
                    dataContext.T_DPRUpdateStagings.InsertOnSubmit(new Providers.DPR.Queues.T_DPRUpdateStaging() { CreateTS = DateTime.Now.AddDays(0), DPRTable = "" });
                    dataContext.SubmitChanges();
                }
                var changes = prov.GetChanges(1, TimeSpan.FromMinutes(0)).ToArray();
                Assert.AreEqual(1, changes.Length);
            }
        }

        [TestFixture]
        public class DeleteChanges : ChangesTest
        {
            [Test]
            public void DeleteChanges_None_OK()
            {
                var prov = new DprDatabaseDataProvider() { ConnectionString = Databases[0].ConnectionString };
                var changes = new T_DPRUpdateStaging[0];
                prov.DeleteChanges(changes);
            }

            [Test]
            public void DeleteChanges_All_NoneLeft()
            {
                var prov = new DprDatabaseDataProvider() { ConnectionString = Databases[0].ConnectionString };
                using (var dataContext = new Providers.DPR.Queues.UpdatesDataContext(prov.ConnectionString))
                {
                    dataContext.T_DPRUpdateStagings.InsertOnSubmit(new Providers.DPR.Queues.T_DPRUpdateStaging() { CreateTS = DateTime.Now.AddDays(-1), DPRTable = "" });
                    dataContext.T_DPRUpdateStagings.InsertOnSubmit(new Providers.DPR.Queues.T_DPRUpdateStaging() { CreateTS = DateTime.Now.AddDays(-1), DPRTable = "" });
                    dataContext.T_DPRUpdateStagings.InsertOnSubmit(new Providers.DPR.Queues.T_DPRUpdateStaging() { CreateTS = DateTime.Now.AddDays(0), DPRTable = "" });
                    dataContext.SubmitChanges();
                }
                var changes = prov.GetChanges(100, TimeSpan.FromMinutes(0)).ToArray();
                prov.DeleteChanges(changes);

                var newChanges = prov.GetChanges(100, new TimeSpan());
                Assert.IsEmpty(newChanges);
            }

            [Test]
            public void DeleteChanges_OneOfTwo_OneLeft()
            {
                var prov = new DprDatabaseDataProvider() { ConnectionString = Databases[0].ConnectionString };
                using (var dataContext = new Providers.DPR.Queues.UpdatesDataContext(prov.ConnectionString))
                {
                    dataContext.T_DPRUpdateStagings.InsertOnSubmit(new Providers.DPR.Queues.T_DPRUpdateStaging() { CreateTS = DateTime.Now.AddDays(-1), DPRTable = "" });
                    dataContext.T_DPRUpdateStagings.InsertOnSubmit(new Providers.DPR.Queues.T_DPRUpdateStaging() { CreateTS = DateTime.Now.AddDays(0), DPRTable = "" });
                    dataContext.SubmitChanges();
                }
                var changes = prov.GetChanges(1, TimeSpan.FromMinutes(0)).ToArray();
                prov.DeleteChanges(changes);

                var newChanges = prov.GetChanges(100, new TimeSpan());
                Assert.AreEqual(1, newChanges.Count());
            }
        }
    }
}
