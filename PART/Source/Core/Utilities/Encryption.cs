/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

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
            return EncryptObject(o, CprBroker.Config.ConfigManager.Current.EncryptionAlgorithm);
        }

        public static byte[] EncryptObject(object o, RijndaelManaged m)
        {
            var ret = new List<byte>();
            var xml = Strings.SerializeObject(o);

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
            return DecryptObject<T>(encryptedData, CprBroker.Config.ConfigManager.Current.EncryptionAlgorithm);
        }

        public static T DecryptObject<T>(byte[] encryptedData, RijndaelManaged m)
        {
            string xml = null;

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
