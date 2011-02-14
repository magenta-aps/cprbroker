using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.EventBroker.DAL;

namespace CprBroker.Installers.EventBrokerInstallers
{
    public class EventBrokerDatabaseInstaller : DBInstaller
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

        protected override LookupInsertionParameters[] GetLookupInsertionParameters()
        {
            List<LookupInsertionParameters> ret = new List<LookupInsertionParameters>();
            ret.Add(new LookupInsertionParameters(this, typeof(ChannelType), Properties.Resources.ChannelType));
            ret.Add(new LookupInsertionParameters(this, typeof(SubscriptionType), Properties.Resources.SubscriptionType));

            return ret.ToArray();
        }

        protected override Dictionary<string, Dictionary<string, string>> GetConnectionStringsToConfigure(System.Collections.IDictionary savedState)
        {
            var ret = new Dictionary<string, Dictionary<string, string>>();
            SavedStateWrapper savedStateWrapper = new SavedStateWrapper(savedState);

            var webConfigPath = CprBroker.Engine.Util.Installation.GetWebConfigFilePathFromInstaller(this);
            ret[webConfigPath] = new Dictionary<string, string>();
            ret[webConfigPath]["CprBroker.Config.Properties.Settings.EventBrokerConnectionString"] = savedStateWrapper.GetDatabaseSetupInfo().CreateConnectionString(false, true);

            var backEndConfigFileName = typeof(CprBroker.EventBroker.Backend.BackendService).Assembly.Location + ".config";
            ret[backEndConfigFileName] = new Dictionary<string, string>();
            ret[backEndConfigFileName]["CprBroker.Config.Properties.Settings.CprBrokerConnectionString"] = savedStateWrapper.GetDatabaseSetupInfo().CreateConnectionString(false, true);

            return ret;
        }
    }
}
