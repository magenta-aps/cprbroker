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
using System.Text.RegularExpressions;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;

using CprBroker.Installers;

namespace CprBrokerWixInstallers
{
    public class RegistryCustomActions
    {
        private static readonly string[] FeatureSuffices = new string[] { "CPR", "EVENT" };

        private static string GetRegistryKey(string key, string suffix, Session session)
        {
            var path = string.Format("HKEY_LOCAL_MACHINE\\{0}\\{1}", key, suffix);
            string pat = @"\[(?<propName>\w+)\]";
            foreach (Match match in Regex.Matches(path, pat))
            {
                string propName = match.Groups["propName"].Value;
                string propValue = session.GetPropertyValue(propName);
                path = path.Replace(match.Value, propValue);
            }
            return path;
        }

        [CustomAction]
        public static ActionResult RunRegistrySearch(Session session)
        {
            View regLocatorView = session.Database.OpenView("SELECT * FROM RegLocator");
            regLocatorView.Execute();

            View appSearchView = session.Database.OpenView("SELECT * FROM AppSearch");
            appSearchView.Execute();

            var properties =
                from regLocatorRecord in regLocatorView.AsQueryable()
                join appSearchRecord in appSearchView.AsQueryable()
                on regLocatorRecord["Signature_"].ToString() equals appSearchRecord["Signature_"]
                select new
                {
                    Property = appSearchRecord["Property"].ToString(),
                    Key = regLocatorRecord["Key"].ToString(),
                    Name = regLocatorRecord["Name"].ToString()
                };

            foreach (string suffix in FeatureSuffices)
            {
                foreach (var registryMap in properties)
                {
                    string key = GetRegistryKey(registryMap.Key, suffix, session);
                    var propValue = Microsoft.Win32.Registry.GetValue(key, registryMap.Name, "").ToString();
                    session[registryMap.Property] = propValue;
                }
                session.DoAction("CA_Set_DB_AllProperties");
                session["DB_ALLPROPERTIES_" + suffix] = session["DB_AllProperties"];

                session.DoAction("CA_Set_WEB_AllProperties");
                session["WEB_ALLPROPERTIES_" + suffix] = session["WEB_AllProperties"];
            }
            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult PopulateRegistryEntries(Session session)
        {
            string pat = @"\A\[(?<propName>\w+)\]\Z";
            var registryEntries =
                from registryEntry in Microsoft.Deployment.WindowsInstaller.Linq.Queryable.AsQueryable(session.Database).Registries.ToArray()
                where Regex.Match(registryEntry.Value, pat).Success
                select registryEntry;

            var keys = new List<string>();
            var names = new List<string>();
            var propNames = new List<string>();

            foreach (var registryRow in registryEntries)
            {
                keys.Add(registryRow.Key);
                names.Add(registryRow.Name);
                propNames.Add(Regex.Match(registryRow.Value, pat).Groups["propName"].Value);
            }
            session["REGISTRY_KEYS"] = string.Join(",", keys.ToArray());
            session["REGISTRY_NAMES"] = string.Join(",", names.ToArray());
            session["REGISTRY_PROPERTY_NAMES"] = string.Join(",", propNames.ToArray());
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RunWriteRegistryValues(Session session)
        {
            string[] keys = session.CustomActionData["REGISTRY_KEYS"].Split(',');
            string[] names = session.CustomActionData["REGISTRY_NAMES"].Split(',');
            string[] propNames = session.CustomActionData["REGISTRY_PROPERTY_NAMES"].Split(',');

            string suffix = session.CustomActionData["REGISTRY_SUFFIX"];

            for (int i = 0; i < keys.Length; i++)
            {
                string key = GetRegistryKey(keys[i], suffix, session);
                string name = names[i];
                string propName = propNames[i];

                var propValue = session.CustomActionData[propName];
                Microsoft.Win32.Registry.SetValue(key, name, propValue, Microsoft.Win32.RegistryValueKind.String);
            }
            return ActionResult.Success;
        }

    }
}
