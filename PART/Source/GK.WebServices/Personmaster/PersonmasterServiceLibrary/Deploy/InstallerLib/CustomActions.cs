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
        public static ActionResult AppSearch_DB(Session session)
        {
            try
            {
                return DatabaseCustomAction.AppSearch_DB(session, true);
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
                return DatabaseCustomAction.AfterDatabaseDialog(session, true);
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
            try
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
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult RollbackDatabase(Session session)
        {
            try
            {
                return DatabaseCustomAction.RemoveDatabase(session, false);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult RemoveDatabase(Session session)
        {
            try
            {
                return DatabaseCustomAction.RemoveDatabase(session, true);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult PopulateWebsites(Session session)
        {
            try
            {
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult AppSearch_WEB(Session session)
        {
            try
            {
                return WebsiteCustomAction.AppSearch_WEB(session);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult AfterWebDialog(Session session)
        {
            try
            {
                return WebsiteCustomAction.AfterWebDialog(session);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult AfterInstallInitialize_WEB(Session session)
        {
            try
            {
                return WebsiteCustomAction.AfterInstallInitialize_WEB(session, new string[0]);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult CreateWebsite(Session session)
        {
            try
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
                var allOptions = new Dictionary<string, WebInstallationOptions>();
                allOptions["PM"] = options;
                return WebsiteCustomAction.DeployWebsite(session, allOptions);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult RollbackWebsite(Session session)
        {
            try
            {
                return WebsiteCustomAction.RollbackWebsite(session);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult RemoveWebsite(Session session)
        {
            try
            {
                return WebsiteCustomAction.RemoveWebsite(session);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult PatchDatabase(Session session)
        {
            try
            {
                return DatabaseCustomAction.PatchDatabase(session, Properties.Resources.patchbas_1_2);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

    }
}
