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
        public static ActionResult AfterDatabaseDialog(Session session)
        {
            return DatabaseCustomAction.AfterDatabaseDialog(session);
        }

        [CustomAction]
        public static ActionResult AfterInstallInitialize_DB(Session session)
        {
            return DatabaseCustomAction.AfterInstallInitialize_DB(session);
        }

        [CustomAction]
        public static ActionResult DeployDatabase(Session session)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, "PM");
            string sql = Properties.Resources.crebas;
            sql = sql.Replace("<pm-cryptpassword>", databaseSetupInfo.EncryptionKey);
            sql = sql.Replace("<pm-namespace>", databaseSetupInfo.Domain);

            var sqlDictionary = new Dictionary<string, string>();
            sqlDictionary["PM"] = sql;

            var lookupDictinary = new Dictionary<string, KeyValuePair<string, string>[]>();
            lookupDictinary["PM"] = new KeyValuePair<string, string>[0];

            return DatabaseCustomAction.DeployDatabase(session, sqlDictionary, lookupDictinary);
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
        public static ActionResult AfterWebDialog(Session session)
        {
            return WebsiteCustomAction.AfterWebDialog(session);
        }

        [CustomAction]
        public static ActionResult AfterInstallInitialize_WEB(Session session)
        {
            return WebsiteCustomAction.AfterInstallInitialize_WEB(session);
        }

        [CustomAction]
        public static ActionResult CreateWebsite(Session session)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, "PM");
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

        [CustomAction]
        public static ActionResult PatchDatabase(Session sesion)
        {
            return DatabaseCustomAction.PatchDatabase(sesion, Properties.Resources.patchbas_1_2);
        }
    }
}
