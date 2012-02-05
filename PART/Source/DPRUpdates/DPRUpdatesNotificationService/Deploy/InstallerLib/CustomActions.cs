using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using System.Data.SqlClient;
using CprBroker.Installers;
using CprBroker.Utilities;
using DPRUpdateLib;

namespace InstallerLib
{
    public partial class CustomActions
    {
        private static UpdateDetectionVariables _UpdateDetectionVariables = new DPRUpdateDetectionVariables();

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
        public static ActionResult DeployDatabase(Session session)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, _UpdateDetectionVariables.DatabaseFeatureName);
            var ddl = GetCreationDDL();
            DatabaseCustomAction.ExecuteDDL(ddl, databaseSetupInfo);
            DatabaseCustomAction.CreateDatabaseUser(databaseSetupInfo, new string[] { _UpdateDetectionVariables.StagingTableName });
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RollbackDatabase(Session session)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, _UpdateDetectionVariables.DatabaseFeatureName);
            DatabaseCustomAction.ExecuteDDL(Properties.Resources.drpbas, databaseSetupInfo);
            DatabaseCustomAction.DropDatabaseUser(databaseSetupInfo);
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveDatabase(Session session)
        {
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, _UpdateDetectionVariables.DatabaseFeatureName);
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
                DatabaseCustomAction.ExecuteDDL(Properties.Resources.drpbas, databaseSetupInfo);
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
        public static ActionResult InstallDprUpdatesService(Session session)
        {
            try
            {
                string appToken = RegisterApplicationInCprBroker(session);
                UpdateConfigFile(session, appToken);
                UpdateRegistry(session, appToken);
                InstallAndStartService(session);
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult RollbackDprUpdatesService(Session session)
        {
            return RemoveDprUpdatesService(session);
        }

        [CustomAction]
        public static ActionResult RemoveDprUpdatesService(Session session)
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
                StopService(session);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
            }
            try
            {
                UnInstallService(session);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
            }
            return ActionResult.Success;
        }

    }
}
