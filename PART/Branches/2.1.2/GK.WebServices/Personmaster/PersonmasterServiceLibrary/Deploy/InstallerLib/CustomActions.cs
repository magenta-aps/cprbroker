/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
        public static ActionResult CalculateExecutionElevated(Session session)
        {
            return ProductCustomActions.CalculateExecutionElevated(session);
        }

        [CustomAction]
        public static ActionResult AfterInstallInitialize_Product(Session session)
        {
            try
            {
                return ProductCustomActions.AfterInstallInitialize_Product(session);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult ForgetOlderVersions(Session session)
        {
            try
            {
                return ProductCustomActions.ForgetOlderVersions(session);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

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
                return WebsiteCustomAction.AfterInstallInitialize_WEB(session, new string[] { "PatchPersonMasterDatabase" });
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        private static Dictionary<string, WebInstallationOptions> GetWebInstallationOptions(Session session)
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
            return allOptions;
        }

        [CustomAction]
        public static ActionResult CreateWebsite(Session session)
        {
            try
            {
                var webInstallationOptions = GetWebInstallationOptions(session);
                return WebsiteCustomAction.DeployWebsite(session, webInstallationOptions);
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
                var patchInfos = new Dictionary<string, DatabasePatchInfo[]>();
                patchInfos["PM"] = new DatabasePatchInfo[]{
                    new DatabasePatchInfo(new Version(1,2), Properties.Resources.patchbas_1_2, null),
                    new DatabasePatchInfo(new Version(2,0), Properties.Resources.patchbas_2_0, null)
                };
                return DatabaseCustomAction.PatchDatabase(session, patchInfos);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult PatchWebsite(Session session)
        {
            try
            {
                Action patch_2_0 = () =>
                {
                    var webInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "PM");
                    var webInstallationOptions = GetWebInstallationOptions(session);
                    var configFilePath = webInstallationInfo.GetWebConfigFilePath(webInstallationOptions["PM"]);
                    var dic = new Dictionary<string, string>();
                    dic["multipleSiteBindingsEnabled"] = "true";
                    CprBroker.Installers.Installation.AddSectionNode("serviceHostingEnvironment", dic, configFilePath, "system.serviceModel");
                };

                var infos = new Dictionary<string, WebPatchInfo[]>();
                infos["PM"] = new WebPatchInfo[]{
                    new WebPatchInfo() { Version = new Version(2, 0), PatchAction = patch_2_0 }
                };

                return WebsiteCustomAction.PatchWebsite(session, infos);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

    }
}
