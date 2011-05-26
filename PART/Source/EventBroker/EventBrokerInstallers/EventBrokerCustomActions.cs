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
        [CustomAction]
        public static ActionResult CreateEventBrokerWebsite(Session session)
        {
            System.Diagnostics.Debugger.Break();
            Dictionary<string, string> connectionStrings = new Dictionary<string, string>();
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.FromSession(session);

            connectionStrings["CprBroker.Config.Properties.Settings.EventBrokerConnectionString"] = databaseSetupInfo.CreateConnectionString(false, true);
            // TODO: Get the correct value of CprBroker connection string
            connectionStrings["CprBroker.Config.Properties.Settings.CprBrokerConnectionString"] = databaseSetupInfo.CreateConnectionString(false, true);

            WebInstallationOptions options = new WebInstallationOptions()
            {
                EncryptConnectionStrings = true,
                ConnectionStrings = connectionStrings,
                InitializeFlatFileLogging = true,
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
            System.Diagnostics.Debugger.Break();
            return WebsiteCustomAction.RollbackWebsite(session);
        }

        [CustomAction]
        public static ActionResult RemoveEventBrokerWebsite(Session session)
        {
            System.Diagnostics.Debugger.Break();
            return WebsiteCustomAction.RemoveWebsite(session);
        }

        [CustomAction]
        public static ActionResult DeployEventBrokerDatabase(Session session)
        {
            System.Diagnostics.Debugger.Break();
            List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();

            ret.Add(new KeyValuePair<string, string>(typeof(ChannelType).Name, Properties.Resources.ChannelType));
            ret.Add(new KeyValuePair<string, string>(typeof(SubscriptionType).Name, Properties.Resources.SubscriptionType));

            return DatabaseCustomAction.DeployDatabase(session, Properties.Resources.CreateEventBrokerDatabaseObjects, ret.ToArray());            
        }

        [CustomAction]
        public static ActionResult RollbackEventBrokerDatabase(Session session)
        {
            System.Diagnostics.Debugger.Break();
            return DatabaseCustomAction.RemoveDatabase(session, false);
        }

        [CustomAction]
        public static ActionResult RemoveEventBrokerDatabase(Session session)
        {
            System.Diagnostics.Debugger.Break();
            return DatabaseCustomAction.RemoveDatabase(session,true);
        }
    }
}
