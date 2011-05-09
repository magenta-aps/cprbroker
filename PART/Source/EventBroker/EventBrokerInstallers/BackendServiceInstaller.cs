/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 *
 * The Initial Developer of the Original Code is
 * IT- og Telestyrelsen / Danish National IT and Telecom Agency.
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */
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

            string assemblyPath = Context.Parameters["assemblypath"];
            this.Context.Parameters["assemblypath"] = typeof(EventBroker.Backend.BackendService).Assembly.Location;

            base.Install(stateSaver);
            
            this.Context.Parameters["assemblypath"] = assemblyPath;

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
                    Installation.CopyConfigNode("//connectionStrings", this.GetWebConfigFilePathFromInstaller(), configFileName);
                    ServiceController serviceController = new ServiceController(serviceName);
                    serviceController.Start();
                }
                );
            Installation.SetApplicationSettingInConfigFile(configFileName, typeof(CprBroker.Config.Properties.Settings), "EventsServiceUrl", cprEventsServiceUrl);
            Installation.SetFlatFileLogListenerAccessRights(configFileName);
        }


    }
}
