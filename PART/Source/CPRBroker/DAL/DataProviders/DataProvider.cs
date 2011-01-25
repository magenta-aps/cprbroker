using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;

namespace CprBroker.DAL.DataProviders
{
    public partial class DataProvider
    {
        public static void SetChildLoadOptions(DataProvidersDataContext dataContext)
        {
            System.Data.Linq.DataLoadOptions loadOptions = new System.Data.Linq.DataLoadOptions();
            loadOptions.LoadWith<DataProvider>(dp => dp.DataProviderProperties);
            dataContext.LoadOptions = loadOptions;
        }

        private DataProviderProperty GetDataProviderProperty(string key)
        {
            return (from p in this.DataProviderProperties where p.Name == key select p).FirstOrDefault();
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
                    this.DataProviderProperties.Add(new DataProviderProperty()
                        {
                            DataProviderPropertyId = Guid.NewGuid(),
                            DataProvider = this,
                            Name = key,
                            Value = value,
                            Ordinal = DataProviderProperties.Count
                        });
                }
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
                Attributes = Array.ConvertAll<DataProviderProperty, AttributeType>(
                    DataProviderProperties.OrderBy(p => p.Ordinal).ToArray(),
                    p => new AttributeType()
                    {
                        Name = p.Name,
                        Value = p.Value
                    }
                )
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
