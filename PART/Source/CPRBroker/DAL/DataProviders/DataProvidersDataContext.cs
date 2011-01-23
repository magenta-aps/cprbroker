using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.DAL.DataProviders
{
    public partial class DataProvidersDataContext
    {
        public DataProvidersDataContext()
            : base(CprBroker.Config.Properties.Settings.Default.CPRConnectionString)
        {
            OnCreated();
        }
    }
}
