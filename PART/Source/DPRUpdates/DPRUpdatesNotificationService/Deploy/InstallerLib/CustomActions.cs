using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Data.SqlClient;
using CprBroker.Installers;

namespace InstallerLib
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult TestDatabaseConnection(Session session)
        {
            return DatabaseCustomAction.TestConnectionString(session, false);
        }

        [CustomAction]
        public static ActionResult DeployDatabase(Session session)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.FromSession(session);
            DatabaseCustomAction.ExecuteDDL(Properties.Resources.crebas, databaseSetupInfo);
            DatabaseCustomAction.CreateDatabaseUser(databaseSetupInfo, new string[] { "T_DPRUpdateStaging" });
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RollbackDatabase(Session session)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.FromSession(session);
            DatabaseCustomAction.ExecuteDDL(Properties.Resources.drpbas, databaseSetupInfo);
            DatabaseCustomAction.DropDatabaseUser(databaseSetupInfo);
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveDatabase(Session session)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.FromSession(session);
            DatabaseCustomAction.ExecuteDDL(Properties.Resources.drpbas, databaseSetupInfo);
            DatabaseCustomAction.DropDatabaseUser(databaseSetupInfo);
            return ActionResult.Success;
        }

        public static ActionResult InstallService(Session session)
        {

            return ActionResult.Success;
        }
    }
}
