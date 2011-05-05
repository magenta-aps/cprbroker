using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration.Install;
using System.Xml;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Security;
using System.Security.AccessControl;

namespace CprBroker.Utilities
{
    /// <summary>
    /// Utility class with methods that assist the installation process by extending the Installer class
    /// </summary>
    public static class Installation
    {
        /// <summary>
        /// Returns the full path of the currently executing exe installer
        /// </summary>
        /// <param name="installer"></param>
        /// <returns></returns>
        public static string GetInstallerAssemblyFilePath(this Installer installer)
        {
            return installer.Context.Parameters["assemblypath"]; ;
        }

        /// <summary>
        /// Returns the full path of the config file related to the current exe
        /// </summary>
        /// <param name="installer"></param>
        /// <returns></returns>
        public static string GetInstallerAssemblyConfigFilePath(this Installer installer)
        {
            return installer.GetInstallerAssemblyFilePath() + ".config";
        }

        /// <summary>
        /// Returns the full path of the folder containing the current exe
        /// </summary>
        /// <param name="installer"></param>
        /// <returns></returns>
        public static string GetInstallerAssemblyFolderPath(this Installer installer)
        {
            string exePath = installer.GetInstallerAssemblyFilePath();
            FileInfo fileInfo = new FileInfo(exePath);
            return fileInfo.Directory.FullName;
        }

        public static string GetWebFolderPath(this Installer installer)
        {
            var installerAssemblyDir = new DirectoryInfo(installer.GetInstallerAssemblyFolderPath());
            return installerAssemblyDir.Parent.FullName;
        }
        /// <summary>
        /// Gets the full path of the web config of the file that contains the current exe installer
        /// </summary>
        /// <param name="installer"></param>
        /// <returns></returns>
        public static string GetWebConfigFilePathFromInstaller(this Installer installer)
        {
            var webDir = new DirectoryInfo(installer.GetWebFolderPath());
            string configFileName = webDir + "\\web.config";
            return configFileName;
        }

        public static Configuration OpenConfigFile(string configFilePath)
        {
            var map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = configFilePath;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            return configuration;
        }

        /// <summary>
        /// Sets the connection string value in the given config file
        /// </summary>
        /// <param name="installer"></param>
        /// <param name="configFilePath"></param>
        /// <param name="connectionString"></param>
        public static void SetConnectionStringInConfigFile(string configFilePath, string name, string connectionString)
        {
            var configuration = OpenConfigFile(configFilePath);
            System.Configuration.ConnectionStringsSection sec = configuration.GetSection("connectionStrings") as ConnectionStringsSection;
            if (sec.ConnectionStrings[name] == null)
            {
                sec.ConnectionStrings.Add(new ConnectionStringSettings(name, connectionString));
            }
            else
            {
                sec.ConnectionStrings[name].ConnectionString = connectionString;
            }
            configuration.Save();
        }

        public static XmlNode RemoveSectionNode(string configFileName, string nodeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFileName);
            XmlNode node = doc.SelectSingleNode("//" + nodeName);
            if (node != null)
            {
                var parentNode = node.ParentNode;
                parentNode.RemoveChild(node);
                doc.Save(configFileName);
            }
            return node;
        }

        public static bool AddSectionNode(string nodeName, Dictionary<string, string> attributes, string configFileName, string parentNodeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFileName);
            XmlNode parentNode = doc.SelectSingleNode("//" + parentNodeName);
            if (parentNode != null)
            {
                var newNode = doc.CreateElement(nodeName);
                foreach (var attr in attributes)
                {
                    var at = doc.CreateAttribute(attr.Key);
                    at.Value = attr.Value;
                    newNode.Attributes.Append(at);
                }
                parentNode.AppendChild(newNode);
                doc.Save(configFileName);
                return true;
            }
            return false;
        }
        public static bool AddSectionNode(XmlNode node, string configFileName, string parentNodeName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFileName);
            XmlNode parentNode = doc.SelectSingleNode("//" + parentNodeName);
            if (parentNode != null)
            {
                var newNode = doc.CreateElement(node.Name);
                newNode.InnerXml = node.InnerXml;
                parentNode.AppendChild(newNode);
                doc.Save(configFileName);
                return true;
            }
            return false;
        }

        public static void SetApplicationSettingInConfigFile(string configFileName, Type settingsType, string settingName, string value)
        {
            var conf = OpenConfigFile(configFileName);
            var applicationSettings = conf.SectionGroups["applicationSettings"] as ApplicationSettingsGroup;
            if (applicationSettings == null)
            {
                applicationSettings = new ApplicationSettingsGroup();
                conf.SectionGroups.Add("applicationSettings", applicationSettings);
            }
            var configSettings = applicationSettings.Sections[settingsType.FullName] as ClientSettingsSection;
            if (configSettings == null)
            {
                configSettings = new ClientSettingsSection();
                applicationSettings.Sections.Add(settingsType.FullName, configSettings);
            }
            var settingElement = configSettings.Settings.Get(settingName);
            if (settingElement == null)
            {
                settingElement = new SettingElement(settingName, SettingsSerializeAs.String);
            }
            settingElement.Value.ValueXml = new XmlDocument().CreateNode(XmlNodeType.Element, "value", "");
            settingElement.Value.ValueXml.InnerText = value;
            configSettings.Settings.Add(settingElement);
            conf.Save(ConfigurationSaveMode.Full);
        }

        private static XmlNode GetConfigNode(string nodePath, ref string configFilePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFilePath);
            XmlNode node = doc.SelectSingleNode(nodePath);

            if (node.Attributes["configSource"] != null)
            {
                string filePath = node.Attributes["configSource"].Value;
                var configDir = Path.GetDirectoryName(configFilePath);
                configFilePath = configDir + "\\" + filePath;

                doc.Load(configFilePath);
                node = doc.SelectSingleNode(nodePath);
            }
            return node;
        }

        public static void SetFlatFileLogListenerAccessRights(string configFileName)
        {
            XmlNode loggingConfigurationNode = GetConfigNode("//loggingConfiguration", ref configFileName);

            var listenersNode = loggingConfigurationNode.SelectSingleNode("listeners");
            var flatFileNode = listenersNode.SelectSingleNode("add[@name='FlatFile']");
            string fileName = flatFileNode.Attributes["fileName"].Value;

            if (!File.Exists(fileName))
            {
                string directoryName = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                File.Create(fileName);
            }
            FileSecurity access = System.IO.File.GetAccessControl(fileName);
            FileSystemAccessRule rule = new FileSystemAccessRule("NETWORK SERVICE", FileSystemRights.FullControl, AccessControlType.Allow);
            access.ResetAccessRule(rule);
            File.SetAccessControl(fileName, access);
        }

        public static void CopyConfigNode(string nodePath, string fromConfigFile, string toConfigFile)
        {
            XmlNode sourceNode = GetConfigNode(nodePath, ref fromConfigFile);
            XmlNode targetNode = GetConfigNode(nodePath, ref toConfigFile);

            targetNode.Attributes.RemoveAll();
            foreach (XmlAttribute sourceAttribute in sourceNode.Attributes)
            {
                var targetAttribute=targetNode.OwnerDocument.CreateAttribute(sourceAttribute.Name);
                targetAttribute.Value=sourceAttribute.Value;
                targetNode.Attributes.Append(targetAttribute);
            }
            targetNode.InnerXml = sourceNode.InnerXml;

            targetNode.OwnerDocument.Save(toConfigFile);
        }

        const int MAX_PATH = 256;
        public static string GetNetFrameworkDirectory()
        {
            StringBuilder buf = new StringBuilder(
                MAX_PATH, MAX_PATH);
            int cch = MAX_PATH;
            int hr = GetCORSystemDirectory(
                buf, MAX_PATH, ref cch);
            if (hr < 0) Marshal.ThrowExceptionForHR(hr);
            return buf.ToString();
        }

        [DllImport("mscoree.dll",
         CharSet = CharSet.Unicode,
         ExactSpelling = true)]
        public static extern int GetCORSystemDirectory(
                StringBuilder buf,
                int cchBuf,
                ref int cchRequired);

        public static WindowHandleWrapper InstallerWindowWrapper(this Installer installer)
        {
            return new WindowHandleWrapper(installer.Context.Parameters["productName"]);
        }

        public static WindowHandleWrapper InstallerWindowWrapper(this Microsoft.Deployment.WindowsInstaller.Session session)
        {
            return new WindowHandleWrapper(session["ProductName"]);
        }
    }
}
