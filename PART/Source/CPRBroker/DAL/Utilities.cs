using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;

namespace CprBroker.DAL
{
    public static class Utilities
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

        public static string EncryptionKeyXml
        {
            get
            {
                if (string.IsNullOrEmpty(Config.Properties.Settings.Default.EncryptionKeyXmlFile))
                {
                    return @"<RSAKeyValue>
  <Modulus>t5DwWMD+NhpRglloEcF1Hc7X2mzD52e9kVXQPs5Ges+sQhk5a1zg9vi3FK/TdDuQYELqazHHbmBhaY5kHC55ge4ubYKNNwrbUBWZwIquw9YbFo+5g4f4zhvyhYfZFEi5X5I8YW49A+9pSoxVojaPZ+qaLm+LHnuPRb1OQMjvAEM=</Modulus>
  <Exponent>AQAB</Exponent>
  <P>90XrhZD0seML3k64laFMerndPAXu2E53UditpBXSKxueDaUdW5cUYGHnMVT3oh/+1a++ecPsI4vxQrwWedeq4w==</P>
  <Q>vgtvVh8e7Ta6/YfUbOamcRAcerlc8uaLZ/5lB8jyyXvGqKfYgXLgZ7vAVhMP1t5dkPmC89QsBkaMa1YW3IpzIQ==</Q>
  <DP>bOqEYlHGJnCusp4UGfxxVoF13FF0shxl3ExHt8XQzCIfDT2UX9p9JDMbhZQ6e1QCiJcfnDzbT5D9lPqKH+MKJw==</DP>
  <DQ>rE834nETHGdcQZWPUDIMxUSjXc6FbSMFUQQCXH2hTHeylqagkjYzKzq7WA+uc9ZoJZNlXWiJhiMfHA8RaWMKoQ==</DQ>
  <InverseQ>onjDR4ztV6/Zaf9EA+Ick3H9wHtmWIo+n4iuk7dFEr7yw9e9IGAbfhTWmDx+PPxGaBqLUA/KFpiVqkK2BRazGg==</InverseQ>
  <D>pnaqbmH9ZcSyG9nGFSvxb+mON0ag1O1vrCc8pGfc5CwFkx9awbDFVVGwfPMBd4s4XwLvn+vRZZfDXrzArgm7JvrqB6sIKMHkDPQzvzx9rsB+dAGTzw3AJGTAbYVs1Zl/AwWFdW+D9P72iUvDyKqVqPKqfdNhtl5TKr0eBqi/d8E=</D>
</RSAKeyValue>";
                }
                else
                {
                    return System.IO.File.ReadAllText(Config.Properties.Settings.Default.EncryptionKeyXmlFile);
                }
            }
        }

        public static byte[] EncryptObject(object o)
        {
            var ret = new List<byte>();
            var xml = SerializeObject(o);
            int maxLen = 80;
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(EncryptionKeyXml);
            for (int i = 0; i < xml.Length; i += maxLen)
            {
                var size = Math.Min(maxLen, xml.Length - i);
                var subStr = xml.Substring(i, size);
                byte[] clearData = Encoding.UTF8.GetBytes(subStr);
                var encryptedData = rsa.Encrypt(clearData, true);
                ret.AddRange(encryptedData);
            }
            return ret.ToArray();
        }

        public static T DecryptObject<T>(byte[] encryptedData)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(EncryptionKeyXml);
            var size = 128;
            StringBuilder xmlBuilder = new StringBuilder();
            for (int i = 0; i < encryptedData.Length; i += size)
            {
                var subEncryptedData = new byte[size];
                Array.Copy(encryptedData, i, subEncryptedData, 0, size);
                var clearData = rsa.Decrypt(subEncryptedData, true);
                var subXml = Encoding.UTF8.GetString(clearData);
                xmlBuilder.Append(subXml);
            }
            var ret = Deserialize<T>(xmlBuilder.ToString());
            return ret;
        }



        /// <summary>
        /// Serializes the given object to an XML string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObject(object obj)
        {
            string ret = "";
            if (obj != null)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, obj);
                ret = writer.ToString();
            }
            return ret;
        }

        public static T Deserialize<T>(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                StringReader writer = new StringReader(xml);
                object o = serializer.Deserialize(writer);
                return (T)o;
            }
            return default(T);
        }
    }
}
