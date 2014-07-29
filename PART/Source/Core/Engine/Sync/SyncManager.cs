using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine.Queues;

namespace CprBroker.Engine.Sync
{
    public class SyncManager
    {
        public static ISyncTargetQueue[] GetAllTargets()
        {
            return Queue.GetQueues<ISyncTargetQueue>();
        }
    }
}
