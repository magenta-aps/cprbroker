using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CprBroker.Installers.EventBrokerInstallers
{
    /// <summary>
    /// Installs the Event Broker system
    /// </summary>
    [System.ComponentModel.RunInstaller(true)]
    public partial class EventBrokerInstaller : System.Configuration.Install.Installer
    {
        public EventBrokerInstaller()
        {
            InitializeComponent();
            AddInstallers();
        }

        public EventBrokerInstaller(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            AddInstallers();
        }

        private void AddInstallers()
        {
            Installers.Add(new EventBrokerDatabaseInstaller());
            Installers.Add(new EventBrokerWebSiteInstaller());
            Installers.Add(new EventBrokerBackendServiceInstaller());
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
