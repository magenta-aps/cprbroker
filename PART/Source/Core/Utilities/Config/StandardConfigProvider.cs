using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;

namespace CprBroker.Config
{
    public class StandardConfigProvider : IConfigProvider
    {
        public Properties.Settings Settings
        {
            get { return Properties.Settings.Default; }
        }

        public System.Security.Cryptography.RijndaelManaged EncryptionAlgorithm
        {
            get
            {
                return CprBroker.Utilities.DataProviderKeysSection.GetFromConfig(Utilities.Config.GetConfigFile());
            }
        }

        public DataProvidersConfigurationSection DataProvidersSection
        {
            get
            {
                var configFile = Utilities.Config.GetConfigFile();
                var group = configFile.SectionGroups[Utilities.Constants.DataProvidersSectionGroupName];
                if (group != null)
                {
                    return group.Sections[DataProvidersConfigurationSection.SectionName] as DataProvidersConfigurationSection;
                }
                return null;
            }
        }
    }
}
