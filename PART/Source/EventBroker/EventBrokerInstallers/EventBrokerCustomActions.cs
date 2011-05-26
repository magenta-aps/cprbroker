using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using CprBroker.Utilities;
using CprBroker.Installers;
using CprBroker.Engine;
using CprBroker.EventBroker.Data;

namespace CprBroker.Installers.EventBrokerInstallers
{
    public static class EventBrokerCustomActions
    {
        static readonly string WebsiteDirectoryRelativePath = "EventBroker\\Website\\";
        public static readonly string ServiceName = "CPR broker backend service";


        [CustomAction]
        public static ActionResult CreateEventBrokerWebsite(Session session)
        {
            Dictionary<string, string> connectionStrings = new Dictionary<string, string>();
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.FromSession(session);

            string connectionString = databaseSetupInfo.CreateConnectionString(false, true);
            string connectionStringName = "CprBroker.Config.Properties.Settings.EventBrokerConnectionString";
            connectionStrings[connectionStringName] = connectionString;

            Installation.SetConnectionStringInConfigFile(GetServiceExeFullFileName(session) + ".config", connectionStringName, connectionString);

            WebInstallationOptions options = new WebInstallationOptions()
            {
                EncryptConnectionStrings = false,
                ConnectionStrings = connectionStrings,
                InitializeFlatFileLogging = true,
                WebsiteDirectoryRelativePath = WebsiteDirectoryRelativePath,
                ConfigSectionGroupEncryptionOptions = new ConfigSectionGroupEncryptionOptions[]
                {
                    new ConfigSectionGroupEncryptionOptions()
                    {
                        ConfigSectionGroupName = Constants.DataProvidersSectionGroupName,
                        ConfigSectionEncryptionOptions = new ConfigSectionEncryptionOptions[]
                        {
                            new ConfigSectionEncryptionOptions(){ SectionName = DataProviderKeysSection.SectionName, SectionType=typeof(DataProviderKeysSection), CustomMethod = config=>DataProviderKeysSection.RegisterInConfig(config)},
                            new ConfigSectionEncryptionOptions(){ SectionName = DataProvidersConfigurationSection.SectionName, SectionType=typeof(DataProvidersConfigurationSection),CustomMethod =null}
                        }
                    }
                }
            };
            return WebsiteCustomAction.DeployWebsite(session, options);
        }

        [CustomAction]
        public static ActionResult RollbackEventBrokerWebsite(Session session)
        {
            return WebsiteCustomAction.RollbackWebsite(session);
        }

        [CustomAction]
        public static ActionResult RemoveEventBrokerWebsite(Session session)
        {
            return WebsiteCustomAction.RemoveWebsite(session);
        }

        [CustomAction]
        public static ActionResult DeployEventBrokerDatabase(Session session)
        {
            List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();

            ret.Add(new KeyValuePair<string, string>(typeof(ChannelType).Name, Properties.Resources.ChannelType));
            ret.Add(new KeyValuePair<string, string>(typeof(SubscriptionType).Name, Properties.Resources.SubscriptionType));

            return DatabaseCustomAction.DeployDatabase(session, Properties.Resources.CreateEventBrokerDatabaseObjects, ret.ToArray());
        }

        [CustomAction]
        public static ActionResult RollbackEventBrokerDatabase(Session session)
        {
            return DatabaseCustomAction.RemoveDatabase(session, false);
        }

        [CustomAction]
        public static ActionResult RemoveEventBrokerDatabase(Session session)
        {
            return DatabaseCustomAction.RemoveDatabase(session, true);
        }

        private static string GetServiceExeFullFileName(Session session)
        {
            return string.Format("{0}{1}bin\\CprBroker.EventBroker.Backend.exe", session.GetInstallDirProperty(), WebsiteDirectoryRelativePath);
        }

        [CustomAction]
        public static ActionResult InstallBackendService(Session session)
        {
            CprBroker.Installers.Installation.RunCommand(
                string.Format("{0}installutil.exe", CprBroker.Installers.Installation.GetNetFrameworkDirectory(new Version(2, 0))),
                string.Format("/i \"{0}\"", GetServiceExeFullFileName(session))
            );
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RollbackBackendService(Session session)
        {
            return UnInstallBackendService(session);
        }

        [CustomAction]
        public static ActionResult UnInstallBackendService(Session session)
        {
            var controller = new System.ServiceProcess.ServiceController(ServiceName);
            if (controller.CanStop)
                controller.Stop();

            CprBroker.Installers.Installation.RunCommand(
                string.Format("{0}installutil.exe", CprBroker.Installers.Installation.GetNetFrameworkDirectory(new Version(2, 0))),
                string.Format("/u \"{0}\"", GetServiceExeFullFileName(session))
            );
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult SetCprBrokerConnectionStrings(Session session)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.FromSession(session);
            WebInstallationInfo webInstallationInfo = WebInstallationInfo.FromSession(session);


            string connectionString = databaseSetupInfo.CreateConnectionString(false, true);
            string connectionStringName = "CprBroker.Config.Properties.Settings.CprBrokerConnectionString";

            Installation.SetConnectionStringInConfigFile(
                GetServiceExeFullFileName(session) + ".config",
                connectionStringName,
                connectionString
                );
            Installation.SetConnectionStringInConfigFile(
                webInstallationInfo.GetWebConfigFilePath(WebsiteDirectoryRelativePath),
                connectionStringName,
                connectionString
                );
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult SetCprBrokerUrl(Session session)
        {
            WebInstallationInfo cprBrokerWebInstallationInfo = WebInstallationInfo.FromSession(session);

            var urls = cprBrokerWebInstallationInfo.CalculateWebUrls();
            Func<string, string> urlFunc = u => string.Format("{0}Services/Events.asmx", u);
            var bestUrl = urlFunc(urls[0]);

            for (int i = 0; i < urls.Length; i++)
            {
                string url = urlFunc(urls[i]);
                var client = new System.Net.WebClient();
                try
                {
                    client.DownloadData(url);
                    bestUrl = url;
                    break;
                }
                catch (Exception ex)
                {
                    object o = ex;
                }
            }

            Installation.SetApplicationSettingInConfigFile(
                GetServiceExeFullFileName(session) + ".config",
                typeof(CprBroker.Config.Properties.Settings),
                "EventsServiceUrl",
                bestUrl
                );
            return ActionResult.Success;
        }
    }
}
