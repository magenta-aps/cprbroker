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
            new SavedStateWrapper(stateSaver).ServiceName = frm.ServiceName;
            
            this.backendServiceInstaller.ServiceName = frm.ServiceName;
            
            base.Install(stateSaver);

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
            this.backendServiceInstaller.ServiceName = new SavedStateWrapper(savedState).ServiceName;
            base.Uninstall(savedState);
        }
    }
}
