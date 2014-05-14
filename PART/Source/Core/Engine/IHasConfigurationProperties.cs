using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Data;

namespace CprBroker.Engine
{
    /// <summary>
    /// Represents an actual object(not database object) that can define its own set of properties
    /// Atcs as the schema for the properties and could use other objects to get the actual values from storage layer
    /// </summary>
    public interface IHasConfigurationProperties
    {
        Dictionary<string, string> ConfigurationProperties { get; set; }
        DataProviderConfigPropertyInfo[] ConfigurationKeys { get; }
    }

    public static class IHasConfigurationPropertiesExtensions
    {
        public static void FillFromEncryptedStorage(this IHasConfigurationProperties obj, IHasEncryptedAttributes encryptedPropsObj)
        {
            if (obj.ConfigurationProperties == null)
                obj.ConfigurationProperties = new Dictionary<string, string>();

            var keys = obj.ConfigurationKeys.Select(k => k.Name).ToArray();
            foreach (var o in encryptedPropsObj.ToPropertiesDictionary(keys))
            {
                obj.ConfigurationProperties[o.Key] = o.Value;
            }
        }

        public static void CopyToEncryptedStorage(this IHasConfigurationProperties obj, IHasEncryptedAttributes encryptedPropsObj)
        {
            encryptedPropsObj.SetAll(obj.ConfigurationProperties);
        }

        public static DataProviderConfigPropertyInfo[] ToAllPropertyInfo(this IHasConfigurationProperties prov)
        {
            var configKeys = prov.ConfigurationKeys;

            if (prov is IPerCallDataProvider)
            {
                configKeys = configKeys.Union((prov as IPerCallDataProvider).ToOperationConfigPropertyInfo()).ToArray();
            }

            return configKeys;
        }

        public static DataProviderConfigProperty[] ToDisplayableProperties(this IHasConfigurationProperties prov)
        {
            return (from pInfo in prov.ConfigurationKeys
                    join pp in prov.ConfigurationProperties
                    on pInfo.Name equals pp.Key into joined
                    from pVal in joined.DefaultIfEmpty()
                    select new DataProviderConfigProperty()
                    {
                        Name = pInfo.Name,
                        Value = (string.IsNullOrEmpty(pVal.Key) || pInfo.Confidential) ? null : pVal.Value,
                        Confidential = pInfo.Confidential,
                        Required = pInfo.Required,
                        Type = pInfo.Type
                    })
                    .ToArray();
        }
    }
}
