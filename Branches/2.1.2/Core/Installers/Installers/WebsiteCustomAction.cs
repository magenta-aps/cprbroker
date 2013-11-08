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
using System.Configuration.Install;
using System.DirectoryServices;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System.IO;
using CprBroker.Utilities;

namespace CprBroker.Installers
{
    public partial class WebsiteCustomAction
    {
        public static ActionResult AppSearch_WEB(Session session)
        {
            RunWebAction(
                session,
                featureName =>
                {
                    session.SetPropertyValue(WebInstallationInfo.FeaturePropertyName, featureName);
                    if (session.IsRemoving() || session.IsPatching() || session.IsOlderVersionDetected())
                    {
                        WebInstallationInfo.CopyRegistryToProperties(session, featureName);
                        WebInstallationInfo webInstallationInfo = WebInstallationInfo.CreateFromCurrentDetails(session);
                        WebInstallationInfo.AddFeatureDetails(session, webInstallationInfo);
                    }
                    else if (session.UiLevel() != InstallUILevel.Full)
                    {
                        bool allowPropNameWithoutFeature = WebInstallationInfo.GetWebFeatureNames(session).Length == 1;
                        WebInstallationInfo webInstallationInfo = WebInstallationInfo.CreateFromCurrentDetails(session, featureName, allowPropNameWithoutFeature);
                        if (ValidateWebProperties(session, webInstallationInfo))
                        {
                            WebInstallationInfo.AddFeatureDetails(session, webInstallationInfo);
                        }
                    }
                }
            );
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult PopulateWebsites(Session session)
        {
            bool multiWebSiteAllowed = GetIisMajorVersion() > 5;

            session["WEB_MultipleSitesAllowed"] = (multiWebSiteAllowed).ToString();
            if (!multiWebSiteAllowed)
            {
                session["WEB_CREATEASWEBSITE"] = "False";
            }

            DirectoryEntry w3svc = new DirectoryEntry(WebInstallationInfo.ServerRoot);
            List<DirectoryEntry> websites = new List<DirectoryEntry>();
            foreach (DirectoryEntry de in w3svc.Children)
            {
                if (de.SchemaClassName == "IIsWebServer")
                {
                    websites.Add(de);
                }
            }

            var sitesData = websites
                             .Select(site => new
                             {
                                 Name = site.Properties["ServerComment"].Value as string,
                                 Path = site.Path + "/Root"
                             })
                             .OrderBy(s => s.Name)
                             .ToArray();

            View lView = session.Database.OpenView("SELECT * FROM ComboBox");
            lView.Execute();
            for (int i = 0; i < sitesData.Length; i++)
            {
                var siteData = sitesData[i];
                Record record = session.Database.CreateRecord(4);
                record.SetString(1, "WEB_SITENAME_VDIR");
                record.SetInteger(2, i + 1);
                record.SetString(3, siteData.Name);
                record.SetString(4, siteData.Name);

                lView.Modify(ViewModifyMode.InsertTemporary, record);
            }
            if (sitesData.Length > 0)
            {
                session["WEB_SITENAME_VDIR"] = sitesData[0].Name;
            }
            session["WEB_SitesFound"] = sitesData.Length > 0 ? sitesData.Length.ToString() : "";
            return ActionResult.Success;
        }

        public static ActionResult PreWebDialog(Session session)
        {
            var featureName = session.GetPropertyValue(WebInstallationInfo.FeaturePropertyName);
            var webInstallationInfo = WebInstallationInfo.CreateFromFeature(session, featureName);
            if (webInstallationInfo != null)
            {
                webInstallationInfo.CopyToCurrentDetails(session);
            }
            else
            {
                WebInstallationInfo.SetSuggestedPropertyValues(session, featureName);
            }
            // Necessary step for WebSiteOrDirectoryDialog
            session.SetPropertyValue("WEB_SITENAME_SITE", session.GetPropertyValue(WebInstallationInfo.Constants.WebsiteName));
            session.SetPropertyValue("WEB_SITENAME_VDIR", session.GetPropertyValue(WebInstallationInfo.Constants.VirtualDirectoryName));
            return ActionResult.Success;
        }

        public static ActionResult AfterWebDialog(Session session)
        {
            WebInstallationInfo webInstallationInfo = WebInstallationInfo.CreateFromCurrentDetails(session);
            ValidateWebProperties(session, webInstallationInfo);
            return ActionResult.Success;
        }

        static bool ValidateWebProperties(Session session, WebInstallationInfo webInstallationInfo)
        {
            string message;
            if (webInstallationInfo.Validate(out message))
            {
                session["WEB_Valid"] = "True";
                WebInstallationInfo.AddFeatureDetails(session, webInstallationInfo);
                return true;
            }
            else
            {
                session["WEB_Valid"] = message;
                return false;
            }
        }

        public static ActionResult AfterInstallInitialize_WEB(Session session, string[] extraCustomActionNames)
        {
            RunWebAction(
                session,
                (featureName) =>
                {
                    WebInstallationInfo webInstallationInfo = WebInstallationInfo.CreateFromFeature(session, featureName);
                    webInstallationInfo.CopyToCurrentDetails(session);
                    WebInstallationInfo.AddRegistryEntries(session, featureName);
                }
            );
            var aggregatedProps = WebInstallationInfo.GetCustomActionData(session).ToString();
            session.SetPropertyValue("RollbackWebsite", aggregatedProps);
            session.SetPropertyValue("CreateWebsite", aggregatedProps);
            session.SetPropertyValue("RemoveWebsite", aggregatedProps);
            session.SetPropertyValue("PatchWebsite", aggregatedProps);
            foreach (var customActionName in extraCustomActionNames)
            {
                session.SetPropertyValue(customActionName, aggregatedProps);
            }
            return ActionResult.Success;
        }

        static void RunWebAction(Session session, Action<string> func)
        {
            foreach (var featureName in WebInstallationInfo.GetWebFeatureNames(session))
            {
                func(featureName);
            }
        }

        public static ActionResult DeployWebsite(Session session, Dictionary<string, WebInstallationOptions> allOptions)
        {
            RunWebAction(session,
                featureName =>
                {
                    try
                    {
                        EnsureIISComponents();
                        var options = allOptions[featureName];
                        var webInstallationInfo = WebInstallationInfo.CreateFromFeature(session, featureName);
                        bool exists = webInstallationInfo.TargetEntryExists;
                        int siteID = webInstallationInfo.GetSiteId();
                        int scriptMapVersion;

                        RunRegIIS("-ir", options.FrameworkVersion);

                        if (webInstallationInfo.CreateAsWebsite)
                        {
                            WebsiteInstallationInfo websiteInstallationInfo = webInstallationInfo as WebsiteInstallationInfo;
                            using (DirectoryEntry machineRoot = new DirectoryEntry(WebInstallationInfo.ServerRoot))
                            {
                                string appPoolName = "";
                                if (DirectoryEntry.Exists(WebsiteInstallationInfo.AppPoolsRoot))
                                {
                                    using (DirectoryEntry appPools = new DirectoryEntry(WebsiteInstallationInfo.AppPoolsRoot))
                                    {
                                        appPoolName = webInstallationInfo.WebsiteName;
                                        bool appPoolExixts = DirectoryEntry.Exists(websiteInstallationInfo.AppPoolWmiPath);
                                        using (DirectoryEntry appPool = appPoolExixts ? new DirectoryEntry(appPools.Path + "/" + websiteInstallationInfo.WebsiteName) : appPools.Invoke("Create", "IIsApplicationPool", websiteInstallationInfo.WebsiteName) as DirectoryEntry)
                                        {
                                            appPool.InvokeSet("AppPoolIdentityType", 2);//LocalSystem 0; LocalService 1; NetworkService 2;  Custom (user & pwd) 3;  ApplicationPoolIdentity 4
                                            appPool.InvokeSet("AppPoolAutoStart", true);
                                            if (GetIisMajorVersion() > 6)
                                            {
                                                appPool.InvokeSet("ManagedRuntimeVersion", string.Format("v{0}", options.FrameworkVersion.ToString(2)));
                                                appPool.InvokeSet("ManagedPipelineMode", 0); // Integrated 0; Classic 1
                                            }
                                            appPool.CommitChanges();
                                        }
                                    }
                                }
                                using (DirectoryEntry site = exists ? new DirectoryEntry(machineRoot.Path + "/" + siteID) : machineRoot.Invoke("Create", "IIsWebServer", siteID) as System.DirectoryServices.DirectoryEntry)
                                {
                                    site.Invoke("Put", "ServerComment", websiteInstallationInfo.WebsiteName);
                                    site.Invoke("Put", "KeyType", "IIsWebServer");
                                    site.Invoke("Put", "ServerBindings", "*:80:" + websiteInstallationInfo.WebsiteName);
                                    site.Invoke("Put", "ServerState", 2);
                                    site.Invoke("Put", "FrontPageWeb", 1);
                                    site.Invoke("Put", "DefaultDoc", "Default.aspx");
                                    site.Invoke("Put", "SecureBindings", "*:443:" + websiteInstallationInfo.WebsiteName);
                                    site.Invoke("Put", "ServerAutoStart", 1);
                                    site.Invoke("Put", "ServerSize", 1);
                                    site.Invoke("SetInfo");
                                    site.CommitChanges();

                                    WebInstallationInfo.AddFeatureDetails(session, webInstallationInfo);
                                    string siteRootPath = site.Path + "/Root";
                                    using (DirectoryEntry siteRoot = DirectoryEntry.Exists(siteRootPath) ? new DirectoryEntry(siteRootPath) : site.Children.Add("Root", "IIsWebVirtualDir"))
                                    {
                                        siteRoot.InvokeSet("Path", websiteInstallationInfo.GetWebFolderPath(options));
                                        siteRoot.InvokeSet("DefaultDoc", "Default.aspx");
                                        if (!string.IsNullOrEmpty(appPoolName))
                                            siteRoot.InvokeSet("AppPoolId", appPoolName);
                                        siteRoot.Invoke("AppCreate", true);
                                        siteRoot.CommitChanges();
                                        scriptMapVersion = GetScriptMapsVersion(siteRoot);
                                    }
                                }
                            }
                        }
                        else
                        {
                            VirtualDirectoryInstallationInfo virtualDirectoryInstallationInfo = webInstallationInfo as VirtualDirectoryInstallationInfo;
                            using (DirectoryEntry websiteEntry = new DirectoryEntry(virtualDirectoryInstallationInfo.WebsitePath))
                            {
                                using (DirectoryEntry applicationEntry = exists ? new DirectoryEntry(virtualDirectoryInstallationInfo.TargetWmiPath) : websiteEntry.Invoke("Create", "IIsWebVirtualDir", virtualDirectoryInstallationInfo.VirtualDirectoryName) as DirectoryEntry)
                                {
                                    applicationEntry.InvokeSet("Path", virtualDirectoryInstallationInfo.GetWebFolderPath(options));
                                    applicationEntry.Invoke("AppCreate", true);
                                    applicationEntry.InvokeSet("AppFriendlyName", virtualDirectoryInstallationInfo.VirtualDirectoryName);
                                    applicationEntry.InvokeSet("DefaultDoc", "Default.aspx");
                                    applicationEntry.CommitChanges();
                                    WebInstallationInfo.AddFeatureDetails(session, virtualDirectoryInstallationInfo);
                                }
                                scriptMapVersion = GetScriptMapsVersion(websiteEntry);
                            }
                        }

                        // Set ASP.NET to target framework version                
                        if (scriptMapVersion != options.FrameworkVersion.Major)
                        {
                            RunRegIIS(string.Format("-s {0}", webInstallationInfo.TargetWmiSubPath), options.FrameworkVersion);
                        }

                        // Mark as done
                        WebInstallationInfo.AddFeatureDetails(session, webInstallationInfo);

                        GrantConfigEncryptionAccess(options.FrameworkVersion);

                        var configFilePath = webInstallationInfo.GetWebConfigFilePath(options);
                        var appRelativePath = webInstallationInfo.GetAppRelativePath();

                        // Data provider keys
                        EncryptConfigSections(configFilePath, siteID.ToString(), appRelativePath, options.ConfigSectionGroupEncryptionOptions, options.FrameworkVersion);

                        // Logging flat file access
                        if (options.InitializeFlatFileLogging)
                        {
                            InitializeFlatFileLogging(configFilePath);
                        }

                        // Set and encrypt connection strings and enqueue their encryption
                        SetConnectionStrings(configFilePath, options.ConnectionStrings);
                        if (options.EncryptConnectionStrings)
                        {
                            EncryptConnectionStrings(siteID.ToString(), appRelativePath, options.FrameworkVersion);
                        }

                        // Add url to internet explorer trusted zone
                        var internetSecurityManager = InternetSecurityManager.CreateObject();
                        foreach (var url in webInstallationInfo.CalculateWebUrls())
                        {
                            AddUrlToLocalIntranet(url);
                        }
                    }
                    catch (InstallException ex)
                    {
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        throw new InstallException(Messages.AnErrorHasOccurredAndInstallationWillBeCancelled, ex);
                    }
                });
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RollbackWebsite(Session session)
        {
            return RemoveWebsite(session);
        }

        [CustomAction]
        public static ActionResult RemoveWebsite(Session session)
        {
            RunWebAction(
                session,
                featureName =>
                {
                    var webInstallationInfo = WebInstallationInfo.CreateFromFeature(session, featureName);
                    string applicationDirectoryPath = webInstallationInfo.TargetWmiPath;
                    if (DirectoryEntry.Exists(applicationDirectoryPath))
                    {
                        using (DirectoryEntry applicationEntry = new DirectoryEntry(applicationDirectoryPath))
                        {
                            applicationEntry.DeleteTree();
                        }
                    }
                    if (webInstallationInfo is WebsiteInstallationInfo)
                    {
                        WebsiteInstallationInfo websiteInstallationInfo = webInstallationInfo as WebsiteInstallationInfo;
                        if (DirectoryEntry.Exists(websiteInstallationInfo.AppPoolWmiPath))
                        {
                            using (DirectoryEntry appPoolEntry = new DirectoryEntry(websiteInstallationInfo.AppPoolWmiPath))
                            {
                                appPoolEntry.DeleteTree();
                            }
                        }
                    }
                });
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult PatchWebsite(Session session, Dictionary<string, WebPatchInfo[]> featurePatchInfos)
        {
            var version = session.GetDetectedOlderVersion();
            RunWebAction(
                session,
                (featureName) =>
                {
                    if (featurePatchInfos.ContainsKey(featureName))
                    {
                        var featurePatchInfo = WebPatchInfo.Merge(featurePatchInfos[featureName], version);
                        featurePatchInfo.PatchAction();
                    }
                }
            );
            return ActionResult.Success;
        }

    }
}
