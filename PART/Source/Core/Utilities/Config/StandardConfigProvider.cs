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
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

namespace CprBroker.Utilities.Config
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
                    _CurrentConfiguration = Utilities.Config.ConfigUtils.GetConfigFile();
                }
                return _CurrentConfiguration;
            }
        }

        public CprBroker.Config.Properties.Settings Settings
        {
            get
            {
                return CprBroker.Config.Properties.Settings.Default;
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
                return CprBroker.Utilities.Config.DataProviderKeysSection.GetFromConfig(Utilities.Config.ConfigUtils.GetConfigFile());
            }
        }

        private TasksConfigurationSection _TasksSection;
        public TasksConfigurationSection TasksSection
        {
            get
            {
                if (_TasksSection == null)
                {
                    _TasksSection = CurrentConfiguration.Sections[TasksConfigurationSection.SectionName] as TasksConfigurationSection;
                    if (_TasksSection == null)
                    {
                        _TasksSection = new TasksConfigurationSection();
                        _DataProvidersConfigurationSectionGroup.Sections.Add(DataProvidersConfigurationSection.SectionName, _TasksSection);
                        _CurrentConfiguration.Save();
                    }
                }
                return _TasksSection;
            }
        }

        public LoggingSettings LoggingSettings
        {
            get 
            {
                var ret= CurrentConfiguration.GetSection("loggingConfiguration") as LoggingSettings;
                if (ret == null)
                {
                    ret = new LoggingSettings();
                    CurrentConfiguration.Sections.Add("loggingConfiguration", ret);
                    Commit();
                }
                return ret;
            }
        }

        public void Commit()
        {
            CurrentConfiguration.Save();
        }

    }
}
