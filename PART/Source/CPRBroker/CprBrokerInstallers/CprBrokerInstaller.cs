using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CprBroker.Installers.CprBrokerInstallers
{
    [System.ComponentModel.RunInstaller(true)]
    public partial class CprBrokerInstaller : System.Configuration.Install.Installer
    {
        public CprBrokerInstaller()
        {
            InitializeComponent();
            AddInstallers();
        }

        public CprBrokerInstaller(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            AddInstallers();
        }

        private void AddInstallers()
        {
            Installers.Add(new CprBrokerDatabaseInstaller());
            Installers.Add(new CprBrokerWebSiteInstaller());
            Installers.Add(new ConnectionStringsInstaller());
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            try
            {
                base.Uninstall(savedState);
            }
            catch (Exception ex)
            {
                Messages.ShowException(this, "", ex);
            }
        }
    }
}
