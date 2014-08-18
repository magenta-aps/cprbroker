using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities.Config;

namespace CprBroker.Providers.Local.Search
{
    partial class PartSearchDataContext
    {
        public PartSearchDataContext()
            : this(ConfigManager.Current.Settings.CprBrokerConnectionString)
        { }
    }
}
