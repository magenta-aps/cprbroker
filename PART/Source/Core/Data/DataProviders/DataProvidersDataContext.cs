using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.DataProviders
{
    /// <summary>
    /// Represents the data context for data providers
    /// </summary>
    public partial class DataProvidersDataContext
    {
        public DataProvidersDataContext()
            : base(CprBroker.Config.Properties.Settings.Default.CprBrokerConnectionString)
        {
            OnCreated();
        }
    }
}
