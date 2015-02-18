using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using CprBroker.Schemas;
using CprBroker.Utilities;

namespace CprBroker.Data
{
    /// <summary>
    /// Represents a database object that has a binary field containing encrypted data,
    /// in the form of AttributeType[]
    /// </summary>
    public interface IHasEncryptedAttributes
    {
        RijndaelManaged EncryptionAlgorithm { get; set; }
        System.Data.Linq.Binary EncryptedData { get; set; }
        List<AttributeType> Attributes { get; set; }
    }

    public static class IHasEncryptedPropertiesExtensions
    {
        public static void PreLoadAttributes(this IHasEncryptedAttributes obj)
        {
            if (obj.EncryptedData != null)
            {
                AttributeType[] attributes;
                if (obj.EncryptionAlgorithm == null)
                {
                    attributes = Encryption.DecryptObject<AttributeType[]>(obj.EncryptedData.ToArray());
                }
                else
                {
                    attributes = Encryption.DecryptObject<AttributeType[]>(obj.EncryptedData.ToArray(), obj.EncryptionAlgorithm);
                }

                if (obj.Attributes == null)
                    obj.Attributes = new List<AttributeType>();

                obj.Attributes.AddRange(attributes);
            }
        }

        public static AttributeType GetAttribute(this IHasEncryptedAttributes obj, string key)
        {
            if (obj.Attributes == null)
            {
                obj.Attributes = new List<AttributeType>();
            }
            return (from p in obj.Attributes where p.Name == key select p).FirstOrDefault();
        }

        public static AttributeType[] GetAttributes(this IHasEncryptedAttributes obj)
        {
            return obj.Attributes.Select(p => new AttributeType() { Name = p.Name, Value = p.Value }).ToArray();
        }

        public static string Get(this IHasEncryptedAttributes obj, string key)
        {
            var prop = obj.GetAttribute(key);
            if (prop != null)
            {
                return prop.Value;
            }
            else
            {
                return null;
            }
        }

        public static void Set(this IHasEncryptedAttributes obj, string key, string value)
        {
            var prop = obj.GetAttribute(key);
            if (prop != null)
            {
                prop.Value = value;
            }
            else
            {
                obj.Attributes.Add(new AttributeType()
                {
                    Name = key,
                    Value = value,
                });
            }
            obj.EncryptedData = Encryption.EncryptObject(obj.Attributes.ToArray());
        }

        public static Dictionary<string, string> ToPropertiesDictionary(this IHasEncryptedAttributes obj)
        {
            return obj.GetAttributes().ToDictionary(a => a.Name, a => a.Value);
        }

        public static Dictionary<string, string> ToPropertiesDictionary(this IHasEncryptedAttributes obj, string[] keys)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach (var k in keys)
            {
                ret[k] = obj.Get(k);
            }
            return ret;
        }

        public static void SetAll(this IHasEncryptedAttributes obj, Dictionary<string, string> props)
        {
            foreach (var kvp in props)
            {
                obj.Set(kvp.Key, kvp.Value);
            }
        }
    }
}
