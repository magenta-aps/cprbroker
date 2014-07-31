using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;
using System.Configuration;

namespace CprBroker.Config
{
    public class StandardConfigProvider : IConfigProvider
    {
        private Configuration _CurrentConfiguration;
        public Configuration CurrentConfiguration
        {
            get
            {
                if (_CurrentConfiguration == null)
                {
                    _CurrentConfiguration = Utilities.Config.GetConfigFile();
                }
                return _CurrentConfiguration;
            }
        }

        public Properties.Settings Settings
        {
            get
            {
                return Properties.Settings.Default;
            }
        }

        private ConfigurationSectionGroup _DataProvidersConfigurationSectionGroup;
        public ConfigurationSectionGroup DataProvidersConfigurationSectionGroup
        {
            get
            {
                if (_DataProvidersConfigurationSectionGroup == null)
                {
                    _DataProvidersConfigurationSectionGroup = CurrentConfiguration.SectionGroups[Utilities.Constants.DataProvidersSectionGroupName];
                    if (_DataProvidersConfigurationSectionGroup == null)
                    {
                        _DataProvidersConfigurationSectionGroup = new System.Configuration.ConfigurationSectionGroup();
                        CurrentConfiguration.SectionGroups.Add(Utilities.Constants.DataProvidersSectionGroupName, _DataProvidersConfigurationSectionGroup);
                        _CurrentConfiguration.Save();
                    }
                }
                return _DataProvidersConfigurationSectionGroup;
            }
        }

        private DataProvidersConfigurationSection _DataProvidersSection;
        public DataProvidersConfigurationSection DataProvidersSection
        {
            get
            {
                if (_DataProvidersSection == null)
                {
                    _DataProvidersSection = DataProvidersConfigurationSectionGroup.Sections[DataProvidersConfigurationSection.SectionName] as DataProvidersConfigurationSection;
                    if (_DataProvidersSection == null)
                    {
                        _DataProvidersSection = new DataProvidersConfigurationSection();
                        _DataProvidersConfigurationSectionGroup.Sections.Add(DataProvidersConfigurationSection.SectionName, _DataProvidersSection);
                        _CurrentConfiguration.Save();
                    }
                }
                return _DataProvidersSection;
            }
        }

        public System.Security.Cryptography.RijndaelManaged EncryptionAlgorithm
        {
            get
            {
                // TODO: Check if the logic in the called method should be moved here
                return CprBroker.Utilities.DataProviderKeysSection.GetFromConfig(Utilities.Config.GetConfigFile());
            }
        }


        public void Commit()
        {
            CurrentConfiguration.Save();
        }
    }
}
