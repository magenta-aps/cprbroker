/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Data.SqlClient;
using CprBroker.Installers;
using Microsoft.Win32;
using System.ServiceProcess;

namespace UpdateLib
{
    public partial class CustomActions
    {
        private static AdminService.Admin CreateAdminServiceProxy(Session session)
        {
            return new AdminService.Admin()
            {
                Url = session.GetPropertyValue("PARTSERVICEURL").ToLower().Replace("part.asmx", "admin.asmx"),
                ApplicationHeaderValue = new AdminService.ApplicationHeader()
                {
                    ApplicationToken = session.GetPropertyValue("BASEAPPLICATIONTOKEN"),
                    UserToken = ""
                }
            };
        }

        private static string RegisterApplicationInCprBroker(Session session)
        {
            AdminService.Admin service = CreateAdminServiceProxy(session);
            var response = service.RequestAppRegistration(
                string.Format(
                    "{0} {1}",
                    session.GetPropertyValue("ProductName"),
                    new Random().Next(100000, 1000000)
                )
            );
            if (int.Parse(response.StandardRetur.StatusKode) != 200)
            {
                throw new Exception(string.Format("App registration failed. {0} {1}", response.StandardRetur.StatusKode, response.StandardRetur.FejlbeskedTekst));
            }
            var response2 = service.ApproveAppRegistration(response.Item.Token);
            if (int.Parse(response.StandardRetur.StatusKode) != 200)
            {
                throw new Exception(string.Format("App approve failed. {0} {1}", response.StandardRetur.StatusKode, response.StandardRetur.FejlbeskedTekst));
            }
            return response.Item.Token;
        }

        private static void UnregisterApplicationInCprBroker(Session session)
        {
            AdminService.Admin service = CreateAdminServiceProxy(session);
            string appToken = session.GetPropertyValue("APPLICATIONTOKEN");
            var response = service.UnregisterApp(appToken);
        }

        private static string GetConfigFileName(Session session, UpdateDetectionVariables updateDetectionVariables)
        {
            return string.Format("{0}bin\\{1}.config", session.GetInstallDirProperty(), updateDetectionVariables.ServiceExeFileName);
        }
        private static void UpdateConfigFile(Session session, UpdateDetectionVariables updateDetectionVariables, string appToken)
        {
            string configFileName = GetConfigFileName(session, updateDetectionVariables);
            // Connection string
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, updateDetectionVariables.DatabaseFeatureName);
            CprBroker.Installers.Installation.SetConnectionStringInConfigFile(
                configFileName,
                updateDetectionVariables.ConnectionStringName,
                databaseSetupInfo.CreateConnectionString(false, true)
            );
            // Service url
            CprBroker.Installers.Installation.SetApplicationSettingInConfigFile(
                configFileName,
                "UpdateLib.Properties.Settings",
                "CPRBrokerPartServiceUrl",
                session.GetPropertyValue("PARTSERVICEURL")
            );
            // App token
            CprBroker.Installers.Installation.SetApplicationSettingInConfigFile(
                configFileName,
                "UpdateLib.Properties.Settings",
                "ApplicationToken",
                appToken
            );
        }

        private static void UninstallServiceByName(Session session, UpdateDetectionVariables updateDetectionVariables)
        {
            if(CprBroker.Installers.Installation.ServiceExists(updateDetectionVariables.ServiceName,updateDetectionVariables.ServiceDisplayName))
            {
                UnInstallService(session,updateDetectionVariables);
            }
        }

        private static void InstallAndStartService(Session session, UpdateDetectionVariables updateDetectionVariables)
        {
            CprBroker.Installers.Installation.RunCommand(
                string.Format("{0}installutil.exe", CprBroker.Installers.Installation.GetNetFrameworkDirectory(new Version(4, 0))),
                string.Format("/i \"{0}bin\\{1}\"", session.GetInstallDirProperty(), updateDetectionVariables.ServiceExeFileName)
            );
            new System.ServiceProcess.ServiceController(updateDetectionVariables.ServiceName).Start();
        }

        private static void StopService(Session session, UpdateDetectionVariables updateDetectionVariables)
        {
            var controller = new System.ServiceProcess.ServiceController(updateDetectionVariables.ServiceName);
            if (controller.CanStop)
                controller.Stop();
        }

        private static void UnInstallService(Session session, UpdateDetectionVariables updateDetectionVariables)
        {
            CprBroker.Installers.Installation.RunCommand(
                string.Format("{0}installutil.exe", CprBroker.Installers.Installation.GetNetFrameworkDirectory(new Version(4, 0))),
                string.Format("/u \"{0}bin\\{1}\"", session.GetInstallDirProperty(), updateDetectionVariables.ServiceExeFileName)
            );
        }

        private static void UpdateRegistry(Session session, string appToken)
        {
            Registry.SetValue(
                string.Format(@"HKEY_LOCAL_MACHINE\Software\{0}\{1}\CprBroker", session.GetPropertyValue("Manufacturer"), session.GetPropertyValue("ProductName")),
                "ApplicationToken",
                appToken,
                RegistryValueKind.String
            );
        }

    }
}
