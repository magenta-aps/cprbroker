using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.DAL.Events
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
