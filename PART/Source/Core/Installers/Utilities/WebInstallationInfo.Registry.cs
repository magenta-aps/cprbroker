using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace CprBroker.Installers
{
    public partial class WebInstallationInfo
    {
        public static Dictionary<string, string> PropertyToRegistryMappings
        {
            get
            {
                var ret = new Dictionary<string, string>();
                ret.Add(Constants.CreateWebsite, "CreateAsWebsite");
                ret.Add(Constants.WebsiteName, "SiteName");
                ret.Add(Constants.VirtualDirectoryName, "VirtualDirectoryName");
                ret.Add(Constants.InstallDir, "InstallDir");
                return ret;
            }
        }

        public const string RegistrySubRoot = "Website";

        public static void CopyPropertiesToRegistry(Session session, string featureName)
        {
            BaseSetupInfo.CopyPropertiesToRegistry(session, RegistrySubRoot, PropertyToRegistryMappings, featureName);
        }

        public static void CopyRegistryToProperties(Session session, string featureName)
        {
            BaseSetupInfo.CopyRegistryToProperties(session, RegistrySubRoot, PropertyToRegistryMappings, featureName);
        }

    }
}
