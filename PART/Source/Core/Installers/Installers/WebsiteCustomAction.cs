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
        public static ActionResult DeployWebsite(Session session)
        {
            try
            {
                EnsureIISComponents();

                string websitePath = GetWebFolderPath(session);
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

                                    webInstallationInfo.ApplicationPath = site.Path;
                                    webInstallationInfo.CopyToSession(session);

                                    using (DirectoryEntry siteRoot = new DirectoryEntry(site.Path + "/Root"))
                                    {
                                        siteRoot.InvokeSet("Path", websitePath);
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
                            applicationEntry.InvokeSet("Path", websitePath);
                            applicationEntry.Invoke("AppCreate", true);
                            applicationEntry.InvokeSet("AppFriendlyName", webInstallationInfo.VirtualDirectoryName);
                            applicationEntry.InvokeSet("DefaultDoc", "Default.aspx");
                            applicationEntry.CommitChanges();
                            webInstallationInfo.ApplicationPath = applicationEntry.Path;
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
                webInstallationInfo.ApplicationInstalled = true;
                webInstallationInfo.CopyToSession(session);

                GrantConfigEncryptionAccess();

                var configFilePath = GetWebConfigFilePath(session);
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
        public static ActionResult RemoveWebSite(Session session)
        {
            var webInstallationInfo = WebInstallationInfo.FromSession(session);
            if (webInstallationInfo.ApplicationInstalled)
            {
                string applicationDirectoryPath = webInstallationInfo.ApplicationPath;
                if (DirectoryEntry.Exists(applicationDirectoryPath))
                {
                    using (DirectoryEntry applicationEntry = new DirectoryEntry(applicationDirectoryPath))
                    {
                        applicationEntry.DeleteTree();
                    }
                }
            }
            return ActionResult.Success;
        }

    }
}
