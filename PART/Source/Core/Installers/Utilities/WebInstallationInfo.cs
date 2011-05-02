using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;

namespace CprBroker.Installers
{
    [Serializable]
    public class WebInstallationInfo
    {
        public bool CreateAsWebsite = false;
        public string WebsitePath;
        public string WebsiteName;
        public string VirtualDirectoryName;

        public string HostHeader;

        public static readonly string ServerRoot = "IIS://localhost/w3svc";

        public bool ApplicationInstalled;
        public string ApplicationPath;


        public bool TargetEntryExists
        {
            get
            {
                if (CreateAsWebsite)
                {
                    DirectoryEntry machineRoot = new DirectoryEntry("IIS://localhost/W3SVC");
                    foreach (DirectoryEntry e in machineRoot.Children)
                    {
                        if (
                                e.SchemaClassName == "IIsWebServer"
                                && e.Properties["ServerComment"].Value.ToString().ToLower() == WebsiteName.ToLower()
                           )
                            return true;
                    }
                    return false;
                }
                else
                {
                    return DirectoryEntry.Exists(TargetVirtualDirectoryPath);
                }
            }
        }

        public bool AppPoolExists(string name)
        {

            DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/W3SVC/APPPOOLS");
            foreach (DirectoryEntry child in appPools.Children)
            {
                if (child.Name.ToLower() == name.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        public string TargetVirtualDirectoryPath
        {
            get
            {
                return WebsitePath + "/" + VirtualDirectoryName;
            }
        }

        public int GetSiteId()
        {
            int siteID = 1;
            DirectoryEntry machineRoot = new DirectoryEntry("IIS://localhost/W3SVC");
            foreach (DirectoryEntry e in machineRoot.Children)
            {
                if (e.SchemaClassName == "IIsWebServer")
                {
                    int ID = Convert.ToInt32(e.Name);
                    if (e.Properties["ServerComment"].Value.ToString().ToLower() == WebsiteName.ToLower())
                    {
                        return ID;
                    }

                    if (ID >= siteID)
                    {
                        siteID = ID + 1;
                    }
                }
            }
            return siteID;
        }

        public int GetSiteId(Microsoft.Deployment.WindowsInstaller.Session session)
        {
            int maxIdPlusOne = -1;
            DirectoryEntry machineRoot = new DirectoryEntry("IIS://localhost/W3SVC");
            foreach (DirectoryEntry e in machineRoot.Children)
            {
                if (e.SchemaClassName == "IIsWebServer")
                {
                    int ID = Convert.ToInt32(e.Name);
                    if (CreateAsWebsite)
                    {
                        if (e.Properties["ServerComment"].Value.ToString().ToLower() == WebsiteName.ToLower())
                        {
                            return ID;
                        }

                        if (ID >= maxIdPlusOne)
                        {
                            maxIdPlusOne = ID + 1;
                        }
                    }
                    else
                    {
                        if (
                            (e.Path + "/Root").ToLower() == WebsitePath.ToLower()
                            || e.Path.ToLower() == WebsitePath.ToLower()
                            )
                        {
                            return ID;
                        }
                    }
                }
            }
            return maxIdPlusOne;
        }

        public string GetAppRelativePath()
        {
            if (this.CreateAsWebsite)
            {
                return "/";
            }
            else
            {
                return "/" + VirtualDirectoryName;
            }
        }

    }
}
