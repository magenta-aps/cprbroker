using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.CPRDirect
{
    partial class ExtractDataContext
    {
        public ExtractDataContext()
            : this(CprBroker.Config.ConfigManager.Current.Settings.CprBrokerConnectionString)
        { }
    }
}
