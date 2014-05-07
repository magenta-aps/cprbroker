﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data.Queues;

namespace CprBroker.Engine.Sync
{
    public class SyncManager
    {
        public static ISyncTargetQueue[] GetAllTargets()
        {
            return QueueBase.GetQueues<ISyncTargetQueue>();
        }
    }
}
