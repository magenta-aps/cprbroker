using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Providers.Local.Search
{
    partial class PartSearchDataContext
    {
        public PartSearchDataContext()
            : this(CprBroker.Config.Properties.Settings.Default.CprBrokerConnectionString)
        { }
    }
}
