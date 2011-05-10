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
using System.Configuration.Install;
using System.DirectoryServices;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System.IO;
using CprBroker.Engine;
using CprBroker.Utilities;

namespace CprBroker.Installers.Installers
{
    public partial class WebsiteCustomAction
    {
        [CustomAction]
        public static ActionResult PopulateWebSites(Session session)
        {
            DirectoryEntry w3svc = new DirectoryEntry(WebInstallationInfo.ServerRoot);

            bool multiWebSiteAllowed = Convert.ToInt32(w3svc.Properties["MaxConnections"].Value) == 0;
            session["WEB_MULTIPLESITESALLOWED"] = (multiWebSiteAllowed).ToString();
            if (!multiWebSiteAllowed)
            {
                session["WEB_CREATEASWEBSITE"] = "False";
            }

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
                record.SetString(1, "WEB_VIRTUALDIRECTORYSITEPATH");
                record.SetInteger(2, i + 1);
                record.SetString(3, siteData.Path);
                record.SetString(4, siteData.Name);

                lView.Modify(ViewModifyMode.InsertTemporary, record);
            }
            if (sitesData.Length > 0)
            {
                session["WEB_VIRTUALDIRECTORYSITEPATH"] = sitesData[0].Path;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CalculateWebsApplicationPath(Session session)
        {
            var webInstallationInfo = WebInstallationInfo.FromSession(session);
            if (webInstallationInfo.CreateAsWebsite)
            {
                int siteID = webInstallationInfo.GetSiteId(session);
                webInstallationInfo.ApplicationPath = WebInstallationInfo.ServerRoot + "/" + siteID;
            }
            else
            {
                webInstallationInfo.ApplicationPath = webInstallationInfo.TargetVirtualDirectoryPath;
            }
            webInstallationInfo.CopyToSession(session);
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult DeployWebsite(Session session)
        {
            try
            {
                EnsureIISComponents();

                var webInstallationInfo = WebInstallationInfo.FromSession(session);
                bool exists = webInstallationInfo.TargetEntryExists;

                int siteID = webInstallationInfo.GetSiteId(session);
                int scriptMapVersion;

                if (webInstallationInfo.CreateAsWebsite)
                {
                    using (DirectoryEntry machineRoot = new DirectoryEntry(WebInstallationInfo.ServerRoot))
                    {
                        using (DirectoryEntry appPools = new DirectoryEntry(machineRoot.Path + "/APPPOOLS"))
                        {
                            bool appPoolExixts = webInstallationInfo.AppPoolExists(webInstallationInfo.WebsiteName);
                            using (DirectoryEntry appPool = appPoolExixts ? new DirectoryEntry(appPools.Path + "/" + webInstallationInfo.WebsiteName) : appPools.Invoke("Create", "IIsApplicationPool", webInstallationInfo.WebsiteName) as DirectoryEntry)
                            {
                                appPool.InvokeSet("AppPoolIdentityType", 2);//LocalSystem 0; LocalService 1; NetworkService 2;  Custom (user & pwd) 3;  ApplicationPoolIdentity 4
                                appPool.InvokeSet("AppPoolAutoStart", true);
                                appPool.CommitChanges();

                                using (DirectoryEntry site = exists ? new DirectoryEntry(machineRoot.Path + "/" + siteID) : machineRoot.Invoke("Create", "IIsWebServer", siteID) as System.DirectoryServices.DirectoryEntry)
                                {
                                    site.Invoke("Put", "ServerComment", webInstallationInfo.WebsiteName);
                                    site.Invoke("Put", "KeyType", "IIsWebServer");
                                    site.Invoke("Put", "ServerBindings", "*:80:" + webInstallationInfo.WebsiteName);
                                    site.Invoke("Put", "ServerState", 2);
                                    site.Invoke("Put", "FrontPageWeb", 1);
                                    site.Invoke("Put", "DefaultDoc", "Default.aspx");
                                    site.Invoke("Put", "SecureBindings", "*:443:" + webInstallationInfo.WebsiteName);
                                    site.Invoke("Put", "ServerAutoStart", 1);
                                    site.Invoke("Put", "ServerSize", 1);
                                    site.Invoke("SetInfo");
                                    site.CommitChanges();


                                    webInstallationInfo.CopyToSession(session);

                                    using (DirectoryEntry siteRoot = new DirectoryEntry(site.Path + "/Root"))
                                    {
                                        siteRoot.InvokeSet("Path", webInstallationInfo.GetWebFolderPath());
                                        siteRoot.InvokeSet("DefaultDoc", "Default.aspx");
                                        siteRoot.InvokeSet("AppPoolId", appPool.Name);
                                        siteRoot.Invoke("AppCreate", true);
                                        siteRoot.CommitChanges();
                                        scriptMapVersion = GetScriptMapsVersion(siteRoot);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    using (DirectoryEntry websiteEntry = new DirectoryEntry(webInstallationInfo.WebsitePath))
                    {
                        using (DirectoryEntry applicationEntry = exists ? new DirectoryEntry(webInstallationInfo.TargetVirtualDirectoryPath) : websiteEntry.Invoke("Create", "IIsWebVirtualDir", webInstallationInfo.VirtualDirectoryName) as DirectoryEntry)
                        {
                            applicationEntry.InvokeSet("Path", webInstallationInfo.GetWebFolderPath());
                            applicationEntry.Invoke("AppCreate", true);
                            applicationEntry.InvokeSet("AppFriendlyName", webInstallationInfo.VirtualDirectoryName);
                            applicationEntry.InvokeSet("DefaultDoc", "Default.aspx");
                            applicationEntry.CommitChanges();
                            webInstallationInfo.CopyToSession(session);
                        }
                        scriptMapVersion = GetScriptMapsVersion(websiteEntry);
                    }
                }

                // Set ASP.NET to version 2.0
                if (scriptMapVersion < 2)
                {
                    RunRegIIS("-i");

                    string localSitePath = webInstallationInfo.ApplicationPath;
                    localSitePath = localSitePath.Remove(0, "IIS://localhost".Length);
                    RunRegIIS(string.Format("-s {0}", localSitePath));
                }

                // Mark as done
                webInstallationInfo.CopyToSession(session);

                GrantConfigEncryptionAccess();

                var configFilePath = webInstallationInfo.GetWebConfigFilePath();
                var appRelativePath = webInstallationInfo.GetAppRelativePath();

                // Data provider keys
                EncryptDataProviderKeys(configFilePath, siteID.ToString(), appRelativePath);

                // Logging flat file access
                InitializeFlatFileLogging(configFilePath);

                // Set connection strings and enqueue their encryption
                ConnectionStringsInstaller.RegisterCommitAction(configFilePath, () => EncryptConnectionStrings(siteID.ToString(), appRelativePath));
            }
            catch (InstallException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new InstallException(Messages.AnErrorHasOccurredAndInstallationWillBeCancelled, ex);
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RollbackWebsite(Session session)
        {
            return RemoveWebSite(session);
        }

        [CustomAction]
        public static ActionResult RemoveWebSite(Session session)
        {
            var webInstallationInfo = WebInstallationInfo.FromSession(session);
            string applicationDirectoryPath = webInstallationInfo.ApplicationPath;
            if (DirectoryEntry.Exists(applicationDirectoryPath))
            {
                using (DirectoryEntry applicationEntry = new DirectoryEntry(applicationDirectoryPath))
                {
                    applicationEntry.DeleteTree();
                }
            }
            return ActionResult.Success;
        }

    }
}
