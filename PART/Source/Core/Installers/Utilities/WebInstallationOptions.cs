using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Installers
{
    public class WebInstallationOptions
    {
        public bool EncryptConnectionStrings;
        public Dictionary<string, string> ConnectionStrings;
        public bool InitializeFlatFileLogging;
        public ConfigSectionGroupEncryptionOptions[] ConfigSectionGroupEncryptionOptions;
        public Version FrameworkVersion = new Version(System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion().Replace("v", ""));
        public string WebsiteDirectoryRelativePath = "Website\\";
    }

    public class ConfigSectionGroupEncryptionOptions
    {
        public string ConfigSectionGroupName;
        public ConfigSectionEncryptionOptions[] ConfigSectionEncryptionOptions;
    }

    public class ConfigSectionEncryptionOptions
    {
        public string SectionName;
        public Type SectionType;
        internal System.Xml.XmlNode SectionNode;
        public Action<System.Configuration.Configuration> CustomMethod;
    }
}
