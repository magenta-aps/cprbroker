using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CprBroker.Installers.CprBrokerInstallers
{
    public partial class CprBrokerInstaller : ProjectInstaller
    {
        public CprBrokerInstaller()
        {
            InitializeComponent();
        }

        public CprBrokerInstaller(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            //Installers.Add(new WebSiteInstaller("CprBroker") { Context = Context });
            Installers.Add(new CprBrokerDatabaseInstaller() { Context = Context });
        }
    }
}
