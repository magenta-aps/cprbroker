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
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using System.Data.SqlClient;
using CprBroker.Installers;

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
            try
            {
                DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, updateDetectionVariables.DatabaseFeatureName);
                var ddl = updateDetectionVariables.SubstituteDDL(Properties.Resources.cre_tbl);
                if (updateDetectionVariables.TriggersEnabled)
                {
                    ddl += updateDetectionVariables.SubstituteDDL(Properties.Resources.cre_trg);
                }
                if (!string.IsNullOrEmpty(updateDetectionVariables.ExtraDatabaseCreateDDL))
                {
                    ddl += updateDetectionVariables.ExtraDatabaseCreateDDL;
                }
                DatabaseCustomAction.ExecuteDDL(ddl, databaseSetupInfo);
                DatabaseCustomAction.CreateDatabaseUser(databaseSetupInfo, new string[] { updateDetectionVariables.StagingTableName });
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult RollbackDatabase(Session session, UpdateDetectionVariables updateDetectionVariables)
        {
            try
            {
                DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, updateDetectionVariables.DatabaseFeatureName);
                var ddl = updateDetectionVariables.SubstituteDDL(Properties.Resources.drp_tbl);
                if (updateDetectionVariables.TriggersEnabled)
                {
                    ddl += updateDetectionVariables.SubstituteDDL(Properties.Resources.drp_trg);
                }
                if (!string.IsNullOrEmpty(updateDetectionVariables.ExtraDatabaseDropDDL))
                {
                    ddl += updateDetectionVariables.ExtraDatabaseDropDDL;
                }
                DatabaseCustomAction.ExecuteDDL(ddl, databaseSetupInfo);
                DatabaseCustomAction.DropDatabaseUser(databaseSetupInfo);
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult RemoveDatabase(Session session, UpdateDetectionVariables updateDetectionVariables)
        {
            try
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
                    var ddl = updateDetectionVariables.SubstituteDDL(Properties.Resources.drp_tbl);
                    if (updateDetectionVariables.TriggersEnabled)
                    {
                        ddl += updateDetectionVariables.SubstituteDDL(Properties.Resources.drp_trg);
                    }
                    if (!string.IsNullOrEmpty(updateDetectionVariables.ExtraDatabaseDropDDL))
                    {
                        ddl += updateDetectionVariables.ExtraDatabaseDropDDL;
                    }

                    DatabaseCustomAction.ExecuteDDL(ddl, databaseSetupInfo);
                    DatabaseCustomAction.DropDatabaseUser(databaseSetupInfo);
                }
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult PatchDatabase(Session session, Dictionary<string, DatabasePatchInfo[]> featurePatchInfos)
        {
            try
            {
                return DatabaseCustomAction.PatchDatabase(session, featurePatchInfos);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
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
                UninstallServiceByName(session, updateDetectionVariables);
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
        public static ActionResult RollbackUpdatesService(Session session, UpdateDetectionVariables updateDetectionVariables)
        {
            try
            {
                return RemoveUpdatesService(session, updateDetectionVariables);
            }
            catch (Exception ex)
            {
                session.ShowErrorMessage(ex);
                throw ex;
            }
        }

        [CustomAction]
        public static ActionResult RemoveUpdatesService(Session session, UpdateDetectionVariables updateDetectionVariables)
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

        [CustomAction]
        public static ActionResult AfterInstallInitialize_Product(Session session)
        {
            return ProductCustomActions.AfterInstallInitialize_Product(session);
        }

        [CustomAction]
        public static ActionResult ForgetOlderVersions(Session session)
        {
            return ProductCustomActions.ForgetOlderVersions(session);
        }

    }
}
