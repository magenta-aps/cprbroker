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
            return ActionResult.Success;
            return DatabaseCustomAction.TestConnectionString(session);
        }

        [CustomAction]
        public static ActionResult DeployDatabase(Session session)
        {
            return ActionResult.Success;
            string encryptionPassword = "";
            string domainNamespace = "";

            string sql = Properties.Resources.crebas;
            sql = sql.Replace("<pm-cryptpassword>", encryptionPassword);
            sql = sql.Replace("<pm-namespace>", domainNamespace);

            return DatabaseCustomAction.DeployDatabase(session, sql, new KeyValuePair<string, string>[0]);
        }

        [CustomAction]
        public static ActionResult RollbackDatabase(Session session)
        {
            return ActionResult.Success;
            return DatabaseCustomAction.RemoveDatabase(session, false);
        }

        [CustomAction]
        public static ActionResult RemoveDatabase(Session session)
        {
            return ActionResult.Success;
            return DatabaseCustomAction.RemoveDatabase(session, true);
        }

        [CustomAction]
        public static ActionResult PopulateWebSites(Session session)
        {
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CreateWebsite(Session session)
        {
            return ActionResult.Success;
            return WebsiteCustomAction.DeployWebsite(session,new Dictionary<string,string>());
        }

        [CustomAction]
        public static ActionResult RollbackWebsite(Session session)
        {
            return ActionResult.Success;
            return WebsiteCustomAction.RollbackWebsite(session);
        }

        [CustomAction]
        public static ActionResult RemoveWebSite(Session session)
        {
            return ActionResult.Success;
            return WebsiteCustomAction.RemoveWebSite(session);
        }

    }
}
