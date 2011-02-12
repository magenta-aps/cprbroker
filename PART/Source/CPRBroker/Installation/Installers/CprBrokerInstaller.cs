using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CprBroker.Installers
{
    public partial class CprBrokerInstaller : Component
    {
        public CprBrokerInstaller()
        {
            
        }

        public CprBrokerInstaller(IContainer container)
        {
            container.Add(this);

            
        }
    }
}
