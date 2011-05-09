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
using System.DirectoryServices;

namespace CprBroker.Installers
{
    [Serializable]
    public partial class WebInstallationInfo
    {
        public bool CreateAsWebsite = false;
        public string WebsitePath;
        public string WebsiteName;
        public string VirtualDirectoryName;

        public static readonly string ServerRoot = "IIS://localhost/w3svc";

        public bool ApplicationInstalled;
        public string ApplicationPath;

        public string InstallDir { get; private set; }

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

        public string GetWebFolderPath()
        {
            return InstallDir + "Website\\";
        }

        public string GetWebConfigFilePath()
        {
            return GetWebFolderPath() + "Web.config";
        }

    }
}
