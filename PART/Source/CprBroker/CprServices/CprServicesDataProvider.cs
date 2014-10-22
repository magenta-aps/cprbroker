using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.PartInterface;
using CprBroker.Engine;
using CprBroker.Engine.Part;

namespace CprBroker.Providers.CprServices
{
    public partial class CprServicesDataProvider : IPartSearchDataProvider, IExternalDataProvider, IPerCallDataProvider
    {
        public CprServicesDataProvider()
        {
            this.ConfigurationProperties = new Dictionary<string, string>();
        }

        public string[] OperationKeys
        {
            get
            {
                return new string[] { 
                Constants.OperationKeys.signon,
                Constants.OperationKeys.newpass,
                Constants.OperationKeys.ADRSOG1,
            };
            }
        }

        public DataProviderConfigPropertyInfo[] ConfigurationKeys
        {
            get
            {
                return new DataProviderConfigPropertyInfo[] { 
                    new DataProviderConfigPropertyInfo(){ Name=Constants.ConfigKeys.Address, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.ConfigKeys.UserId, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = false, Required=true},
                    new DataProviderConfigPropertyInfo(){ Name=Constants.ConfigKeys.Password, Type = DataProviderConfigPropertyInfoTypes.String, Confidential = true, Required=true},                    
                };
            }
        }

        public string Address
        {
            get { return this.ConfigurationProperties[Constants.ConfigKeys.Address]; }
            set { this.ConfigurationProperties[Constants.ConfigKeys.Address] = value; }
        }

        public string UserId
        {
            get { return this.ConfigurationProperties[Constants.ConfigKeys.UserId]; }
            set { this.ConfigurationProperties[Constants.ConfigKeys.UserId] = value; }
        }

        public string Password
        {
            get { return this.ConfigurationProperties[Constants.ConfigKeys.Password]; }
            set { this.ConfigurationProperties[Constants.ConfigKeys.Password] = value; }
        }

        public Version Version
        {
            get { return new Version(CprBroker.Utilities.Constants.Versioning.Major, CprBroker.Utilities.Constants.Versioning.Minor); }
        }

        public Dictionary<string, string> ConfigurationProperties { get; set; }

        public bool IsAlive()
        {
            // TODO: Call signon here
            throw new NotImplementedException();
        }

        public Guid[] Search(CprBroker.Schemas.Part.SoegInputType1 searchCriteria)
        {
            // TODO: Implement search here
            throw new NotImplementedException();
        }


    }
}
