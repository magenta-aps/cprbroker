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

namespace CprBroker.Installers.Installers
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

        private static void RunRegIIS(string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Utilities.Installation.GetNetFrameworkDirectory() + "aspnet_regiis.exe";
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

        private static void GrantConfigEncryptionAccess()
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

        private static void CopyTypeAssemblyFileToNetFramework(Type t)
        {
            string path = Utilities.Installation.GetNetFrameworkDirectory();
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

        private static void DeleteTypeAssemblyFileFroNetFramework(Type t)
        {
            string path = Utilities.Installation.GetNetFrameworkDirectory() + Path.GetFileName(t.Assembly.Location);
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

        public static void EncryptDataProviderKeys(string configFilePath, string site, string app)
        {
            CopyTypeAssemblyFileToNetFramework(typeof(DataProviderKeysSection));
            CopyTypeAssemblyFileToNetFramework(typeof(DataProvidersConfigurationSection));

            Utilities.Installation.RemoveSectionNode(configFilePath, DataProviderKeysSection.SectionName);
            var dataProvidersNode = Utilities.Installation.RemoveSectionNode(configFilePath, DataProvidersConfigurationSection.SectionName);

            var config = Utilities.Installation.OpenConfigFile(configFilePath);
            DataProviderKeysSection.RegisterInConfig(config);

            RunRegIIS(string.Format("-pe \"{0}/{1}\" -site \"{2}\" -app \"{3}\"", Constants.DataProvidersSectionGroupName, DataProviderKeysSection.SectionName, site, app));

            if (dataProvidersNode != null)
            {
                Utilities.Installation.AddSectionNode(dataProvidersNode, configFilePath, Constants.DataProvidersSectionGroupName);
            }

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic["name"] = DataProvidersConfigurationSection.SectionName;
            dic["type"] = typeof(DataProvidersConfigurationSection).AssemblyQualifiedName;

            Utilities.Installation.AddSectionNode("section", dic, configFilePath, string.Format("sectionGroup[@name='{0}']", Constants.DataProvidersSectionGroupName));

            DeleteTypeAssemblyFileFroNetFramework(typeof(DataProviderKeysSection));
            DeleteTypeAssemblyFileFroNetFramework(typeof(DataProvidersConfigurationSection));
        }

        private static void EncryptConnectionStrings(string site, string app)
        {
            RunRegIIS(string.Format("-pe \"connectionStrings\" -site \"{0}\" -app \"{1}\"", site, app));
        }

        private static void InitializeFlatFileLogging(string configFilePath)
        {
            Utilities.Installation.SetFlatFileLogListenerAccessRights(configFilePath);
        }
    }
}
