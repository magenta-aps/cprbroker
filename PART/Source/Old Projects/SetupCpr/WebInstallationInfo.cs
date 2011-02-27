using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;

namespace CprBroker.SetupCpr
{
    public class WebInstallationInfo
    {
        public bool CreateAsWebsite = false;
        public string WebsitePath;
        public string WebsiteName;
        public string VirtualDirectoryName;

        public static readonly string ServerRoot = "IIS://localhost/w3svc";
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
    }
}
