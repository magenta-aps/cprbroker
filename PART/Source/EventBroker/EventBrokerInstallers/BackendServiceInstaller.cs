using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Windows.Forms;
using CprBroker.Utilities;

namespace CprBroker.Installers.EventBrokerInstallers
{
    /// <summary>
    /// Installs Backend service
    /// </summary>
    public partial class EventBrokerBackendServiceInstaller : Installer
    {
        public EventBrokerBackendServiceInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            ServiceNameForm frm = new ServiceNameForm() { ServiceName = "Event Broker backend service" };
            BaseForm.ShowAsDialog(frm, this.InstallerWindowWrapper());

            string serviceName = frm.ServiceName;
            var eventsServiceUrl = frm.CprEventsServiceUrl;

            var savedStateWrapper = new SavedStateWrapper(stateSaver);
            savedStateWrapper.ServiceName = serviceName;
            this.backendServiceInstaller.ServiceName = serviceName;

            base.Install(stateSaver);

            UpdateConfiguration(serviceName, eventsServiceUrl, frm.CprBrokerDatabaseInfo.CreateConnectionString(false, true));
        }

        public override void Rollback(IDictionary savedState)
        {
            try
            {
                this.backendServiceInstaller.ServiceName = new SavedStateWrapper(savedState).ServiceName;
                base.Uninstall(savedState);
            }
            catch (Exception ex)
            {
                Messages.ShowException(this, "", ex);
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                this.backendServiceInstaller.ServiceName = new SavedStateWrapper(savedState).ServiceName;
                base.Uninstall(savedState);
            }
            catch (Exception ex)
            {
                Messages.ShowException(this, "", ex);
            }
        }

        private void UpdateConfiguration(string serviceName, string cprEventsServiceUrl, string cprBrokerConnectionString)
        {
            string configFileName = typeof(CprBroker.EventBroker.Backend.BackendService).Assembly.Location + ".config";

            ConnectionStringsInstaller.RegisterConnectionString(
                configFileName,
                typeof(Config.Properties.Settings).FullName + ".CprBrokerConnectionString",
                cprBrokerConnectionString
                );

            ConnectionStringsInstaller.RegisterConnectionString(
                this.GetWebConfigFilePathFromInstaller(),
                typeof(Config.Properties.Settings).FullName + ".CprBrokerConnectionString",
                cprBrokerConnectionString
                );

            ConnectionStringsInstaller.RegisterCommitAction(
                configFileName,
                () =>
                {
                    ServiceController serviceController = new ServiceController(serviceName);
                    serviceController.Start();
                }
                );
            Installation.SetApplicationSettingInConfigFile(configFileName, typeof(CprBroker.Config.Properties.Settings), "EventsServiceUrl", cprEventsServiceUrl);

            Installation.SetFlatFileLogListenerAccessRights(configFileName);
        }

        
    }
}
