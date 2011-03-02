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
            var ret = new List<byte>();
            var xml = Strings.SerializeObject(o);

            RijndaelManaged m = DataProviderKeysSection.GetFromConfig();

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
            string xml = null;
            RijndaelManaged m = DataProviderKeysSection.GetFromConfig();
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
    }
}
