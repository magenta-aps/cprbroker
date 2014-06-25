using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CprBroker.Config
{
    class CustomConfigProvider : IConfigProvider
    {
        public Properties.Settings Settings
        {
            get 
            {
                var ret = new Properties.Settings();
                return ret;
            }
        }

        public System.Security.Cryptography.RijndaelManaged EncryptionAlgorithm
        {
            get { throw new NotImplementedException(); }
        }

        public Engine.DataProvidersConfigurationSection DataProvidersSection
        {
            get { throw new NotImplementedException(); }
        }
    }
}
