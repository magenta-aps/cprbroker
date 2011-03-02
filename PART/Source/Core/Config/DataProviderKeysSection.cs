using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CprBroker.Config
{
    public class DataProviderKeysSection : ConfigurationSection
    {
        public const string SectionName = "dataProviderKeys";

        [ConfigurationProperty("IV", IsRequired = true)]
        public string IV
        {
            get
            {
                return (string)this["IV"];
            }
            set
            {
                this["IV"] = value;
            }
        }

        [ConfigurationProperty("Key", IsRequired = true)]
        public string Key
        {
            get
            {
                return (string)this["Key"];
            }
            set
            {
                this["Key"] = value;
            }
        }

        public static bool IsNullOrEmpty(DataProviderKeysSection section)
        {
            if (section == null || string.IsNullOrEmpty(section.IV) || string.IsNullOrEmpty(section.Key))
            {
                return false;
            }
            return true;
        }
    }
}
