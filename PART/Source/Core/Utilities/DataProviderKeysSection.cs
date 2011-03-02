using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using System.Web;

namespace CprBroker.Utilities
{
    public class DataProviderKeysSection : ConfigurationSection
    {
        public const string SectionName = "dataProviderKeys";

        [ConfigurationProperty("IV", IsRequired = true)]
        private string IVString
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
        private string KeyString
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

        public static bool IsValid(DataProviderKeysSection section)
        {
            if (section == null || string.IsNullOrEmpty(section.IVString) || string.IsNullOrEmpty(section.KeyString))
            {
                return false;
            }
            RijndaelManaged rm = new RijndaelManaged();
            rm.GenerateIV();
            if (rm.IV.Length != section.IV.Length)
            {
                return false;
            }
            rm.GenerateKey();
            if (rm.Key.Length != section.Key.Length)
            {
                return false;
            }
            return true;
        }

        public byte[] IV
        {
            get
            {
                return Strings.Deserialize<byte[]>(IVString);
            }
        }

        public byte[] Key
        {
            get
            {
                return Strings.Deserialize<byte[]>(KeyString);
            }
        }

        
        public static RijndaelManaged GetFromConfig()
        {
            RijndaelManaged rm = new RijndaelManaged();                
            Configuration configFile = Config.GetConfigFile();
            DataProviderKeysSection section = configFile.Sections[SectionName] as DataProviderKeysSection;
            if (!IsValid(section))
            {
                if (section == null)
                {
                    section = new DataProviderKeysSection();
                    configFile.Sections.Add(DataProviderKeysSection.SectionName, section);
                }
                rm.GenerateIV();
                rm.GenerateKey();
                section.IVString = Strings.SerializeObject(rm.IV);
                section.KeyString = Strings.SerializeObject(rm.Key);

                if (!section.SectionInformation.IsProtected)
                {
                    section.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                }

                configFile.Save();
                ConfigurationManager.RefreshSection(SectionName);
            }
            rm.IV = section.IV;
            rm.Key = section.Key;
            return rm;
        }

        

    }
}
