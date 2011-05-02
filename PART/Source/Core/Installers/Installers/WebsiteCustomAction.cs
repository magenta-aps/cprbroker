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
    public class WebsiteCustomAction
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
                                 Path = site.Path
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
                var webInstallationInfo = GetWebInstallationInfo(session);
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
                                    //savedStateWrapper.SetWebInstallationInfo(webInstallationInfo);

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
                    using (DirectoryEntry websiteEntry = new DirectoryEntry(webInstallationInfo.WebsitePath + "/Root"))
                    {
                        using (DirectoryEntry applicationEntry = exists ? new DirectoryEntry(webInstallationInfo.TargetVirtualDirectoryPath) : websiteEntry.Invoke("Create", "IIsWebVirtualDir", webInstallationInfo.VirtualDirectoryName) as DirectoryEntry)
                        {
                            applicationEntry.InvokeSet("Path", websitePath);
                            applicationEntry.Invoke("AppCreate", true);
                            applicationEntry.InvokeSet("AppFriendlyName", webInstallationInfo.VirtualDirectoryName);
                            applicationEntry.InvokeSet("DefaultDoc", "Default.aspx");
                            applicationEntry.CommitChanges();
                            webInstallationInfo.ApplicationPath = applicationEntry.Path;
                            //savedStateWrapper.SetWebInstallationInfo(webInstallationInfo);
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
                //savedStateWrapper.SetWebInstallationInfo(webInstallationInfo);

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

        public static ActionResult RemoveWebSite(Session session)
        {
            var webInstallationInfo = GetWebInstallationInfo(session);
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

        /// <summary>
        /// Tries to access IIS via directory services to make sure that all IIS components are installed
        /// Throws exception if failed
        /// </summary>
        private static void EnsureIISComponents()
        {
            //try
            //{
            using (DirectoryEntry machineRoot = new DirectoryEntry(WebInstallationInfo.ServerRoot))
            {
                Guid id = machineRoot.Guid;
            }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(this.InstallerWindowWrapper(), Messages.MissingIISComponents, Messages.Unsuccessful);
            //    throw new InstallException("", ex);
            //}
        }

        public static string GetWebFolderPath(Session session)
        {
            return session["INSTALLDIR"] + "Web\\";
        }

        public static string GetWebConfigFilePath(Session session)
        {
            return GetWebFolderPath(session) + "Web.config";
        }

        private static WebInstallationInfo GetWebInstallationInfo(Session session)
        {
            WebInstallationInfo ret = new WebInstallationInfo();
            ret.CreateAsWebsite = session["WEB_CREATEASWEBSITE"] == "True";
            ret.ApplicationPath = null;
            ret.ApplicationInstalled = false;
            ret.VirtualDirectoryName = session["WEB_VIRTUALDIRECTORYNAME"];
            ret.WebsiteName = session["WEB_SITENAME"];
            ret.WebsitePath = session["WEB_VIRTUALDIRECTORYSITEPATH"];

            return ret;
        }

        private static void SetWebInstallationInfo(WebInstallationInfo webInstallationInfo, Session session)
        {
            session["WEB_CREATEASWEBSITE"] = webInstallationInfo.CreateAsWebsite.ToString();
            //webInstallationInfo.ApplicationPath = null;
            //webInstallationInfo.ApplicationInstalled = false;
            session["WEB_VIRTUALDIRECTORYNAME"] = webInstallationInfo.VirtualDirectoryName;
            session["WEB_SITENAME"] = webInstallationInfo.WebsiteName;
            session["WEB_VIRTUALDIRECTORYSITEPATH"] = webInstallationInfo.WebsitePath;
        }

        private static int GetScriptMapsVersion(DirectoryEntry site)
        {
            PropertyValueCollection vals = site.Properties["ScriptMaps"];
            foreach (string val in vals)
            {
                if (val.StartsWith(".aspx"))
                {
                    string framework = "Framework\\v";
                    int startIndex = val.IndexOf(framework);
                    if (startIndex != -1)
                    {
                        startIndex += framework.Length;
                        int endIndex = val.IndexOf(".", startIndex);
                        if (endIndex != -1)
                        {
                            string version = val.Substring(startIndex, endIndex - startIndex);
                            int intVersion;
                            if (int.TryParse(version, out intVersion))
                            {
                                return intVersion;
                            }
                        }
                    }
                }
            }
            return -1;
        }

        private static void RunRegIIS(string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Utilities.Installation.GetNetFrameworkDirectory() + "aspnet_regiis.exe";
            // use aspnet_regiis for 64 bit machines whenever possible
            string fileName64 = startInfo.FileName.Replace("Framework", "Framework64");
            if (File.Exists(fileName64))
            {
                startInfo.FileName = fileName64;
            }
            startInfo.Arguments = args;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            Process regIisProcess = new Process();
            regIisProcess.StartInfo = startInfo;
            regIisProcess.Start();
            regIisProcess.WaitForExit();
            if (regIisProcess.ExitCode != 0)
            {
                throw new InstallException(string.Format("Process '{0} {1}' failed", startInfo.FileName, startInfo.Arguments));
            }
        }

        private static void GrantConfigEncryptionAccess()
        {
            string[] users = new string[]
            {                
                "AUTHENTICATED USERS"
            };
            foreach (string user in users)
            {
                RunRegIIS(string.Format("-pa \"NetFrameworkConfigurationKey\" \"{0}\"", user));
            }
        }

        private static void CopyTypeAssemblyFileToNetFramework(Type t)
        {
            string path = Utilities.Installation.GetNetFrameworkDirectory();
            string fileName = Path.GetFileName(t.Assembly.Location);
            if (Directory.Exists(path))
            {
                File.Copy(t.Assembly.Location, path + fileName, true);
            }
            path = path.Replace("Framework", "Framework64");
            if (Directory.Exists(path))
            {
                File.Copy(t.Assembly.Location, path + fileName, true);
            }
        }

        private static void DeleteTypeAssemblyFileFroNetFramework(Type t)
        {
            string path = Utilities.Installation.GetNetFrameworkDirectory() + Path.GetFileName(t.Assembly.Location);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            path = path.Replace("Framework", "Framework64");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void EncryptDataProviderKeys(string configFilePath, string site, string app)
        {
            System.Windows.Forms.MessageBox.Show("Will Encrypt data providers");
            //try
            {
                CopyTypeAssemblyFileToNetFramework(typeof(DataProviderKeysSection));
                CopyTypeAssemblyFileToNetFramework(typeof(DataProvidersConfigurationSection));

                Utilities.Installation.RemoveSectionNode(configFilePath, DataProviderKeysSection.SectionName);
                var dataProvidersNode = Utilities.Installation.RemoveSectionNode(configFilePath, DataProvidersConfigurationSection.SectionName);

                var config = Utilities.Installation.OpenConfigFile(configFilePath);
                DataProviderKeysSection.RegisterInConfig(config);

                RunRegIIS(string.Format("-pe \"{0}/{1}\" -site \"{2}\" -app \"{3}\"", Constants.DataProvidersSectionGroupName, DataProviderKeysSection.SectionName, site, app));

                if (dataProvidersNode != null)
                {
                    Utilities.Installation.AddSectionNode(dataProvidersNode, configFilePath, Constants.DataProvidersSectionGroupName);
                }

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic["name"] = DataProvidersConfigurationSection.SectionName;
                dic["type"] = typeof(DataProvidersConfigurationSection).AssemblyQualifiedName;

                Utilities.Installation.AddSectionNode("section", dic, configFilePath, string.Format("sectionGroup[@name='{0}']", Constants.DataProvidersSectionGroupName));

                DeleteTypeAssemblyFileFroNetFramework(typeof(DataProviderKeysSection));
                DeleteTypeAssemblyFileFroNetFramework(typeof(DataProvidersConfigurationSection));
            }
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        private static void EncryptConnectionStrings(string site, string app)
        {
            RunRegIIS(string.Format("-pe \"connectionStrings\" -site \"{0}\" -app \"{1}\"", site, app));
        }

        private static void InitializeFlatFileLogging(string configFilePath)
        {
            Utilities.Installation.SetFlatFileLogListenerAccessRights(configFilePath);
        }
    }
}
