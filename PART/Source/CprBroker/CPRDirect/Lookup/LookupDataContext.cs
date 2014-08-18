using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities.Config;

namespace CprBroker.Providers.CPRDirect
{
    public partial class LookupDataContext
    {
        public LookupDataContext()
            : this(ConfigManager.Current.Settings.CprBrokerConnectionString)
        { }
    }
}
