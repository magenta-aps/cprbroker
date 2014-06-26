using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Engine;

namespace CprBroker.Config
{
    public interface IConfigProvider
    {
        Properties.Settings Settings { get; }
        System.Security.Cryptography.RijndaelManaged EncryptionAlgorithm { get; }
        DataProvidersConfigurationSection DataProvidersSection { get; }
        void Commit();
    }
}
