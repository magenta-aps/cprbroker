using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Utilities;

namespace CprBroker.DAL.DataProviders
{
    public partial class DataProvider
    {
        public static void SetChildLoadOptions(DataProvidersDataContext dataContext)
        {
            System.Data.Linq.DataLoadOptions loadOptions = new System.Data.Linq.DataLoadOptions();
            dataContext.LoadOptions = loadOptions;
        }

        partial void OnLoaded()
        {
            if (Data != null)
            {
                Properties.AddRange(Encryption.DecryptObject<AttributeType[]>(Data.ToArray()));
            }
        }
        private AttributeType GetDataProviderProperty(string key)
        {
            return (from p in this.Properties where p.Name == key select p).FirstOrDefault();
        }

        private readonly List<AttributeType> Properties = new List<AttributeType>();
        public AttributeType[] GetProperties()
        {
            return Properties.Select(p => new AttributeType() { Name = p.Name, Value = p.Value }).ToArray();
        }

        public string this[string key]
        {
            get
            {
                var prop = GetDataProviderProperty(key);
                if (key != null)
                {
                    return prop.Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                var prop = GetDataProviderProperty(key);
                if (prop != null)
                {
                    prop.Value = value;
                }
                else
                {
                    this.Properties.Add(new AttributeType()
                        {
                            Name = key,
                            Value = value,
                        });
                }
                Data = Encryption.EncryptObject(Properties.ToArray());
            }
        }

        public Dictionary<string, string> ToPropertiesDictionary(string[] keys)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            foreach (var k in keys)
            {
                ret[k] = this[k];
            }
            return ret;
        }

        public DataProviderType ToXmlType()
        {
            return new DataProviderType()
            {
                TypeName = TypeName,
                Enabled = IsEnabled,
                Attributes = GetProperties()
            };
        }

        public static DataProvider FromXmlType(DataProviderType oio, int ordinal, string[] keysInOrder)
        {
            DataProvider dbProv = new DataProvider()
            {
                DataProviderId = Guid.NewGuid(),
                TypeName = oio.TypeName,
                IsEnabled = oio.Enabled,
                IsExternal = true,
                Ordinal = ordinal,
                Data = null,
            };

            for (int iProp = 0; iProp < keysInOrder.Length; iProp++)
            {
                var propName = keysInOrder[iProp];
                var oioProp = oio.Attributes.FirstOrDefault(p => p.Name == propName);
                dbProv[propName] = oioProp.Value;
            }
            return dbProv;
        }
    }
}
