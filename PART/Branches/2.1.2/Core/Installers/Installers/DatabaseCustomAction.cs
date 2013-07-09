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
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using CprBroker.Utilities;

namespace CprBroker.Installers
{
    public static partial class DatabaseCustomAction
    {
        public static ActionResult AppSearch_DB(Session session, bool databaseShouldBeNew)
        {
            RunDatabaseAction(
                session,
                featureName =>
                {
                    session.SetPropertyValue(DatabaseSetupInfo.FeaturePropertyName, featureName);
                    if (session.IsRemoving() || session.IsPatching() || session.IsOlderVersionDetected())
                    {
                        DatabaseSetupInfo.CopyRegistryToProperties(session, featureName);
                        DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromCurrentDetails(session);
                        DatabaseSetupInfo.AddFeatureDetails(session, databaseSetupInfo);
                    }
                    else if (session.UiLevel() != InstallUILevel.Full)
                    {
                        bool allowPropNameWithoutFeature = DatabaseSetupInfo.GetDatabaseFeatureNames(session).Length == 1;
                        DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromCurrentDetails(session, featureName, allowPropNameWithoutFeature);
                        if (TestConnectionString(session, databaseSetupInfo, databaseShouldBeNew, false))
                        {
                            DatabaseSetupInfo.AddFeatureDetails(session, databaseSetupInfo);
                        }
                    }
                }
            );
            return ActionResult.Success;
        }

        public static ActionResult PreDatabaseDialog(Session session)
        {
            var featureName = session.GetPropertyValue(DatabaseSetupInfo.FeaturePropertyName);
            var databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, featureName);
            if (databaseSetupInfo != null)
            {
                databaseSetupInfo.CopyToCurrentDetails(session);
            }
            else
            {
                DatabaseSetupInfo.SetSuggestedPropertyValues(session, featureName);
            }
            return ActionResult.Success;
        }

        public static ActionResult AfterDatabaseDialog(Session session, bool databaseShouldBeNew)
        {
            var databaseSetupInfo = DatabaseSetupInfo.CreateFromCurrentDetails(session);
            databaseSetupInfo.UseExistingDatabase = false;

            if (TestConnectionString(session, databaseSetupInfo, databaseShouldBeNew, true))
            {
                DatabaseSetupInfo.AddFeatureDetails(session, databaseSetupInfo);
            }
            return ActionResult.Success;
        }

        public static ActionResult AfterInstallInitialize_DB(Session session)
        {
            RunDatabaseAction(
                session,
                (featureName) =>
                {
                    DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, featureName);
                    databaseSetupInfo.CopyToCurrentDetails(session);
                    DatabaseSetupInfo.AddRegistryEntries(session, featureName);
                }
            );
            var aggregatedProps = DatabaseSetupInfo.GetCustomActionData(session).ToString();
            session.SetPropertyValue("RollbackDatabase", aggregatedProps);
            session.SetPropertyValue("DeployDatabase", aggregatedProps);
            session.SetPropertyValue("RemoveDatabase", aggregatedProps);
            session.SetPropertyValue("PatchDatabase", aggregatedProps);
            return ActionResult.Success;
        }

        public static bool TestConnectionString(Session session, DatabaseSetupInfo dbInfo, bool databaseShouldBeNew, bool userInterfaceEnabled)
        {
            string message = "";
            dbInfo.UseExistingDatabase = false;
            session["DB_Valid"] = "False";

            Func<bool> asker = userInterfaceEnabled ?
                () => MessageBox.Show(session.InstallerWindowWrapper(), Messages.DatabaseAlreadyExistsDoYouWantToUseExisting, "", MessageBoxButtons.YesNo) == DialogResult.Yes
                : null as Func<bool>;

            if (dbInfo.Validate(ref message))
            {
                if (dbInfo.ValidateDatabaseExistence(databaseShouldBeNew, asker, ref message))
                {
                    session["DB_Valid"] = "True";
                    return true;
                }
                else
                {
                    session["DB_Valid"] = message;
                    if (userInterfaceEnabled && !databaseShouldBeNew)
                    {
                        MessageBox.Show(session.InstallerWindowWrapper(), message, "", MessageBoxButtons.OK);
                    }
                    return false;
                }
            }
            else
            {
                session["DB_Valid"] = message;
                if (userInterfaceEnabled)
                {
                    MessageBox.Show(session.InstallerWindowWrapper(), message, "", MessageBoxButtons.OK);
                }
                return false;
            }
        }

        static void RunDatabaseAction(Session session, Action<string> func)
        {
            foreach (var featureName in DatabaseSetupInfo.GetDatabaseFeatureNames(session))
            {
                func(featureName);
            }
        }

        public static ActionResult DeployDatabase(Session session, Dictionary<string, string> createDatabaseObjectsSql, Dictionary<string, KeyValuePair<string, string>[]> lookupDataArray, Dictionary<string, Action<SqlConnection>> customMethods = null)
        {
            customMethods = customMethods != null ? customMethods : new Dictionary<string, Action<SqlConnection>>();

            RunDatabaseAction(
                session,
                (featureName) =>
                {
                    DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.CreateFromFeature(session, featureName);
                    if (CreateDatabase(databaseSetupInfo, createDatabaseObjectsSql[featureName], lookupDataArray[featureName]))
                    {
                        ExecuteDDL(createDatabaseObjectsSql[featureName], databaseSetupInfo);
                        InsertLookups(lookupDataArray[featureName], databaseSetupInfo);
                        if (customMethods.ContainsKey(featureName))
                        {
                            RunCustomMethod(customMethods[featureName], databaseSetupInfo);
                        }
                    }
                    CreateDatabaseUser(databaseSetupInfo, null);
                }
            );
            return ActionResult.Success;
        }

        public static ActionResult RemoveDatabase(Session session, bool askUser)
        {
            RunDatabaseAction(
                session,
                (featureName) =>
                {
                    DatabaseSetupInfo setupInfo = DatabaseSetupInfo.CreateFromFeature(session, featureName);
                    if (!setupInfo.UseExistingDatabase)
                    {
                        DropDatabaseForm dropDatabaseForm = new DropDatabaseForm()
                        {
                            SetupInfo = setupInfo
                        };

                        if (!askUser || BaseForm.ShowAsDialog(dropDatabaseForm, session.InstallerWindowWrapper()) == DialogResult.Yes)
                        {
                            DropDatabase(setupInfo);
                            DropDatabaseUser(setupInfo);
                        }
                    }
                }
            );
            return ActionResult.Success;
        }

        public static ActionResult PatchDatabase(Session session, Dictionary<string, DatabasePatchInfo[]> featurePatchInfos)
        {
            var version = session.GetDetectedOlderVersion();

            RunDatabaseAction(
                session,
                (featureName) =>
                {
                    if (featurePatchInfos.ContainsKey(featureName))
                    {
                        var featurePatchInfo = DatabasePatchInfo.Merge(featurePatchInfos[featureName], version);

                        if (featurePatchInfo != null)
                        {
                            DatabaseSetupInfo setupInfo = DatabaseSetupInfo.CreateFromFeature(session, featureName);
                            PatchDatabaseForm patchDatabaseForm = new PatchDatabaseForm();
                            patchDatabaseForm.SetupInfo = setupInfo;

                            var dialogResult = BaseForm.ShowAsDialog(patchDatabaseForm, session.InstallerWindowWrapper());
                            if (dialogResult == DialogResult.Cancel)
                            {
                                throw new Exception("Cancelled");
                            }
                            else
                            {
                                ExecuteDDL(featurePatchInfo.SqlScript, patchDatabaseForm.SetupInfo);
                                RunCustomMethod(featurePatchInfo.PatchAction, patchDatabaseForm.SetupInfo);
                            }
                        }
                    }
                }
            );
            return ActionResult.Success;
        }

    }

}
