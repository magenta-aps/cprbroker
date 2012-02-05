using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Data.SqlClient;
using CprBroker.Installers;
using CprBroker.Utilities;
using Microsoft.Win32;

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
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, "DPRN");
            CprBroker.Installers.Installation.SetConnectionStringInConfigFile(
                configFileName,
                "DPRUpdates",
                databaseSetupInfo.CreateConnectionString(false, true)
            );
            // Service url
            CprBroker.Installers.Installation.SetApplicationSettingInConfigFile(
                configFileName,
                "DPRUpdatesNotification.Properties.Settings",
                "CPRBrokerPartServiceUrl",
                string.Format("{0}/Services/Part.asmx", session.GetPropertyValue("PARTSERVICEURL"))
            );
            // App token
            CprBroker.Installers.Installation.SetApplicationSettingInConfigFile(
                configFileName,
                "DPRUpdatesNotification.Properties.Settings",
                "ApplicationToken",
                appToken
            );
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
