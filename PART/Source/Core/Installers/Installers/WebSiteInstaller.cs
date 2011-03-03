using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.DirectoryServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using CprBroker.Engine;
using CprBroker.Utilities;

namespace CprBroker.Installers
{
    /// <summary>
    /// Installs a website
    /// </summary>
    public class WebSiteInstaller : Installer
    {
        protected virtual string DefaultWebsiteName
        {
            get { return ""; }
        }

        #region Install
        protected override void OnBeforeInstall(System.Collections.IDictionary savedState)
        {
            GetInstallInfoFromUser(savedState);
        }
        public void GetInstallInfoFromUser(System.Collections.IDictionary stateSaver)
        {
            SavedStateWrapper savedStateWrapper = new SavedStateWrapper(stateSaver);
            var webInstallationInfo = new WebInstallationInfo()
            {
                VirtualDirectoryName = DefaultWebsiteName,
                WebsiteName = DefaultWebsiteName
            };
            WebSiteForm form = new WebSiteForm() { InstallationInfo = webInstallationInfo };
            CprBroker.Installers.BaseForm.ShowAsDialog(form, this.InstallerWindowWrapper());
            savedStateWrapper.SetWebInstallationInfo(webInstallationInfo);
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            SavedStateWrapper savedStateWrapper = new SavedStateWrapper(stateSaver);
            try
            {
                base.Install(stateSaver);

                this.EnsureIISComponents();

                string websitePath = this.GetWebFolderPath();
                var webInstallationInfo = savedStateWrapper.GetWebInstallationInfo();
                bool exists = webInstallationInfo.TargetEntryExists;

                int siteID = webInstallationInfo.GetSiteId();
                int scriptMapVersion;

                if (webInstallationInfo.CreateAsWebsite)
                {
                    using (DirectoryEntry machineRoot = new DirectoryEntry(WebInstallationInfo.ServerRoot))
                    {
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
                            savedStateWrapper.SetWebInstallationInfo(webInstallationInfo);

                            using (DirectoryEntry siteRoot = new DirectoryEntry(site.Path + "/Root"))
                            {
                                siteRoot.InvokeSet("Path", websitePath);
                                siteRoot.InvokeSet("DefaultDoc", "Default.aspx");
                                siteRoot.Invoke("AppCreate", true);
                                siteRoot.CommitChanges();
                                scriptMapVersion = GetScriptMapsVersion(siteRoot);
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
                            savedStateWrapper.SetWebInstallationInfo(webInstallationInfo);
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
                savedStateWrapper.SetWebInstallationInfo(webInstallationInfo);

                GrantConfigEncryptionAccess();

                var configFilePath = this.GetWebConfigFilePathFromInstaller();
                var appRelativePath = webInstallationInfo.GetAppRelativePath();

                // Data provider keys
                EncryptDataProviderKeys(configFilePath, siteID.ToString(), appRelativePath);

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
        }

        #endregion

        #region Rollback
        public override void Rollback(System.Collections.IDictionary savedState)
        {
            try
            {
                base.Rollback(savedState);
                DeleteApplication(savedState);
            }
            catch (Exception ex)
            {
                Messages.ShowException(this, "", ex);
            }
        }
        #endregion

        #region Uninstall

        public void GetUnInstallInfoFromUser(System.Collections.IDictionary stateSaver)
        {

        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            try
            {
                base.Uninstall(savedState);
                DeleteApplication(savedState);
            }
            catch (Exception ex)
            {
                Messages.ShowException(this, "", ex);
            }
        }
        #endregion

        #region Utility methods

        /// <summary>
        /// Tries to access IIS via directory services to make sure that all IIS components are installed
        /// Throws exception if failed
        /// </summary>
        private void EnsureIISComponents()
        {
            try
            {
                using (DirectoryEntry machineRoot = new DirectoryEntry(WebInstallationInfo.ServerRoot))
                {
                    Guid id = machineRoot.Guid;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.InstallerWindowWrapper(), Messages.MissingIISComponents, Messages.Unsuccessful);
                throw new InstallException("", ex);
            }
        }

        int GetScriptMapsVersion(DirectoryEntry site)
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

        private void RunRegIIS(string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Installation.GetNetFrameworkDirectory() + "aspnet_regiis.exe";
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

        private void GrantConfigEncryptionAccess()
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

        void CopyTypeAssemblyFileToNetFramework(Type t)
        {
            string path = Installation.GetNetFrameworkDirectory();
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
        void DeleteTypeAssemblyFileFroNetFramework(Type t)
        {
            string path = Installation.GetNetFrameworkDirectory() + Path.GetFileName(t.Assembly.Location);
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

        public void EncryptDataProviderKeys(string configFilePath, string site, string app)
        {
            try
            {
                //var defNode = DataProviderKeysSection.CreateXmlSectionDefinitionNode();
                //var sectionNode = DataProviderKeysSection.CreateNewXmlSectionNode();
                CopyTypeAssemblyFileToNetFramework(typeof(DataProviderKeysSection));
                CopyTypeAssemblyFileToNetFramework(typeof(DataProvidersConfigurationSection));

                Installation.RemoveSectionNode(configFilePath, DataProviderKeysSection.SectionName);
                var dataProvidersNode = Installation.RemoveSectionNode(configFilePath, DataProvidersConfigurationSection.SectionName);

                var config = Installation.OpenConfigFile(configFilePath);
                DataProviderKeysSection.RegisterInConfig(config);

                RunRegIIS(string.Format("-pe \"{0}/{1}\" -site \"{2}\" -app \"{3}\"", Constants.DataProvidersSectionGroupName, DataProviderKeysSection.SectionName, site, app));

                Installation.AddSectionNode(dataProvidersNode, configFilePath, Constants.DataProvidersSectionGroupName);

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic["name"] = DataProvidersConfigurationSection.SectionName;
                dic["type"] = typeof(DataProvidersConfigurationSection).AssemblyQualifiedName;

                Installation.AddSectionNode("section", dic, configFilePath, string.Format("sectionGroup[@name='{0}']", Constants.DataProvidersSectionGroupName));

                DeleteTypeAssemblyFileFroNetFramework(typeof(DataProviderKeysSection));
                DeleteTypeAssemblyFileFroNetFramework(typeof(DataProvidersConfigurationSection));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void EncryptConnectionStrings(string site, string app)
        {
            RunRegIIS(string.Format("-pe \"connectionStrings\" -site \"{0}\" -app \"{1}\"", site, app));
        }

        private void DeleteApplication(System.Collections.IDictionary savedState)
        {
            SavedStateWrapper savedStateWrapper = new SavedStateWrapper(savedState);
            var webInstallationInfo = savedStateWrapper.GetWebInstallationInfo();
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
        }

        #endregion

    }
}
