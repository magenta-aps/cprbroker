using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Installers.CprBrokerInstallers
{
    public class CprBrokerDatabaseInstaller : CprBroker.Installers.DBInstaller
    {
        public CprBrokerDatabaseInstaller()
            : base("CprBroker")
        {

        }

        protected override string CreateDatabaseObjectsSql
        {
            get
            {
                return CprBroker.Installers.CprBrokerInstallers.Properties.Resources.CreateDatabaseObjects;
            }
        }

        protected override LookupInsertionParameters[] GetLookupInsertionParameters()
        {
            return LookupInsertionParameters.InitializeInsertionParameters(this);
        }

        protected override string[] ConfigFileNames
        {
            get
            {
                return new string[] 
                { 
                    //CprBroker.Engine.Util.Installation.GetWebConfigFilePath(this) 
                };
            }
        }
    }
}
