﻿using System;
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
                    //serviceController.Start();
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
