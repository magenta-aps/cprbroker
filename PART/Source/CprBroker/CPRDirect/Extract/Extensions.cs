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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CprBroker.Providers.CPRDirect
{
    public static class Extensions
    {
        public static void BulkInsertAll<T>(this SqlConnection conn, IEnumerable<T> entities, SqlTransaction trans = null)
        {
            Type t = typeof(T);
            BulkInsertAll(conn, t, entities, trans);
        }

        public static void BulkInsertAll(this SqlConnection conn, Type t, IEnumerable entities, SqlTransaction trans = null)
        {
            SqlBulkCopy bulkCopy = trans == null ?
                new SqlBulkCopy(conn)
                : new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, trans);

            bulkCopy.DestinationTableName = Utilities.DataLinq.GetTableName(t);
            bulkCopy.BatchSize = 100;

            var properties = t.GetProperties()
                .Select(p => new { Property = p, ColumnAttribute = Attribute.GetCustomAttribute(p, typeof(ColumnAttribute)) as ColumnAttribute })
                .Where(o => o.ColumnAttribute != null)
                .ToArray();
            var table = new DataTable();

            foreach (var property in properties)
            {
                Type propertyType = property.Property.PropertyType;
                if (propertyType.IsGenericType &&
                    propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                }

                var columnName = string.IsNullOrEmpty(property.ColumnAttribute.Name) ? property.Property.Name : property.ColumnAttribute.Name;
                table.Columns.Add(new DataColumn(property.Property.Name, propertyType));
                bulkCopy.ColumnMappings.Add(property.Property.Name, columnName);
            }

            foreach (var entity in entities)
            {
                table.Rows.Add(properties.Select(
                  property => GetPropertyValue(
                  property.Property.GetValue(entity, null))).ToArray());
            }

            bulkCopy.WriteToServer(table);
        }

        public static void BulkInsertChanges(this SqlConnection conn, DataContext dataContext, SqlTransaction trans = null)
        {
            var inserts = dataContext.GetChangeSet().Inserts;
            BulkInsertChanges(conn, inserts, trans);
        }

        public static void BulkInsertChanges(this SqlConnection conn, IList<object> inserts, SqlTransaction trans = null)
        {
            var groups = inserts.GroupBy(i => i.GetType()).ToArray();
            foreach (var group in groups)
            {
                BulkInsertAll(conn, group.Key, group.ToArray(), trans);
            }
        }

        private static object GetPropertyValue(object o)
        {
            if (o == null)
                return DBNull.Value;
            return o;
        }

        public static void DeleteAll<T>(this SqlConnection conn, SqlTransaction transaction)
        {
            Type t = typeof(T);
            var tableAttribute = (TableAttribute)t.GetCustomAttributes(typeof(TableAttribute), false).Single();
            var command = new SqlCommand(string.Format("DELETE [{0}]", tableAttribute.Name.Replace(".", "].[")), conn, transaction);

            int ret = command.ExecuteNonQuery();
        }
    }
}
