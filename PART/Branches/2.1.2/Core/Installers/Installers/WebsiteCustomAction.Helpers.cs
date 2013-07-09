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
using System.Configuration.Install;
using System.DirectoryServices;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System.IO;
using CprBroker.Utilities;

namespace CprBroker.Installers
{
    public partial class WebsiteCustomAction
    {

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

        private static int GetIisMajorVersion()
        {
            var iisMajorVersion = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\INetStp", "MajorVersion", 0);
            return Convert.ToInt32(iisMajorVersion);
        }

        private static int GetScriptMapsVersion(DirectoryEntry site)
        {
            return GetScriptMapsVersion(site, ".aspx");
        }

        private static int GetScriptMapsVersion(DirectoryEntry site, string extension)
        {
            PropertyValueCollection vals = site.Properties["ScriptMaps"];
            foreach (string val in vals)
            {
                if (val.StartsWith(extension))
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

        private static void RunServiceModelReg(string args, Version frameworkVersion)
        {
            string fileName = Installation.GetNetFrameworkDirectory(frameworkVersion) + "Windows Communication Foundation\\ServiceModelReg.exe";
            // use aspnet_regiis for 64 bit machines whenever possible
            string fileName64 = fileName.Replace("Framework", "Framework64");
            if (File.Exists(fileName64))
            {
                fileName = fileName64;
            };

            Installation.RunCommand(fileName, args);
        }

        private static void RunRegIIS(string args, Version frameworkVersion)
        {
            string fileName = Installation.GetNetFrameworkDirectory(frameworkVersion) + "aspnet_regiis.exe";
            // use aspnet_regiis for 64 bit machines whenever possible
            string fileName64 = fileName.Replace("Framework", "Framework64");
            if (File.Exists(fileName64))
            {
                fileName = fileName64;
            };

            Installation.RunCommand(fileName, args);
        }


        private static void GrantConfigEncryptionAccess(Version frameworkVersion)
        {
            try
            {
                RunRegIIS("-pc \"NetFrameworkConfigurationKey\" -exp", frameworkVersion);
            }
            catch (Exception ex)
            {
                // Exception is not important because we run this step in case the key container does not exist
            }
            string[] users = new string[]
            {                
                "AUTHENTICATED USERS"
            };
            foreach (string user in users)
            {
                RunRegIIS(string.Format("-pa \"NetFrameworkConfigurationKey\" \"{0}\"", user), frameworkVersion);
            }
        }

        private static void CopyTypeAssemblyFileToNetFramework(Type t, Version frameworkVersion)
        {
            string path = Installation.GetNetFrameworkDirectory(frameworkVersion);
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

        private static void DeleteTypeAssemblyFileFromNetFramework(Type t, Version frameworkVersion)
        {
            string path = Installation.GetNetFrameworkDirectory(frameworkVersion) + Path.GetFileName(t.Assembly.Location);
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

        public static void EncryptConfigSections(string configFilePath, string site, string app, ConfigSectionGroupEncryptionOptions[] configSectionGroupOptions, Version frameworkVersion)
        {
            foreach (var sectionGroup in configSectionGroupOptions)
            {
                // Copy needed assemblies
                foreach (var section in sectionGroup.ConfigSectionEncryptionOptions)
                {
                    CopyTypeAssemblyFileToNetFramework(section.SectionType, frameworkVersion);
                }

                // Create section with custom method
                foreach (var section in sectionGroup.ConfigSectionEncryptionOptions)
                {
                    if (section.CustomMethod != null)
                    {
                        var config = Installation.OpenConfigFile(configFilePath);
                        section.CustomMethod(config);
                    }
                }

                // Encrypt sections
                foreach (var section in sectionGroup.ConfigSectionEncryptionOptions)
                {
                    if (section.CustomMethod != null)
                    {
                        RunRegIIS(string.Format("-pe \"{0}/{1}\" -site \"{2}\" -app \"{3}\"", sectionGroup.ConfigSectionGroupName, section.SectionName, site, app), frameworkVersion);
                    }
                }

                // Restore sections and delete files
                // TODO: Seems no longer needed
                foreach (var section in sectionGroup.ConfigSectionEncryptionOptions)
                {
                    if (section.CustomMethod == null && section.SectionNode != null)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic["name"] = section.SectionName;
                        dic["type"] = section.SectionType.AssemblyQualifiedName;

                        Installation.AddSectionNode("section", dic, configFilePath, string.Format("sectionGroup[@name='{0}']", sectionGroup.ConfigSectionGroupName));
                    }
                    DeleteTypeAssemblyFileFromNetFramework(section.SectionType, frameworkVersion);
                }
            }
        }

        private static void SetConnectionStrings(string configFilePath, Dictionary<string, string> connectionStrings)
        {
            foreach (KeyValuePair<string, string> connectionString in connectionStrings)
            {
                Installation.SetConnectionStringInConfigFile(configFilePath, connectionString.Key, connectionString.Value);
            }
        }

        private static void EncryptConnectionStrings(string site, string app, Version frameworkVersion)
        {
            RunRegIIS(string.Format("-pe \"connectionStrings\" -site \"{0}\" -app \"{1}\"", site, app), frameworkVersion);
        }

        private static void InitializeFlatFileLogging(string configFilePath)
        {
            Installation.SetFlatFileLogListenerAccessRights(configFilePath);
        }

        public static void AddUrlToLocalIntranet(string url)
        {
            uint zone;
            var manager = InternetSecurityManager.CreateObject();
            int result;
            result = manager.MapUrlToZone(url, out zone, (uint)InternetSecurityManager.SZM_FLAGS.SZM_CREATE);
            InternetSecurityManager.WinError.TestHResult(result);
            if (zone != (int)InternetSecurityManager.URLZONE.URLZONE_INTRANET)
            {
                result = manager.SetZoneMapping(zone, url, InternetSecurityManager.SZM_FLAGS.SZM_DELETE);
                InternetSecurityManager.WinError.TestHResult(result);
                result = manager.SetZoneMapping(InternetSecurityManager.URLZONE.URLZONE_INTRANET, url, InternetSecurityManager.SZM_FLAGS.SZM_CREATE);
                InternetSecurityManager.WinError.TestHResult(result);
            }
        }
    }
}

