using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Events
{
    public partial class DataChangeEventDataContext
    {
        public DataChangeEventDataContext()
            : base(Config.Properties.Settings.Default.CprBrokerConnectionString)
        {
            OnCreated();
        }
    }
}
