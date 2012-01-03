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

namespace CprBroker.Installers
{
    public abstract partial class WebInstallationInfo
    {
        public static WebInstallationInfo CreateFromCurrentDetails(Session session)
        {
            return CreateFromCurrentDetails(session, "");
        }
        public static WebInstallationInfo CreateFromCurrentDetails(Session session, string featureName)
        {
            if (!string.IsNullOrEmpty(featureName))
            {
                session.SetPropertyValue(FeaturePropertyName, featureName);
            }

            bool createAsWebsite = session.GetBooleanPropertyValue(Constants.CreateWebsite, featureName);
            if (createAsWebsite)
            {
                return new WebsiteInstallationInfo()
                {
                    FeatureName = session.GetPropertyValue(FeaturePropertyName),
                    WebsiteName = session.GetPropertyValue(Constants.WebsiteName, featureName),
                    InstallDir = session.GetInstallDirProperty()
                };
            }
            else
            {
                return new VirtualDirectoryInstallationInfo()
                {
                    FeatureName = session.GetPropertyValue(FeaturePropertyName),
                    VirtualDirectoryName = session.GetPropertyValue(Constants.VirtualDirectoryName, featureName),
                    WebsiteName = session.GetPropertyValue(Constants.WebsiteName, featureName),
                    InstallDir = session.GetInstallDirProperty()
                };
            }
        }

        public void CopyToCurrentDetails(Session session)
        {
            session.SetPropertyValue(FeaturePropertyName, this.FeatureName);

            if (this is WebsiteInstallationInfo)
            {
                WebsiteInstallationInfo websiteInstallationInfo = this as WebsiteInstallationInfo;
                session.SetPropertyValue(Constants.CreateWebsite, websiteInstallationInfo.CreateAsWebsite.ToString());
                session.SetPropertyValue(Constants.ApplicationPath, websiteInstallationInfo.TargetWmiPath);
                session.SetPropertyValue(Constants.VirtualDirectoryName, "");
                session.SetPropertyValue(Constants.WebsiteName, websiteInstallationInfo.WebsiteName);
                session.SetPropertyValue(Constants.VirtualDirectorySitePath, "");
            }
            else
            {
                VirtualDirectoryInstallationInfo virtualDirectoryInstallationInfo = this as VirtualDirectoryInstallationInfo;
                session.SetPropertyValue(Constants.CreateWebsite, virtualDirectoryInstallationInfo.CreateAsWebsite.ToString());
                session.SetPropertyValue(Constants.ApplicationPath, virtualDirectoryInstallationInfo.TargetWmiPath);
                session.SetPropertyValue(Constants.VirtualDirectoryName, virtualDirectoryInstallationInfo.VirtualDirectoryName);
                session.SetPropertyValue(Constants.WebsiteName, virtualDirectoryInstallationInfo.WebsiteName);
                session.SetPropertyValue(Constants.VirtualDirectorySitePath, virtualDirectoryInstallationInfo.WebsitePath);
            }
        }

        public static string[] GetWebFeatureNames(Session session)
        {
            return session.GetPropertyValue(AllFeaturesPropertyName).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static readonly string FeaturePropertyName = "WEB_Feature";
        public static readonly string AllFeaturesPropertyName = "WEB_FeatureNames";
        public static readonly string AllInfoPropertyName = "WEB_All";

        private static WebInstallationInfo[] DeserializeAllFeatures(Session session)
        {
            var allPropVal = session.GetPropertyValue(AllInfoPropertyName);
            if (allPropVal == "-")
                allPropVal = "";
            var allInfo = CprBroker.Utilities.Strings.Deserialize<WebInstallationInfo[]>(allPropVal);
            if (allInfo == null)
                allInfo = new WebInstallationInfo[0];
            return allInfo;
        }

        private static void SerializeAllFeatures(Session session, WebInstallationInfo[] allInfo)
        {
            var allPropVal = CprBroker.Utilities.Strings.SerializeObject(allInfo);
            session.SetPropertyValue(AllInfoPropertyName, allPropVal);
        }

        public static void AddFeatureDetails(Session session, WebInstallationInfo webInstallationInfo)
        {
            var allInfo = DeserializeAllFeatures(session);
            var index = Array.FindIndex<WebInstallationInfo>(allInfo, inf => inf.FeatureName == webInstallationInfo.FeatureName);
            if (index != -1)
            {
                allInfo[index] = webInstallationInfo;
            }
            else
            {
                var list = new List<WebInstallationInfo>(allInfo);
                list.Add(webInstallationInfo);
                allInfo = list.ToArray();
            }
            SerializeAllFeatures(session, allInfo);
        }

        public static WebInstallationInfo CreateFromFeature(Session session, string featureName)
        {
            var allPropValue = session.GetPropertyValue(AllInfoPropertyName);
            var allInfo = CprBroker.Utilities.Strings.Deserialize<WebInstallationInfo[]>(allPropValue);
            var ret = allInfo.Where(inf => inf.FeatureName == featureName).FirstOrDefault();
            if (ret != null && string.IsNullOrEmpty(ret.InstallDir))
            {
                ret.InstallDir = session.GetInstallDirProperty();
            }
            return ret;
        }

        public static string GetCustomActionData(Session session)
        {
            var commponProps = BaseSetupInfo.GetCustomActionData(session);
            var databaseProps = DatabaseSetupInfo.GetCustomActionData(session);
            var webProps = string.Format("{0}={1};{2}={3};{4}={5}",
                WebInstallationInfo.AllInfoPropertyName, session.GetPropertyValue(WebInstallationInfo.AllInfoPropertyName),
                WebInstallationInfo.FeaturePropertyName, session.GetPropertyValue(WebInstallationInfo.FeaturePropertyName),
                WebInstallationInfo.AllFeaturesPropertyName, session.GetPropertyValue(WebInstallationInfo.AllFeaturesPropertyName)
                );
            return string.Format("{0};{1};{2}", commponProps, webProps, databaseProps);
        }

    }
}
