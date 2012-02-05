using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Data.SqlClient;

namespace DPRUpdatesNotification
{
    public partial class CustomActions
    {
        private static DPRUpdateDetectionVariables _UpdateDetectionVariables = new DPRUpdatesNotification.DPRUpdateDetectionVariables();

        [CustomAction]
        public static ActionResult AppSearch_DB(Session session)
        {
            return UpdateLib.CustomActions.AppSearch_DB(session);
        }

        [CustomAction]
        public static ActionResult AfterDatabaseDialog(Session session)
        {
            return UpdateLib.CustomActions.AfterDatabaseDialog(session);
        }

        [CustomAction]
        public static ActionResult AfterInstallInitialize_DB(Session session)
        {
            return UpdateLib.CustomActions.AfterInstallInitialize_DB(session);
        }

        [CustomAction]
        public static ActionResult DeployDatabase(Session session)
        {
            return UpdateLib.CustomActions.DeployDatabase(session, _UpdateDetectionVariables);
        }

        [CustomAction]
        public static ActionResult RollbackDatabase(Session session)
        {
            return UpdateLib.CustomActions.RollbackDatabase(session, _UpdateDetectionVariables);
        }

        [CustomAction]
        public static ActionResult RemoveDatabase(Session session)
        {
            return UpdateLib.CustomActions.RemoveDatabase(session, _UpdateDetectionVariables);
        }

        [CustomAction]
        public static ActionResult ValidateCprBrokerServicesUrl(Session session)
        {
            return UpdateLib.CustomActions.ValidateCprBrokerServicesUrl(session);
        }

        [CustomAction]
        public static ActionResult InstallDprUpdatesService(Session session)
        {
            return UpdateLib.CustomActions.InstallUpdatesService(session, _UpdateDetectionVariables);
        }

        [CustomAction]
        public static ActionResult RollbackDprUpdatesService(Session session)
        {
            return UpdateLib.CustomActions.RollbackDprUpdatesService(session, _UpdateDetectionVariables);
        }

        [CustomAction]
        public static ActionResult RemoveDprUpdatesService(Session session)
        {
            return UpdateLib.CustomActions.RemoveDprUpdatesService(session, _UpdateDetectionVariables);
        }

    }
}
