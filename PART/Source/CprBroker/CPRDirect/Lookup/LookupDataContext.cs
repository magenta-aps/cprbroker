using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    public partial class LookupDataContext
    {
        public LookupDataContext()
            : this(CprBroker.Config.ConfigManager.Current.Settings.CprBrokerConnectionString)
        { }
    }
}
