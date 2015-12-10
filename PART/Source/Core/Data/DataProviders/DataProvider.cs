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

namespace CprBroker.Data.DataProviders
{
    /// <summary>
    /// Represents the DataProvider table
    /// </summary>
    public partial class DataProvider : IHasEncryptedAttributes
    {
        public static System.Security.Cryptography.RijndaelManaged EncryptionAlgorithm { get; set; }
        // TODO: Do not rely on this static method for the encryption algorithm
        System.Security.Cryptography.RijndaelManaged IHasEncryptedAttributes.EncryptionAlgorithm
        {
            get { return DataProvider.EncryptionAlgorithm; }
            set { DataProvider.EncryptionAlgorithm = value; }
        }
        public List<AttributeType> Attributes { get; set; }
        System.Data.Linq.Binary IHasEncryptedAttributes.EncryptedData
        {
            get { return this.Data; }
            set { this.Data = value; }
        }

        partial void OnLoaded()
        {
            this.PreLoadAttributes();
        }

        public DataProviderType ToXmlType()
        {
            return new DataProviderType()
            {
                TypeName = TypeName,
                Enabled = IsEnabled,
                Attributes = this.GetAttributes()
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
                dbProv.Set(propName, oioProp.Value);
            }
            return dbProv;
        }
    }

    public class DataProviderDatabaseReader : IEnumerable<DataProvider>
    {

        IEnumerator<DataProvider> IEnumerable<DataProvider>.GetEnumerator()
        {
            using (var dataContext = new CprBroker.Data.DataProviders.DataProvidersDataContext())
            {
                var dbProviders = (from prov in dataContext.DataProviders
                                   where prov.IsEnabled == true
                                   orderby prov.Ordinal
                                   select prov).ToArray();
                return dbProviders.AsEnumerable().GetEnumerator();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<DataProvider>).GetEnumerator();
        }
    }
}
