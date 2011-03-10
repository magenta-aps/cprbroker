using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Installers;
using CprBroker.Data;
using CprBroker.Data.Applications;
using CprBroker.Data.Part;
using CprBroker.Data.DataProviders;
using CprBroker.Utilities;

namespace CprBroker.Installers.CprBrokerInstallers
{
    /// <summary>
    /// Installs the CPR Broker database
    /// </summary>
    public class CprBrokerDatabaseInstaller : CprBroker.Installers.DatabaseInstaller
    {
        public CprBrokerDatabaseInstaller()
        {

        }

        protected override string SuggestedDatabaseName
        {
            get
            {
                return "CprBroker";
            }
        }
        protected override string CreateDatabaseObjectsSql
        {
            get
            {
                return CprBroker.Installers.CprBrokerInstallers.Properties.Resources.CreatePartDatabaseObjects;
            }
        }

        protected override KeyValuePair<string, string>[] GetLookupData()
        {
            List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();
            ret.Add(new KeyValuePair<string,string>(typeof(CprBroker.Data.Part.AddressCoordinateQualityType).Name, Properties.Resources.AddressCoordinateQualityType));
            ret.Add(new KeyValuePair<string, string>(typeof(CprBroker.Data.Applications.Application).Name, Properties.Resources.Application));
            ret.Add(new KeyValuePair<string, string>(typeof(CivilStatusCodeType).Name, Properties.Resources.CivilStatusCodeType));
            ret.Add(new KeyValuePair<string, string>(typeof(ContactChannelType).Name, Properties.Resources.ContactChannelType));
            ret.Add(new KeyValuePair<string, string>(typeof(CountrySchemeType).Name, Properties.Resources.CountrySchemeType));
            ret.Add(new KeyValuePair<string, string>(typeof(Gender).Name, Properties.Resources.Gender));
            ret.Add(new KeyValuePair<string, string>(typeof(LifecycleStatus).Name, Properties.Resources.LifecycleStatus));
            ret.Add(new KeyValuePair<string, string>(typeof(LifeStatusCodeType).Name, Properties.Resources.LifeStatusCodeType));
            ret.Add(new KeyValuePair<string, string>(typeof(LogType).Name, Properties.Resources.LogType));
            ret.Add(new KeyValuePair<string, string>(typeof(RelationshipType).Name, Properties.Resources.RelationshipType));

            return ret.ToArray();
        }

        protected override Dictionary<string, Dictionary<string, string>> GetConnectionStringsToConfigure(System.Collections.IDictionary savedState)
        {
            var ret = new Dictionary<string, Dictionary<string, string>>();
            SavedStateWrapper savedStateWrapper = new SavedStateWrapper(savedState);
            var webConfigPath = Installation.GetWebConfigFilePathFromInstaller(this);
            ret[webConfigPath] = new Dictionary<string, string>();
            ret[webConfigPath]["CprBroker.Config.Properties.Settings.CprBrokerConnectionString"] = savedStateWrapper.GetDatabaseSetupInfo().CreateConnectionString(false, true);
            return ret;
        }
    }
}
