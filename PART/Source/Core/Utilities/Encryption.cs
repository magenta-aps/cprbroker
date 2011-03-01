using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;

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

        private static byte[] RijndaelEncryptionKey
        {
            get
            {
                if (!string.IsNullOrEmpty(Config.Properties.Settings.Default.RijndaelEncryptionKeyFile))
                {
                    string text = File.ReadAllText(Config.Properties.Settings.Default.RijndaelEncryptionKeyFile);
                    string[] arr = text.Split(new char[] { ' ', ';', ',', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
                    var ret = Array.ConvertAll<string, byte>(arr, s => byte.Parse(s));
                    return ret;
                }
                else
                {
                    return new byte[] { 64, 95, 48, 189, 85, 19, 22, 231, 82, 67, 73, 99, 33, 233, 112, 174, 64, 95, 48, 189, 85, 19, 22, 231, 82, 67, 73, 99, 33, 233, 112, 174 };
                }
            }
        }

        private static byte[] RijndaelIV = new byte[] { 55, 35, 92, 173, 56, 28, 94, 202, 55, 35, 92, 173, 56, 28, 94, 202 };

        public static byte[] EncryptObject(object o)
        {
            var ret = new List<byte>();
            var xml = Strings.SerializeObject(o);

            RijndaelManaged m = new RijndaelManaged() { IV = RijndaelIV, Key = RijndaelEncryptionKey };

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
            RijndaelManaged m = new RijndaelManaged() { IV = RijndaelIV, Key = RijndaelEncryptionKey };
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
