using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using Microsoft.Win32;
using CprBroker.Engine.Util;
using System.Windows.Forms;

namespace CprBroker.Installers.EventBrokerInstallers
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
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

            UpdateConfiguration(eventsServiceUrl, frm.CprBrokerDatabaseInfo.CreateConnectionString(false, true));
            StartService();
        }

        public override void Rollback(IDictionary savedState)
        {
            try
            {
                this.backendServiceInstaller.ServiceName = new SavedStateWrapper(savedState).ServiceName;
                base.Uninstall(savedState);
            }
            catch (Exception ex)
            { }
        }

        public override void Uninstall(IDictionary savedState)
        {            
            this.backendServiceInstaller.ServiceName = new SavedStateWrapper(savedState).ServiceName;
            base.Uninstall(savedState);
        }

        private void UpdateConfiguration(string cprEventsServiceUrl, string cprBrokerConnectionString)
        {
            var configFileName = typeof(CprBroker.EventBroker.Backend.BackendService).Assembly.Location + ".config";

            Engine.Util.Installation.SetApplicationSettingInConfigFile(configFileName, typeof(CprBroker.Config.Properties.Settings), "EventsServiceUrl", cprEventsServiceUrl);
            Engine.Util.Installation.SetConnectionStringInConfigFile(configFileName, typeof(Config.Properties.Settings).FullName + ".CprBrokerConnectionString", cprBrokerConnectionString);
        }

        private void StartService()
        {
            RegistryKey hklm = Registry.LocalMachine;
            hklm = hklm.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + backendServiceInstaller.ServiceName, true);
            var path = typeof(CprBroker.EventBroker.Backend.BackendService).Assembly.Location;
            if (path.IndexOf(" ") != -1)
            {
                path = "\"" + path + "\"";
            }
            hklm.SetValue("ImagePath", path);
            hklm.Flush();

            if (backendServiceInstaller.StartType == ServiceStartMode.Automatic)
            {
                try
                {
                    ServiceController serviceController = new ServiceController(this.backendServiceInstaller.ServiceName);
                    serviceController.Start();
                }
                catch (Exception ex)
                {
                    string message = string.Format(
                        "Could not start service \"{0}\", the installation will continue but you need to start the service manually.\r\nThe error was\r\n\t\"{1}\"",
                        this.backendServiceInstaller.ServiceName,
                        ex.Message);
                    System.Windows.Forms.MessageBox.Show(message);
                }
            }
        }
    }
}
