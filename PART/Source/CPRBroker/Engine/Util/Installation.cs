using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration.Install;
using System.Xml;
using System.Runtime.InteropServices;

namespace CprBroker.Engine.Util
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
        public static string GetAssemblyFilePath(this Installer installer)
        {
            return installer.Context.Parameters["assemblypath"]; ;
        }

        /// <summary>
        /// Returns the full path of the config file related to the current exe
        /// </summary>
        /// <param name="installer"></param>
        /// <returns></returns>
        public static string GetAssemblyConfigFilePath(this Installer installer)
        {
            return installer.GetAssemblyFilePath() + ".config";
        }

        /// <summary>
        /// Returns the full path of the folder containing the current exe
        /// </summary>
        /// <param name="installer"></param>
        /// <returns></returns>
        public static string GetAssemblyFolderPath(this Installer installer)
        {
            string exePath = installer.GetAssemblyFilePath();
            FileInfo fileInfo = new FileInfo(exePath);
            return fileInfo.Directory.FullName;
        }

        public static string GetWebFolderPath(this Installer installer)
        {
            string assemblyFolderPath = installer.GetAssemblyFolderPath();
            return assemblyFolderPath + "\\Web";
        }
        /// <summary>
        /// Gets the full path of the web config of the file that contains the current exe installer
        /// </summary>
        /// <param name="installer"></param>
        /// <returns></returns>
        public static string GetWebConfigFilePath(this Installer installer)
        {
            string configFileName = installer.GetWebFolderPath() + "\\web.config";
            return configFileName;
        }

        /// <summary>
        /// XPath of the connection string node in config files
        /// </summary>
        public static readonly string ConnectionStringNodePath = "//connectionStrings/add[@name='CprBroker.Config.Properties.Settings.CPRConnectionString']";

        /// <summary>
        /// Gets the connection string from the web.config file of the current installer
        /// </summary>
        /// <param name="installer"></param>
        /// <returns></returns>
        public static string GetConnectionStringFromWebConfig(this Installer installer)
        {
            string configFileName = installer.GetWebConfigFilePath();
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(configFileName);
                XmlNode connectionStringNode = doc.SelectSingleNode(ConnectionStringNodePath);
                if (connectionStringNode != null)
                {
                    return connectionStringNode.Attributes["connectionString"].Value;
                }
            }
            catch (Exception)
            { }
            return CprBroker.Config.Properties.Settings.Default.CPRConnectionString;
        }

        /// <summary>
        /// Sets the connection string value in the given config file
        /// </summary>
        /// <param name="installer"></param>
        /// <param name="configFilePath"></param>
        /// <param name="connectionString"></param>
        public static void SetConnectionStringInConfigFile(this Installer installer, string configFilePath, string connectionString)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(configFilePath);
            XmlNode connectionStringNode = doc.SelectSingleNode(Installation.ConnectionStringNodePath);
            connectionStringNode.Attributes["connectionString"].Value = connectionString;
            doc.Save(configFilePath);
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

        public static CprBroker.Engine.UI.WindowHandleWrapper InstallerWindowWrapper(this Installer installer)
        {
            return new CprBroker.Engine.UI.WindowHandleWrapper(installer.Context.Parameters["productName"]);
        }
    }
}
