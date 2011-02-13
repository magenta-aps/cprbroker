using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Installers;
using CprBroker.DAL;
using CprBroker.DAL.Applications;
using CprBroker.DAL.Part;
using CprBroker.DAL.DataProviders;


namespace CprBroker.Installers.CprBrokerInstallers
{
    [System.ComponentModel.RunInstaller(true)]
    public class CprBrokerDatabaseInstaller : CprBroker.Installers.DBInstaller
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

            ret.Add(new LookupInsertionParameters(this, typeof(CprBroker.DAL.Part.AddressCoordinateQualityType), Properties.Resources.AddressCoordinateQualityType));
            ret.Add(new LookupInsertionParameters(this, typeof(CprBroker.DAL.Applications.Application), Properties.Resources.Application,
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

        protected override string[] ConfigFileNames
        {
            get
            {
                return new string[] 
                { 
                    //CprBroker.Engine.Util.Installation.GetWebConfigFilePathFromInstaller(this) 
                };
            }
        }
    }
}
