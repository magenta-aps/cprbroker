using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using CprBroker.Config;
using System.Web;

namespace CprBroker.Utilities
{
    /// <summary>
    /// Utility methods for encryption
    /// </summary>
    public static class Encryption
    {
        public static T[] AsArray<T>(object o) where T : class
        {
            if (o == null)
            {
                return new T[0];
            }
            else
            {
                return new T[] { o as T };
            }
        }


        public static byte[] EncryptObject(object o)
        {
            InitializeKeysConfiguration();
            var ret = new List<byte>();
            var xml = Strings.SerializeObject(o);

            RijndaelManaged m = LoadKeys();

            var transform = m.CreateEncryptor();

            MemoryStream ms = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
            {
                using (StreamWriter w = new StreamWriter(cs))
                {
                    w.Write(xml);
                }
            }

            var encryptedData = ms.ToArray();
            return encryptedData;
        }

        public static T DecryptObject<T>(byte[] encryptedData)
        {
            InitializeKeysConfiguration();
            string xml = null;
            RijndaelManaged m = LoadKeys();
            var transform = m.CreateDecryptor();

            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, transform, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        xml = srDecrypt.ReadToEnd();
                    }
                }
            }
            var ret = Strings.Deserialize<T>(xml);
            return ret;
        }

        private static RijndaelManaged LoadKeys()
        {
            Configuration configFile = GetConfigFile();
            DataProviderKeysSection section = configFile.Sections[DataProviderKeysSection.SectionName] as DataProviderKeysSection;
            RijndaelManaged rm = new RijndaelManaged();
            rm.IV = Strings.Deserialize<byte[]>(section.IV);
            rm.Key = Strings.Deserialize<byte[]>(section.Key);
            return rm;
        }

        private static void InitializeKeysConfiguration()
        {
            Configuration configFile = GetConfigFile();
            DataProviderKeysSection section = configFile.Sections[DataProviderKeysSection.SectionName] as DataProviderKeysSection;
            if (!DataProviderKeysSection.IsNullOrEmpty(section))
            {
                InitializeKeysConfiguration(configFile, true);
                section = configFile.Sections[DataProviderKeysSection.SectionName] as DataProviderKeysSection;
                if (!section.SectionInformation.IsProtected)
                {
                    section.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                    configFile.Save();
                }
                ConfigurationManager.RefreshSection(DataProviderKeysSection.SectionName);
            }
        }

        public static void InitializeKeysConfiguration(Configuration configFile, bool overwrite)
        {
            DataProviderKeysSection section = configFile.Sections[DataProviderKeysSection.SectionName] as DataProviderKeysSection;
            if (section == null)
            {
                section = new DataProviderKeysSection();
                configFile.Sections.Add(DataProviderKeysSection.SectionName, section);
                overwrite = true;
            }
            if (DataProviderKeysSection.IsNullOrEmpty(section) || overwrite)
            {
                RijndaelManaged rm = new RijndaelManaged();
                rm.GenerateIV();
                rm.GenerateKey();
                section.IV = Strings.SerializeObject(rm.IV);
                section.Key = Strings.SerializeObject(rm.Key);
            }
            configFile.Save();
        }

        private static Configuration GetConfigFile()
        {
            if (HttpContext.Current != null)
            {
                return System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");
            }
            else
            {
                return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
        }

    }
}
