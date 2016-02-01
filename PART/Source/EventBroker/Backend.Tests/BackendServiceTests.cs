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
using CprBroker.EventBroker;
using CprBroker.EventBroker.Notifications;
using CprBroker.EventBroker.Backend;
using CprBroker.Utilities.Config;
using CprBroker.Engine.Tasks;
using CprBroker.Engine.Queues;
using CprBroker.DBR;

namespace CprBroker.Tests.EventBroker.Backend
{
    [TestFixture]
    public class BackendServiceTests : PartInterface.TestBase
    {
        class TaskFactoryStub : TaskFactory
        {
            public override TasksConfigurationSection.TaskElement[] LoadTaskConfigElements()
            {
                var types = new Type[] { 
                    typeof(BirthdateEventEnqueuer),
                    typeof(DataChangeEventPuller),
                    typeof(CriteriaSubscriptionPersonPopulator),
                    typeof(DataChangeEventEnqueuer),
                    typeof(NotificationSender),
                    typeof(CPRDirectDownloader),
                    typeof(CPRDirectExtractor),
                    typeof(BudgetChecker),
                    typeof(QueueExecutionManager),
                    typeof(DprDiversionManager)
                };
                return types
                    .Select(t => new TasksConfigurationSection.TaskElement() { Type = t })
                    .ToArray();
            }
        }

        
        [Test]
        public void Create_XTimes_NoBlocking([Values(1, 10, 100, 1000)]int count)
        {
            CprBroker.Engine.BrokerContext.Initialize(CprBroker.Utilities.Constants.EventBrokerApplicationToken.ToString(), "");
            var services = new List<BackendService>();
            for (int i = 0; i < count; i++)
            {
                var service = new BackendService() { TaskFactory = new TaskFactoryStub() };
                service.StartTasks();
                services.Add(service);
            }
            System.Threading.Thread.Sleep(10);
            foreach (var service in services)
            {
                service.StopTasks();
                service.Dispose();
            }
        }
    }
}
