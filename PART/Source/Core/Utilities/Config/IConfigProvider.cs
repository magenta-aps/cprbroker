using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Utilities.Config
{
    public interface IConfigProvider
    {
        CprBroker.Config.Properties.Settings Settings { get; }
        System.Security.Cryptography.RijndaelManaged EncryptionAlgorithm { get; }
        DataProvidersConfigurationSection DataProvidersSection { get; }
        TasksConfigurationSection TasksSection { get; }
        Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.LoggingSettings LoggingSettings { get; }
        void Commit();
    }
}
