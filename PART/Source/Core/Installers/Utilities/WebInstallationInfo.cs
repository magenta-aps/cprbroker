/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
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
using System.Xml.Serialization;
using System.DirectoryServices;

namespace CprBroker.Installers
{
    [Serializable]
    [XmlInclude(typeof(WebsiteInstallationInfo))]
    [XmlInclude(typeof(VirtualDirectoryInstallationInfo))]
    public abstract partial class WebInstallationInfo
    {
        public string FeatureName = "";
        // OK
        public static readonly string ServerRoot = "IIS://localhost/w3svc";
        [XmlIgnore]
        public bool CreateAsWebsite
        {
            get
            {
                return this is WebsiteInstallationInfo;
            }
        }

        public string WebsiteName;

        [XmlIgnore]
        public string InstallDir { get; private set; }

        [XmlIgnore]
        public virtual bool TargetEntryExists { get { throw new Exception(""); } }

        public int GetSiteId()
        {
            int siteID = 1;
            DirectoryEntry machineRoot = new DirectoryEntry(ServerRoot);
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

        public abstract string GetAppRelativePath();

        public string GetWebFolderPath(WebInstallationOptions options)
        {
            return GetWebFolderPath(options.WebsiteDirectoryRelativePath);
        }

        public string GetWebFolderPath(string websiteDirectoryRelativePath)
        {
            return Utilities.Strings.EnsureDirectoryEndSlash(InstallDir + websiteDirectoryRelativePath);
        }

        public string GetWebConfigFilePath(WebInstallationOptions options)
        {
            return GetWebConfigFilePath(options.WebsiteDirectoryRelativePath);
        }

        public string GetWebConfigFilePath(string websiteDirectoryRelativePath)
        {
            return GetWebFolderPath(websiteDirectoryRelativePath) + "Web.config";
        }


        [XmlIgnore]
        public abstract string TargetWmiPath { get; }
        [XmlIgnore]
        public abstract string TargetWmiSubPath { get; }

        // Not yet        

        [Obsolete]
        public bool ApplicationInstalled;


        protected abstract bool IsMatchingDirectoryEntry(DirectoryEntry e);

        public abstract bool Validate(out string message);

        protected string[] GetSiteHostHeaders(DirectoryEntry siteEntry)
        {
            siteEntry.RefreshCache();
            List<string> ret = new List<string>();
            var serverBindings = siteEntry.Properties["ServerBindings"].Value;
            object[] serverBindingsArray;
            if (serverBindings is string)
            {
                serverBindingsArray = new object[] { serverBindings };
            }
            else
            {
                serverBindingsArray = serverBindings as object[];
            }
            foreach (string serverBinding in serverBindingsArray)
            {
                string[] arr = serverBinding.Split(':');
                if (arr.Length == 3)
                {
                    string address = arr[0];
                    string port = arr[1];
                    string hostHeader = arr[2];

                    int dummy;
                    if (!int.TryParse(port, out dummy))
                    {
                        port = "80";
                    }
                    if (string.IsNullOrEmpty(hostHeader))
                    {
                        hostHeader = "localhost";
                    }

                    if (string.IsNullOrEmpty(address) || address == "*")
                    {
                        ret.Add(string.Format("{0}:{1}", hostHeader, port));
                    }
                    else
                    {
                        ret.Add(string.Format("{0}:{1}", address, port));
                    }
                }
            }
            return ret.ToArray();
        }

        public abstract string[] CalculateWebUrls();

    }
}
