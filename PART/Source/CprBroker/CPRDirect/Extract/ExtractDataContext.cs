using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities.Config;

namespace CprBroker.Providers.CPRDirect
{
    partial class ExtractDataContext
    {
        public ExtractDataContext()
            : this(ConfigManager.Current.Settings.CprBrokerConnectionString)
        { }
    }
}
