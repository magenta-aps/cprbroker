using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using CprBroker.Installers;

namespace PersonMasterInstallers
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult TestDatabaseConnection(Session session)
        {
            return DatabaseCustomAction.TestConnectionString(session);
        }

        [CustomAction]
        public static ActionResult DeployDatabase(Session session)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.FromSession(session);
            string sql = Properties.Resources.crebas;
            sql = sql.Replace("<pm-cryptpassword>", databaseSetupInfo.EncryptionKey);
            sql = sql.Replace("<pm-namespace>", databaseSetupInfo.Domain);

            return DatabaseCustomAction.DeployDatabase(session, sql, new KeyValuePair<string, string>[0]);
        }

        [CustomAction]
        public static ActionResult RollbackDatabase(Session session)
        {
            return DatabaseCustomAction.RemoveDatabase(session, false);
        }

        [CustomAction]
        public static ActionResult RemoveDatabase(Session session)
        {
            return DatabaseCustomAction.RemoveDatabase(session, true);
        }

        [CustomAction]
        public static ActionResult PopulateWebsites(Session session)
        {
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ValidateWebProperties(Session session)
        {
            return WebsiteCustomAction.ValidateWebProperties(session);
        }

        [CustomAction]
        public static ActionResult CreateWebsite(Session session)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.FromSession(session);
            Dictionary<string, string> connectionStrings = new Dictionary<string, string>();
            connectionStrings["CPRMapperDB"] = databaseSetupInfo.CreateConnectionString(false, true);
            WebInstallationOptions options = new WebInstallationOptions()
            {
                ConnectionStrings = connectionStrings,
                ConfigSectionGroupEncryptionOptions = new ConfigSectionGroupEncryptionOptions[0],
                EncryptConnectionStrings = false,
                InitializeFlatFileLogging = false,
                FrameworkVersion = new Version("4.0")
            };
            return WebsiteCustomAction.DeployWebsite(session, options);
        }

        [CustomAction]
        public static ActionResult RollbackWebsite(Session session)
        {
            return WebsiteCustomAction.RollbackWebsite(session);
        }

        [CustomAction]
        public static ActionResult RemoveWebsite(Session session)
        {
            return WebsiteCustomAction.RemoveWebsite(session);
        }

    }
}
