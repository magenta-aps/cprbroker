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

        protected override LookupInsertionParameters[] GetLookupInsertionParameters()
        {
            List<LookupInsertionParameters> ret = new List<LookupInsertionParameters>();

            ret.Add(new LookupInsertionParameters(this, typeof(CprBroker.Data.Part.AddressCoordinateQualityType), Properties.Resources.AddressCoordinateQualityType));
            ret.Add(new LookupInsertionParameters(this, typeof(CprBroker.Data.Applications.Application), Properties.Resources.Application,
                new LookupInsertionParameters.ColumnType() { Name = "ApplicationId", Type = typeof(Guid) }
                ));
            ret.Add(new LookupInsertionParameters(this, typeof(CivilStatusCodeType), Properties.Resources.CivilStatusCodeType));
            ret.Add(new LookupInsertionParameters(this, typeof(ContactChannelType), Properties.Resources.ContactChannelType));
            ret.Add(new LookupInsertionParameters(this, typeof(CountrySchemeType), Properties.Resources.CountrySchemeType));
            ret.Add(new LookupInsertionParameters(this, typeof(Gender), Properties.Resources.Gender));
            ret.Add(new LookupInsertionParameters(this, typeof(LifecycleStatus), Properties.Resources.LifecycleStatus));
            ret.Add(new LookupInsertionParameters(this, typeof(LifeStatusCodeType), Properties.Resources.LifeStatusCodeType));
            ret.Add(new LookupInsertionParameters(this, typeof(LogType), Properties.Resources.LogType));
            ret.Add(new LookupInsertionParameters(this, typeof(RelationshipType), Properties.Resources.RelationshipType));

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
