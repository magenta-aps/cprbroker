using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Config
{
    public class StandardConfigProvider : IConfigProvider
    {
        public Properties.Settings Settings
        {
            get { return Properties.Settings.Default; }
        }
    }
}
