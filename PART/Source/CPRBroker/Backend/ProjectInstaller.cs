using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using Microsoft.Win32;
using CprBroker.Engine.Util;
using System.Windows.Forms;

namespace CprBroker.EventBroker.Backend
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
            try
            {
                base.Install(stateSaver);
            }
            catch (Win32Exception ex)
            {
                if (ex.Message == "The specified service already exists")
                {
                    this.Uninstall(stateSaver);
                    stateSaver.Clear();
                    base.Install(stateSaver);
                }
            }

            //Installation.SetConnectionStringInConfigFile(this.GetInstallerAssemblyConfigFilePath(), this.GetConnectionStringFromWebConfig());
            ServiceController serviceController = new ServiceController(this.backendServiceInstaller.ServiceName);
            if (backendServiceInstaller.StartType == ServiceStartMode.Automatic)
            {
                try
                {
                    serviceController.Start();
                }
                catch (Exception ex)
                {
                    string message = string.Format(
                        "Could not start service \"{0}\", the installation will continue but you need to start the service manually.\r\nThe error was\r\n\t\"{1}\"",
                        serviceController.ServiceName,
                        ex.Message);
                    System.Windows.Forms.MessageBox.Show(message);
                }
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            try
            {
                base.Uninstall(savedState);
            }
            catch (Exception ex)
            {
                object o = ex;
            }
        }
    }
}
