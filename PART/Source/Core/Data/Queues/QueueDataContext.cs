using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Queues
{
    public partial class QueueDataContext
    {
        public QueueDataContext()
            : this(CprBroker.Config.Properties.Settings.Default.CprBrokerConnectionString)
        { }
    }
}
