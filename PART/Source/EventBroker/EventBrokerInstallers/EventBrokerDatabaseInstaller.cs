using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.EventBroker.Data;
using CprBroker.Utilities;

namespace CprBroker.Installers.EventBrokerInstallers
{
    /// <summary>
    /// Installs Event Broker database
    /// </summary>
    public class EventBrokerDatabaseInstaller : DatabaseInstaller
    {
        protected override string CreateDatabaseObjectsSql
        {
            get
            {
                return Properties.Resources.CreateEventBrokerDatabaseObjects;
            }
        }

        protected override string SuggestedDatabaseName
        {
            get
            {
                return "EventBroker";
            }
        }

        protected override KeyValuePair<string, string>[] GetLookupData()
        {
            List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();

            ret.Add(new KeyValuePair<string, string>(typeof(ChannelType).Name, Properties.Resources.ChannelType));
            ret.Add(new KeyValuePair<string, string>(typeof(SubscriptionType).Name, Properties.Resources.SubscriptionType));

            return ret.ToArray();
        }

        protected override Dictionary<string, Dictionary<string, string>> GetConnectionStringsToConfigure(System.Collections.IDictionary savedState)
        {
            var ret = new Dictionary<string, Dictionary<string, string>>();
            SavedStateWrapper savedStateWrapper = new SavedStateWrapper(savedState);
            string connectionStringKey = typeof(Config.Properties.Settings).FullName + ".EventBrokerConnectionString";

            var webConfigPath = Installation.GetWebConfigFilePathFromInstaller(this);
            ret[webConfigPath] = new Dictionary<string, string>();
            ret[webConfigPath][connectionStringKey] = savedStateWrapper.GetDatabaseSetupInfo().CreateConnectionString(false, true);

            var backEndConfigFileName = typeof(CprBroker.EventBroker.Backend.BackendService).Assembly.Location + ".config";
            ret[backEndConfigFileName] = new Dictionary<string, string>();
            ret[backEndConfigFileName][connectionStringKey] = savedStateWrapper.GetDatabaseSetupInfo().CreateConnectionString(false, true);

            return ret;
        }
    }
}
