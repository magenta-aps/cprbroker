/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 *
 * The Initial Developer of the Original Code is
 * IT- og Telestyrelsen / Danish National IT and Telecom Agency.
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
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
using CprBroker.Data.Part;
using CprBroker.Data.Applications;
using CprBroker.Utilities;
using CprBroker.Engine;

namespace CprBrokerWixInstallers
{
    public class CprBrokerCustomActions
    {
        [CustomAction]
        public static ActionResult TestDatabaseConnection(Session session)
        {
            return DatabaseCustomAction.TestConnectionString(session);
        }

        [CustomAction]
        public static ActionResult CopyCustomActionDataToSession(Session session)
        {
            var cad = new CustomActionData(session["CopyCustomActionDataToSession"]);
            foreach (var propName in cad.Keys)
            {
                session[propName] = cad[propName];
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult DeployDatabase(Session session)
        {
            List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();
            ret.Add(new KeyValuePair<string, string>(typeof(CprBroker.Data.Part.AddressCoordinateQualityType).Name, Properties.Resources.AddressCoordinateQualityType));
            ret.Add(new KeyValuePair<string, string>(typeof(CprBroker.Data.Applications.Application).Name, Properties.Resources.Application));
            ret.Add(new KeyValuePair<string, string>(typeof(CivilStatusCodeType).Name, Properties.Resources.CivilStatusCodeType));
            ret.Add(new KeyValuePair<string, string>(typeof(ContactChannelType).Name, Properties.Resources.ContactChannelType));
            ret.Add(new KeyValuePair<string, string>(typeof(CountrySchemeType).Name, Properties.Resources.CountrySchemeType));
            ret.Add(new KeyValuePair<string, string>(typeof(Gender).Name, Properties.Resources.Gender));
            ret.Add(new KeyValuePair<string, string>(typeof(LifecycleStatus).Name, Properties.Resources.LifecycleStatus));
            ret.Add(new KeyValuePair<string, string>(typeof(LifeStatusCodeType).Name, Properties.Resources.LifeStatusCodeType));
            ret.Add(new KeyValuePair<string, string>(typeof(LogType).Name, Properties.Resources.LogType));
            ret.Add(new KeyValuePair<string, string>(typeof(RelationshipType).Name, Properties.Resources.RelationshipType));

            return DatabaseCustomAction.DeployDatabase(session, Properties.Resources.CreatePartDatabaseObjects, ret.ToArray());
        }

        [CustomAction]
        public static ActionResult RemoveDatabase(Session session)
        {
            return DatabaseCustomAction.RemoveDatabase(session, true);
        }

        [CustomAction]
        public static ActionResult RollbackDatabase(Session session)
        {
            return DatabaseCustomAction.RemoveDatabase(session, false);
        }

        [CustomAction]
        public static ActionResult PopulateWebsites(Session session)
        {
            return WebsiteCustomAction.PopulateWebsites(session);
        }

        [CustomAction]
        public static ActionResult ValidateWebProperties(Session session)
        {
            return WebsiteCustomAction.ValidateWebProperties(session);
        }

        [CustomAction]
        public static ActionResult CreateWebsite(Session session)
        {
            Dictionary<string, string> connectionStrings = new Dictionary<string, string>();
            DatabaseSetupInfo databaseSetupInfo = DatabaseSetupInfo.FromSession(session);

            connectionStrings["CprBroker.Config.Properties.Settings.CprBrokerConnectionString"] = databaseSetupInfo.CreateConnectionString(false, true);

            WebInstallationOptions options = new WebInstallationOptions()
            {
                EncryptConnectionStrings = true,
                ConnectionStrings = connectionStrings,
                InitializeFlatFileLogging = true,
                WebsiteDirectoryRelativePath = "CprBroker\\Website\\",
                ConfigSectionGroupEncryptionOptions = new ConfigSectionGroupEncryptionOptions[]
                {
                    new ConfigSectionGroupEncryptionOptions()
                    {
                        ConfigSectionGroupName = Constants.DataProvidersSectionGroupName,
                        ConfigSectionEncryptionOptions = new ConfigSectionEncryptionOptions[]
                        {
                            new ConfigSectionEncryptionOptions(){ SectionName = DataProviderKeysSection.SectionName, SectionType=typeof(DataProviderKeysSection), CustomMethod = config=>DataProviderKeysSection.RegisterInConfig(config)},
                            new ConfigSectionEncryptionOptions(){ SectionName = DataProvidersConfigurationSection.SectionName, SectionType=typeof(DataProvidersConfigurationSection),CustomMethod =null}
                        }
                    }
                }
            };
            return WebsiteCustomAction.DeployWebsite(session, options);
        }

        [CustomAction]
        public static ActionResult RollbackWebsite(Session session)
        {
            return WebsiteCustomAction.RollbackWebsite(session);
        }

        [CustomAction]
        public static ActionResult RemoveWebsite(Session session)
        {
            return WebsiteCustomAction.RemoveWebsite(session);
        }

    }
}
