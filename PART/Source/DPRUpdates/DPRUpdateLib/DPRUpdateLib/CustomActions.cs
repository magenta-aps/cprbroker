using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using System.Data.SqlClient;
using CprBroker.Installers;
using CprBroker.Utilities;

namespace UpdateLib
{
    public partial class CustomActions
    {
        [CustomAction]
        public static ActionResult AppSearch_DB(Session session)
        {
            try
            {
                return DatabaseCustomAction.AppSearch_DB(session, false);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult AfterDatabaseDialog(Session session)
        {
            try
            {
                return DatabaseCustomAction.AfterDatabaseDialog(session, false);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult AfterInstallInitialize_DB(Session session)
        {
            try
            {
                return DatabaseCustomAction.AfterInstallInitialize_DB(session);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult DeployDatabase(Session session, UpdateDetectionVariables updateDetectionVariables)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, updateDetectionVariables.DatabaseFeatureName);
            var ddl = updateDetectionVariables.SubstituteDDL(Properties.Resources.crebas);
            DatabaseCustomAction.ExecuteDDL(ddl, databaseSetupInfo);
            DatabaseCustomAction.CreateDatabaseUser(databaseSetupInfo, new string[] { updateDetectionVariables.StagingTableName });
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RollbackDatabase(Session session, UpdateDetectionVariables udateDetectionVariables)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, udateDetectionVariables.DatabaseFeatureName);
            DatabaseCustomAction.ExecuteDDL(Properties.Resources.drpbas, databaseSetupInfo);
            DatabaseCustomAction.DropDatabaseUser(databaseSetupInfo);
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveDatabase(Session session, UpdateDetectionVariables updateDetectionVariables)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, updateDetectionVariables.DatabaseFeatureName);
            DropDatabaseForm dropDatabaseForm = new DropDatabaseForm()
            {
                SetupInfo = databaseSetupInfo,
                Text = "Drop database objects",
                QuestionText = "Should the database objects be removed?",
                NoText = "No, keep the database objects",
                YesText = "Yes, drop the database objects"
            };


            if (BaseForm.ShowAsDialog(dropDatabaseForm, session.InstallerWindowWrapper()) == DialogResult.Yes)
            {
                var ddl = updateDetectionVariables.SubstituteDDL(Properties.Resources.drpbas);
                DatabaseCustomAction.ExecuteDDL(ddl, databaseSetupInfo);
                DatabaseCustomAction.DropDatabaseUser(databaseSetupInfo);
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ValidateCprBrokerServicesUrl(Session session)
        {
            var service = CreateAdminServiceProxy(session);
            try
            {
                var response = service.ListAppRegistrations();
                session["CPRBROKERSERVICESURL_VALID"] = "True";
                if (int.Parse(response.StandardRetur.StatusKode) != 200)
                {
                    session["CPRBROKERSERVICESURL_VALID"] = response.StandardRetur.FejlbeskedTekst;
                }
            }
            catch (Exception ex)
            {
                session["CPRBROKERSERVICESURL_VALID"] = ex.Message;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult InstallUpdatesService(Session session, UpdateDetectionVariables updateDetectionVariables)
        {
            try
            {
                string appToken = RegisterApplicationInCprBroker(session);
                UpdateConfigFile(session, updateDetectionVariables, appToken);
                UpdateRegistry(session, appToken);
                InstallAndStartService(session, updateDetectionVariables);
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult RollbackDprUpdatesService(Session session, UpdateDetectionVariables updateDetectionVariables)
        {
            return RemoveDprUpdatesService(session, updateDetectionVariables);
        }

        [CustomAction]
        public static ActionResult RemoveDprUpdatesService(Session session, UpdateDetectionVariables updateDetectionVariables)
        {
            try
            {
                UnregisterApplicationInCprBroker(session);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
            }
            try
            {
                StopService(session, updateDetectionVariables);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
            }
            try
            {
                UnInstallService(session, updateDetectionVariables);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
            }
            return ActionResult.Success;
        }

    }
}
