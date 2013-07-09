using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.DirectoryServices;
using CPRBroker.Engine.Util;
using System.Diagnostics;

namespace CPRBroker.SetupCpr
{
    [System.ComponentModel.RunInstaller(true)]
    public class WebSiteInstaller : Installer
    {
        private WebInstallationInfo InstallationInfo = new WebInstallationInfo();
        private static readonly string ApplicationPathKeyName = "ApplicationPath";

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            try
            {
                base.Install(stateSaver);
                WebSiteForm form = new WebSiteForm() { InstallationInfo = this.InstallationInfo };
                BaseForm.ShowAsDialog(form, this.InstallerWindowWrapper());

                string websitePath = this.GetWebFolderPath();
                bool exists = InstallationInfo.TargetEntryExists;

                if (InstallationInfo.CreateNewWebSite)
                {
                    using (DirectoryEntry machineRoot = new DirectoryEntry("IIS://localhost/W3SVC"))
                    {
                        int siteID = InstallationInfo.GetSiteId();

                        using (DirectoryEntry site = exists ? new DirectoryEntry(machineRoot.Path + "/" + siteID) : machineRoot.Invoke("Create", "IIsWebServer", siteID) as System.DirectoryServices.DirectoryEntry)
                        {
                            site.Invoke("Put", "ServerComment", InstallationInfo.WebSiteName);
                            site.Invoke("Put", "KeyType", "IIsWebServer");
                            site.Invoke("Put", "ServerBindings", "*:80:" + InstallationInfo.WebSiteName);
                            site.Invoke("Put", "ServerState", 2);
                            site.Invoke("Put", "FrontPageWeb", 1);
                            site.Invoke("Put", "DefaultDoc", "Pages/Default.aspx");
                            site.Invoke("Put", "SecureBindings", "*:443:" + InstallationInfo.WebSiteName);
                            site.Invoke("Put", "ServerAutoStart", 1);
                            site.Invoke("Put", "ServerSize", 1);
                            site.Invoke("SetInfo");
                            site.CommitChanges();

                            stateSaver[ApplicationPathKeyName] = site.Path;

                            using (DirectoryEntry siteRoot = new DirectoryEntry(site.Path + "/Root"))
                            {
                                siteRoot.InvokeSet("Path", websitePath);
                                siteRoot.InvokeSet("DefaultDoc", "Pages/Default.aspx");
                                siteRoot.Invoke("AppCreate", true);
                                siteRoot.CommitChanges();
                            }
                        }
                    }
                }
                else
                {
                    using (DirectoryEntry websiteEntry = new DirectoryEntry(InstallationInfo.WebSitePath))
                    {
                        using (DirectoryEntry applicationEntry = exists ? new DirectoryEntry(InstallationInfo.TargetVirtualDirectoryPath) : websiteEntry.Invoke("Create", "IIsWebVirtualDir", InstallationInfo.VirtualDirectoryName) as DirectoryEntry)
                        {
                            applicationEntry.InvokeSet("Path", websitePath);
                            applicationEntry.Invoke("AppCreate", true);
                            applicationEntry.InvokeSet("AppFriendlyName", InstallationInfo.VirtualDirectoryName);
                            applicationEntry.InvokeSet("DefaultDoc", "Pages/Default.aspx");
                            applicationEntry.CommitChanges();
                            stateSaver[ApplicationPathKeyName] = applicationEntry.Path;
                        }
                    }
                }

                // Set ASP.NET to version 2.0
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Engine.Util.Installation.GetNetFrameworkDirectory() + "aspnet_regiis.exe";
                string localSitePath = stateSaver[ApplicationPathKeyName].ToString();
                localSitePath = localSitePath.Remove(0, "IIS://localhost".Length);
                startInfo.Arguments = string.Format("-s {0}", localSitePath);
                Process regIisProcess = new Process();
                regIisProcess.StartInfo = startInfo;
                regIisProcess.Start();
                regIisProcess.WaitForExit();
            }
            catch (InstallException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new InstallException(Messages.AnErrorHasOccuredAndInstallationWillBeCancelled, ex);
            }
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            try
            {
                base.Uninstall(savedState);
                string applicationDirectoryPath = savedState[ApplicationPathKeyName].ToString();
                if (DirectoryEntry.Exists(applicationDirectoryPath))
                {
                    using (DirectoryEntry applicationEntry = new DirectoryEntry(applicationDirectoryPath))
                    {
                        applicationEntry.DeleteTree();
                    }
                }
            }
            catch (InstallException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new InstallException(Messages.AnErrorHasOccuredAndInstallationWillBeCancelled, ex);
            }
        }
    }
}
