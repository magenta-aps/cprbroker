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
using CprBroker.Utilities;
using CprBroker.Data.DataProviders;
using CprBroker.Installers;
using CprBroker.Installers.EventBrokerInstallers;

namespace CprBrokerWixInstallers
{
    public partial class CprBrokerCustomActions
    {
        public static void PatchWebsite_1_3_0(Session session)
        {
            var types = new Type[]
            {
                typeof(CprBroker.Providers.CPRDirect.CPRDirectClientDataProvider), 
                typeof(CprBroker.Providers.CPRDirect.CPRDirectExtractDataProvider)
            };
            var webInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "CPR");
            var configFilePath = webInstallationInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath);

            // Add new node(s) for data providers
            Array.ForEach<Type>(
                types,
                type =>
                {
                    var dic = new Dictionary<string, string>();
                    dic["type"] = string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
                    CprBroker.Installers.Installation.AddSectionNode("add", dic, configFilePath, "dataProviders/knownTypes");
                }
            );
        }

        private static void PatcWebsite_2_1_1(Session session)
        {
            // This patch adds /PersonMasterService12 to the address of existing person master data providers for versions prior to 2.1.1
            try
            {
                // Load connection string and encryption keys
                var webInstallationInfo = WebInstallationInfo.CreateFromFeature(session, "CPR");
                var configFilePath = webInstallationInfo.GetWebConfigFilePath(EventBrokerCustomActions.PathConstants.CprBrokerWebsiteDirectoryRelativePath);
                var config = CprBroker.Installers.Installation.OpenConfigFile(configFilePath);
                DataProvider.EncryptionAlgorithm = DataProviderKeysSection.GetFromConfig(config);
                var connectionString = config.ConnectionStrings.ConnectionStrings["CprBroker.Config.Properties.Settings.CprBrokerConnectionString"].ConnectionString;

                using (var dataContext = new CprBroker.Data.DataProviders.DataProvidersDataContext(connectionString))
                {
                    var providers = dataContext.DataProviders.ToArray();
                    providers = providers
                        .Where(dp =>
                        {
                            var type = Type.GetType(dp.TypeName, false, true);
                            return
                                type != null
                                && string.Equals(type.Name, typeof(CprBroker.Providers.PersonMaster.PersonMasterDataProvider).Name);
                        }
                        ).ToArray();

                    foreach (var prov in providers)
                    {
                        var adr = prov["Address"];
                        if (!string.IsNullOrEmpty(adr))
                        {
                            if (!adr.EndsWith("/PersonMasterService12", StringComparison.InvariantCultureIgnoreCase))
                            {
                                prov["Address"] += "/PersonMasterService12";
                            }
                        }
                    }
                    dataContext.SubmitChanges();
                }
            }
            finally
            {
                // Unload encryption keys
                DataProvider.EncryptionAlgorithm = null;
            }
        }
    }
}
