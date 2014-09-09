using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine.Queues;
using CprBroker.Engine.Tasks;

namespace CprBroker.DBR
{
    public class DprDiversionManager : TaskExecutionManager<DprDiversionServer, DprDiversionServer.EqualityComparer>
    {
        public override void StartTask(DprDiversionServer task)
        {
            CprBroker.Engine.Local.Admin.LogSuccess(string.Format("Staring DBR Diversion: queue<{0}> on port<{1}>", task.DbrQueue.QueueId, task.Port));
            task.Start();
        }

        public override void DisposeTask(DprDiversionServer task)
        {
            task.Stop();
            task.Dispose();
        }

        public override DprDiversionServer[] GetTasks()
        {
            return Queue.GetQueues<DbrQueue>()
                .Where(q => q.DiversionEnabled)
                .Select(q => q.CreateListener())
                .ToArray();
        }
    }
}
