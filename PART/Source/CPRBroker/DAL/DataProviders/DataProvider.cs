using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
