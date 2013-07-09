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

namespace CprBroker.Installers
{
    public abstract class BaseSetupInfo
    {
        public static string GetRegistryProductRoot(Session session, bool includeLocalMachine)
        {
            var ret = string.Format(@"Software\{0}\{1}", session.GetPropertyValue(PropertyNames.Manufacturer), session.GetPropertyValue(PropertyNames.ProductName));
            if (includeLocalMachine)
                ret = @"HKEY_LOCAL_MACHINE\" + ret;
            return ret;
        }

        public static string GetRegistryPath(Session session, string subRoot, string featureName, bool includeLocalMachine)
        {
            return string.Format(@"{0}\{1}\{2}", GetRegistryProductRoot(session, includeLocalMachine), subRoot, featureName);
        }

        public static void AddRegistryEntries(Session session, string subRoot, Dictionary<string, string> propertyToRegistryMappings, string featureName, string componentName)
        {
            string key = GetRegistryPath(session, subRoot, featureName, false);
            View lView = session.Database.OpenView("SELECT * FROM Registry");
            lView.Execute();

            foreach (var map in propertyToRegistryMappings)
            {
                string valueName = map.Value;
                string valueValue = session.GetPropertyValue(map.Key);

                Record record = session.Database.CreateRecord(6);
                record.SetString(1, string.Format("REG_{0}_{1}_{2}", subRoot, featureName, valueName));
                record.SetInteger(2, 2);
                record.SetString(3, key);
                record.SetString(4, valueName);
                record.SetString(5, valueValue);
                record.SetString(6, componentName);
                lView.Modify(ViewModifyMode.InsertTemporary, record);
            }
        }

        public static void CopyRegistryToProperties(Session session, string subRoot, Dictionary<string, string> propertyToRegistryMappings, string featureName)
        {
            string key = GetRegistryPath(session, subRoot, featureName, true);
            string keyWithoutFeature = key.Replace("\\" + featureName, "");
            var allValues = new List<string>();

            foreach (var map in propertyToRegistryMappings)
            {
                string value = Microsoft.Win32.Registry.GetValue(key, map.Value, "") as string;
                if (string.IsNullOrEmpty(value))
                {
                    value = Microsoft.Win32.Registry.GetValue(keyWithoutFeature, map.Value, "") as string;
                }
                session.SetPropertyValue(map.Key, value);
            }
        }

        public static CustomActionData GetCustomActionData(Session session)
        {
            var ret = new CustomActionData();
            ret[PropertyNames.InstallDir] = session.GetPropertyValue(PropertyNames.InstallDir);
            ret[PropertyNames.Manufacturer] = session.GetPropertyValue(PropertyNames.Manufacturer);
            ret[PropertyNames.ProductName] = session.GetPropertyValue(PropertyNames.ProductName);
            ret[PropertyNames.Remove] = session.GetPropertyValue(PropertyNames.Remove);
            ret[PropertyNames.Patch] = session.GetPropertyValue(PropertyNames.Patch);
            ret[PropertyNames.OlderVersionDetected] = session.GetPropertyValue(PropertyNames.OlderVersionDetected);
            return ret;
        }

        public static void SetSuggestedPropertyValues(Session session, string featureName, string[] allFeatureNames, string[] allSuggestedNames, string[] propertyNames)
        {
            var index = Array.IndexOf<string>(allFeatureNames, featureName);
            if (index != -1)
            {
                if (index < allSuggestedNames.Length)
                {
                    string suggestedValue = allSuggestedNames[index];
                    foreach (var propName in propertyNames)
                    {
                        session.SetPropertyValue(propName, suggestedValue);
                    }

                }
            }
        }
    }
}
