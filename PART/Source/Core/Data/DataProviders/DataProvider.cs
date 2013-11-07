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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Utilities;
using System.Security.Cryptography;

namespace CprBroker.Data.DataProviders
{
    /// <summary>
    /// Represents the DataProvider table
    /// </summary>
    public partial class DataProvider
    {
        public static RijndaelManaged EncryptionAlgorithm;

        private readonly List<AttributeType> Properties = new List<AttributeType>();
        private readonly List<AttributeType> Operations = new List<AttributeType>();

        partial void OnLoaded()
        {
            if (Data != null)
            {
                AttributeType[] attributes;
                if (EncryptionAlgorithm == null)
                {
                    attributes = Encryption.DecryptObject<AttributeType[]>(Data.ToArray());
                }
                else
                {
                    attributes = Encryption.DecryptObject<AttributeType[]>(Data.ToArray(), EncryptionAlgorithm);
                }

                for (int i = 0; i < attributes.Length; i++)
                {
                    if (!attributes[i].Name.Contains("Online "))
                        Properties.Add(attributes[i]);
                    else
                        Operations.Add(attributes[i]);
                }
            }
        }

        private AttributeType GetDataProviderProperty(string key)
        {
            return (from p in this.Properties where p.Name == key select p).FirstOrDefault();
        }

        public AttributeType[] GetProperties()
        {
            return Properties.Select(p => new AttributeType() { Name = p.Name, Value = p.Value }).ToArray();
        }

        public AttributeType[] GetOperations()
        {
            return Operations.Select(p => new AttributeType() { Name = p.Name, Value = p.Value }).ToArray();
        }

        public string this[string key]
        {
            get
            {
                var prop = GetDataProviderProperty(key);
                if (prop != null)
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
                if (EncryptionAlgorithm == null)
                {
                    Data = Encryption.EncryptObject(Properties.ToArray());
                }
                else
                {
                    Data = Encryption.EncryptObject(Properties.ToArray(), EncryptionAlgorithm);
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
