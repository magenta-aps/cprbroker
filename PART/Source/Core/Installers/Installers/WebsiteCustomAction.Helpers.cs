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
using System.Configuration.Install;
using System.DirectoryServices;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System.IO;
using CprBroker.Engine;
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

        private static void RunRegIIS(string args, Version frameworkVersion)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Utilities.Installation.GetNetFrameworkDirectory(frameworkVersion) + "aspnet_regiis.exe";
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

        private static void GrantConfigEncryptionAccess(Version frameworkVersion)
        {
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
            string path = Utilities.Installation.GetNetFrameworkDirectory(frameworkVersion);
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

        private static void DeleteTypeAssemblyFileFromNetFramework(Type t,Version frameworkVersion)
        {
            string path = Utilities.Installation.GetNetFrameworkDirectory(frameworkVersion) + Path.GetFileName(t.Assembly.Location);
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

        public static void EncryptDataProviderKeys(string configFilePath, string site, string app, ConfigSectionGroupEncryptionOptions[] configSectionGroupOptions, Version frameworkVersion)
        {
            foreach (var sectionGroup in configSectionGroupOptions)
            {
                // Copy needed assemblies
                foreach (var section in sectionGroup.ConfigSectionEncryptionOptions)
                {
                    CopyTypeAssemblyFileToNetFramework(section.SectionType, frameworkVersion);
                }

                // Remove nodes
                foreach (var section in sectionGroup.ConfigSectionEncryptionOptions)
                {
                    section.SectionNode = Utilities.Installation.RemoveSectionNode(configFilePath, section.SectionName);
                }

                // Create section with custom method
                foreach (var section in sectionGroup.ConfigSectionEncryptionOptions)
                {
                    if (section.CustomMethod != null)
                    {
                        var config = Utilities.Installation.OpenConfigFile(configFilePath);
                        section.CustomMethod(config);
                    }
                }

                // Encrypt sections
                foreach (var section in sectionGroup.ConfigSectionEncryptionOptions)
                {
                    if (section.CustomMethod != null)
                    {
                        RunRegIIS(string.Format("-pe \"{0}/{1}\" -site \"{2}\" -app \"{3}\"", sectionGroup.ConfigSectionGroupName, section.SectionName, site, app),frameworkVersion);
                    }
                }

                // Restore sections and delete files
                foreach (var section in sectionGroup.ConfigSectionEncryptionOptions)
                {
                    if (section.CustomMethod == null && section.SectionNode != null)
                    {
                        Utilities.Installation.AddSectionNode(section.SectionNode, configFilePath, sectionGroup.ConfigSectionGroupName);
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic["name"] = DataProvidersConfigurationSection.SectionName;
                        dic["type"] = typeof(DataProvidersConfigurationSection).AssemblyQualifiedName;

                        Utilities.Installation.AddSectionNode("section", dic, configFilePath, string.Format("sectionGroup[@name='{0}']", sectionGroup.ConfigSectionGroupName));
                    }
                    DeleteTypeAssemblyFileFromNetFramework(section.SectionType,frameworkVersion);
                }
            }
        }

        private static void SetConnectionStrings(string configFilePath, Dictionary<string, string> connectionStrings)
        {
            foreach (KeyValuePair<string, string> connectionString in connectionStrings)
            {
                Utilities.Installation.SetConnectionStringInConfigFile(configFilePath, connectionString.Key, connectionString.Value);
            }
        }

        private static void EncryptConnectionStrings(string site, string app, Version frameworkVersion)
        {
            RunRegIIS(string.Format("-pe \"connectionStrings\" -site \"{0}\" -app \"{1}\"", site, app),frameworkVersion);
        }

        private static void InitializeFlatFileLogging(string configFilePath)
        {
            Utilities.Installation.SetFlatFileLogListenerAccessRights(configFilePath);
        }
    }
}
