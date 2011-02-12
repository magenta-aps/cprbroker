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

namespace CprBroker.SetupCpr
{
    [System.ComponentModel.RunInstaller(true)]
    public class WebSiteInstaller : Installer
    {
        private WebInstallationInfo InstallationInfo = new WebInstallationInfo();
        private SavedStateWrapper SavedStateWrapper = null;


        public override void Install(System.Collections.IDictionary stateSaver)
        {
            try
            {
                base.Install(stateSaver);

                this.EnsureIISComponents();

                SavedStateWrapper = new SavedStateWrapper(stateSaver);

                WebSiteForm form = new WebSiteForm() { InstallationInfo = this.InstallationInfo };
                CprBroker.SetupDatabase.BaseForm.ShowAsDialog(form, this.InstallerWindowWrapper());

                string websitePath = this.GetWebFolderPath();
                bool exists = InstallationInfo.TargetEntryExists;

                if (InstallationInfo.CreateAsWebsite)
                {
                    using (DirectoryEntry machineRoot = new DirectoryEntry(WebInstallationInfo.ServerRoot))
                    {
                        int siteID = InstallationInfo.GetSiteId();

                        using (DirectoryEntry site = exists ? new DirectoryEntry(machineRoot.Path + "/" + siteID) : machineRoot.Invoke("Create", "IIsWebServer", siteID) as System.DirectoryServices.DirectoryEntry)
                        {
                            site.Invoke("Put", "ServerComment", InstallationInfo.WebsiteName);
                            site.Invoke("Put", "KeyType", "IIsWebServer");
                            site.Invoke("Put", "ServerBindings", "*:80:" + InstallationInfo.WebsiteName);
                            site.Invoke("Put", "ServerState", 2);
                            site.Invoke("Put", "FrontPageWeb", 1);
                            site.Invoke("Put", "DefaultDoc", "Default.aspx");
                            site.Invoke("Put", "SecureBindings", "*:443:" + InstallationInfo.WebsiteName);
                            site.Invoke("Put", "ServerAutoStart", 1);
                            site.Invoke("Put", "ServerSize", 1);
                            site.Invoke("SetInfo");
                            site.CommitChanges();

                            SavedStateWrapper.ApplicationPath = site.Path;

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
                    using (DirectoryEntry websiteEntry = new DirectoryEntry(InstallationInfo.WebsitePath))
                    {
                        using (DirectoryEntry applicationEntry = exists ? new DirectoryEntry(InstallationInfo.TargetVirtualDirectoryPath) : websiteEntry.Invoke("Create", "IIsWebVirtualDir", InstallationInfo.VirtualDirectoryName) as DirectoryEntry)
                        {
                            applicationEntry.InvokeSet("Path", websitePath);
                            applicationEntry.Invoke("AppCreate", true);
                            applicationEntry.InvokeSet("AppFriendlyName", InstallationInfo.VirtualDirectoryName);
                            applicationEntry.InvokeSet("DefaultDoc", "Default.aspx");
                            applicationEntry.CommitChanges();
                            SavedStateWrapper.ApplicationPath = applicationEntry.Path;
                        }
                    }
                }

                // Set ASP.NET to version 2.0
                RunRegIIS("-i");

                string localSitePath = SavedStateWrapper.ApplicationPath;
                localSitePath = localSitePath.Remove(0, "IIS://localhost".Length);
                RunRegIIS(string.Format("-s {0}", localSitePath));

                // Mark as done
                SavedStateWrapper.ApplicationInstalled = true;
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

        public override void Rollback(System.Collections.IDictionary savedState)
        {
            try
            {
                base.Rollback(savedState);
                SavedStateWrapper = new SavedStateWrapper(savedState);
                DeleteApplication();
            }
            catch (Exception ex)
            {
                Messages.ShowException(ex);
            }
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            try
            {
                base.Uninstall(savedState);
                SavedStateWrapper = new SavedStateWrapper(savedState);
                DeleteApplication();
            }
            catch (Exception ex)
            {
                Messages.ShowException(ex);
            }
        }

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

        private void DeleteApplication()
        {
            if (SavedStateWrapper.ApplicationInstalled)
            {
                string applicationDirectoryPath = SavedStateWrapper.ApplicationPath;
                if (DirectoryEntry.Exists(applicationDirectoryPath))
                {
                    using (DirectoryEntry applicationEntry = new DirectoryEntry(applicationDirectoryPath))
                    {
                        applicationEntry.DeleteTree();
                    }
                }
            }
        }
    }
}
