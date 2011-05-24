using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Data.SqlClient;
using CprBroker.Installers;
using CprBroker.Utilities;

namespace InstallerLib
{
    public partial class CustomActions
    {
        private static AdminService.Admin CreateAdminServiceProxy(Session session)
        {
            Uri uri = new Uri(session.GetPropertyValue("CPRBROKERSERVICESURL"));
            return new AdminService.Admin()
            {
                Url = session.GetPropertyValue("CPRBROKERSERVICESURL").ToLower().Replace("part.asmx", "admin.asmx"),
                ApplicationHeaderValue = new AdminService.ApplicationHeader()
                {
                    ApplicationToken = session.GetPropertyValue("APPLICATIONTOKEN"),
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

        private static void UpdateConfigFile(Session session, string appToken)
        {
            string configFileName = string.Format("{0}bin\\{1}.config", session.GetInstallDirProperty(), ServiceExeFileName);
            // Connection string
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.FromSession(session);
            CprBroker.Utilities.Installation.SetConnectionStringInConfigFile(
                configFileName,
                "DPRUpdates",
                databaseSetupInfo.CreateConnectionString(false, true)
            );
            // Service url
            CprBroker.Utilities.Installation.SetApplicationSettingInConfigFile(
                configFileName,
                "DPRUpdatesNotification.Properties.Settings",
                "CPRBrokerPartServiceUrl",
                string.Format("{0}/Services/Part.asmx", session.GetPropertyValue("CPRBROKERSERVICESURL"))
            );
            // App token
            CprBroker.Utilities.Installation.SetApplicationSettingInConfigFile(
                configFileName,
                "DPRUpdatesNotification.Properties.Settings",
                "ApplicationToken",
                appToken
            );
        }

        private static void InstallAndStartService(Session session)
        {
            CprBroker.Utilities.Installation.RunCommand(
                string.Format("{0}installutil.exe", CprBroker.Utilities.Installation.GetNetFrameworkDirectory(new Version(4, 0))),
                string.Format("/i \"{0}bin\\{1}\"", session.GetInstallDirProperty(), ServiceExeFileName)
            );
            new System.ServiceProcess.ServiceController(ServiceName).Start();
        }

    }
}
