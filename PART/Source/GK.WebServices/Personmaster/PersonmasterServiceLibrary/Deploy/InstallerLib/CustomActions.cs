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
        public static ActionResult TestConnection(Session session)
        {
            return DatabaseCustomAction.TestConnectionString(session);
        }

        [CustomAction]
        public static ActionResult DeployPersonMasterDatabase(Session session)
        {
            string encryptionPassword = "";
            string domainNamespace = "";

            string sql = Properties.Resources.crebas;
            sql = sql.Replace("<pm-cryptpassword>", encryptionPassword);
            sql = sql.Replace("<pm-namespace>", domainNamespace);

            return DatabaseCustomAction.DeployDatabase(session, sql, new KeyValuePair<string, string>[0]);
        }

        [CustomAction]
        public static ActionResult RollbackPersonMasterDatabase(Session session)
        {
            return DatabaseCustomAction.RemoveDatabase(session, false);
        }

        [CustomAction]
        public static ActionResult RemovePersonMasterDatabase(Session session)
        {
            return DatabaseCustomAction.RemoveDatabase(session, true);
        }

    }
}
