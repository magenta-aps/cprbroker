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
using System.Data.SqlClient;

namespace E_MUpdatesNotification
{
    public partial class CustomActions
    {
        private static E_MUpdateDetectionVariables _UpdateDetectionVariables = new E_MUpdateDetectionVariables();

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
        public static ActionResult PatchDatabase(Session session)
        {
            var patchDDL_1_1 = _UpdateDetectionVariables.SubstituteDDL(UpdateLib.Properties.Resources.drp_trg);
            patchDDL_1_1 += Properties.Resources.create_EM_objects;

            var dic = new Dictionary<string, CprBroker.Installers.DatabasePatchInfo[]>();
            dic[_UpdateDetectionVariables.DatabaseFeatureName] = new CprBroker.Installers.DatabasePatchInfo[] { 
                new CprBroker.Installers.DatabasePatchInfo(new Version(1,1), patchDDL_1_1,null),
                new CprBroker.Installers.DatabasePatchInfo(new Version(1,2), Properties.Resources.patch_EM_objects_1_2,null)
            };
            return UpdateLib.CustomActions.PatchDatabase(session, dic);
        }

        [CustomAction]
        public static ActionResult ValidateCprBrokerServicesUrl(Session session)
        {
            return UpdateLib.CustomActions.ValidateCprBrokerServicesUrl(session);
        }

        [CustomAction]
        public static ActionResult InstallUpdatesService(Session session)
        {
            return UpdateLib.CustomActions.InstallUpdatesService(session, _UpdateDetectionVariables);
        }

        [CustomAction]
        public static ActionResult RollbackUpdatesService(Session session)
        {
            return UpdateLib.CustomActions.RollbackUpdatesService(session, _UpdateDetectionVariables);
        }

        [CustomAction]
        public static ActionResult RemoveUpdatesService(Session session)
        {
            return UpdateLib.CustomActions.RemoveUpdatesService(session, _UpdateDetectionVariables);
        }

        [CustomAction]
        public static ActionResult AfterInstallInitialize_Product(Session session)
        {
            return UpdateLib.CustomActions.AfterInstallInitialize_Product(session);
        }

        [CustomAction]
        public static ActionResult ForgetOlderVersions(Session session)
        {
            return UpdateLib.CustomActions.ForgetOlderVersions(session);
        }

    }
}
