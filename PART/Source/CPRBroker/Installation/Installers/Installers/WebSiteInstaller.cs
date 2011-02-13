using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.DirectoryServices;
using CprBroker.Engine.Util;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace CprBroker.Installers
{
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
            savedStateWrapper.WebInstallationInfo = new WebInstallationInfo() { WebsiteName = DefaultWebsiteName };
            WebSiteForm form = new WebSiteForm() { InstallationInfo = savedStateWrapper.WebInstallationInfo };
            CprBroker.Installers.BaseForm.ShowAsDialog(form, this.InstallerWindowWrapper());
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            SavedStateWrapper savedStateWrapper = new SavedStateWrapper(stateSaver);
            try
            {
                base.Install(stateSaver);

                this.EnsureIISComponents();

                string websitePath = this.GetWebFolderPath();
                bool exists = savedStateWrapper.WebInstallationInfo.TargetEntryExists;

                if (savedStateWrapper.WebInstallationInfo.CreateAsWebsite)
                {
                    using (DirectoryEntry machineRoot = new DirectoryEntry(WebInstallationInfo.ServerRoot))
                    {
                        int siteID = savedStateWrapper.WebInstallationInfo.GetSiteId();

                        using (DirectoryEntry site = exists ? new DirectoryEntry(machineRoot.Path + "/" + siteID) : machineRoot.Invoke("Create", "IIsWebServer", siteID) as System.DirectoryServices.DirectoryEntry)
                        {
                            site.Invoke("Put", "ServerComment", savedStateWrapper.WebInstallationInfo.WebsiteName);
                            site.Invoke("Put", "KeyType", "IIsWebServer");
                            site.Invoke("Put", "ServerBindings", "*:80:" + savedStateWrapper.WebInstallationInfo.WebsiteName);
                            site.Invoke("Put", "ServerState", 2);
                            site.Invoke("Put", "FrontPageWeb", 1);
                            site.Invoke("Put", "DefaultDoc", "Default.aspx");
                            site.Invoke("Put", "SecureBindings", "*:443:" + savedStateWrapper.WebInstallationInfo.WebsiteName);
                            site.Invoke("Put", "ServerAutoStart", 1);
                            site.Invoke("Put", "ServerSize", 1);
                            site.Invoke("SetInfo");
                            site.CommitChanges();

                            savedStateWrapper.ApplicationPath = site.Path;

                            using (DirectoryEntry siteRoot = new DirectoryEntry(site.Path + "/Root"))
                            {
                                siteRoot.InvokeSet("Path", websitePath);
                                siteRoot.InvokeSet("DefaultDoc", "Default.aspx");
                                siteRoot.Invoke("AppCreate", true);
                                siteRoot.CommitChanges();
                            }
                        }
                    }
                }
                else
                {
                    using (DirectoryEntry websiteEntry = new DirectoryEntry(savedStateWrapper.WebInstallationInfo.WebsitePath))
                    {
                        using (DirectoryEntry applicationEntry = exists ? new DirectoryEntry(savedStateWrapper.WebInstallationInfo.TargetVirtualDirectoryPath) : websiteEntry.Invoke("Create", "IIsWebVirtualDir", savedStateWrapper.WebInstallationInfo.VirtualDirectoryName) as DirectoryEntry)
                        {
                            applicationEntry.InvokeSet("Path", websitePath);
                            applicationEntry.Invoke("AppCreate", true);
                            applicationEntry.InvokeSet("AppFriendlyName", savedStateWrapper.WebInstallationInfo.VirtualDirectoryName);
                            applicationEntry.InvokeSet("DefaultDoc", "Default.aspx");
                            applicationEntry.CommitChanges();
                            savedStateWrapper.ApplicationPath = applicationEntry.Path;
                        }
                    }
                }

                // Set ASP.NET to version 2.0
                RunRegIIS("-i");

                string localSitePath = savedStateWrapper.ApplicationPath;
                localSitePath = localSitePath.Remove(0, "IIS://localhost".Length);
                RunRegIIS(string.Format("-s {0}", localSitePath));

                // Mark as done
                savedStateWrapper.ApplicationInstalled = true;
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
                Messages.ShowException(ex);
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
                Messages.ShowException(ex);
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
        private void RunRegIIS(string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Engine.Util.Installation.GetNetFrameworkDirectory() + "aspnet_regiis.exe";
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

        private void DeleteApplication(System.Collections.IDictionary savedState)
        {
            SavedStateWrapper savedStateWrapper = new SavedStateWrapper(savedState);
            if (savedStateWrapper.ApplicationInstalled)
            {
                string applicationDirectoryPath = savedStateWrapper.ApplicationPath;
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
