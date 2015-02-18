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
using System.Reflection;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Data.SqlClient;

namespace CprBroker.Utilities
{
    public static class DataLinq
    {
        public static string GetTableName<TTable>()
        {
            Type tableType = typeof(TTable);
            return GetTableName(tableType);
        }
        /// <summary>
        /// Gets the name of the table that the <typeparamref name="TTable"/> is mapped to
        /// </summary>
        /// <typeparam name="TTable">Type of object</typeparam>
        /// <exception cref="ArgumentException"><typeparamref name="TTable"/> has no TableAttribute defined</exception>
        /// <returns>Table name</returns>
        public static string GetTableName(Type tableType)
        {
            var attrType = typeof(TableAttribute);
            TableAttribute tableAttribute = null;
            var tempTableType = tableType;
            do
            {
                tableAttribute = tempTableType.GetCustomAttributes(attrType, true).FirstOrDefault() as TableAttribute;
                tempTableType = tempTableType.BaseType;
            }
            while (tableAttribute == null && tempTableType != null && tempTableType.BaseType != null);

            if (tableAttribute == null)
            {
                throw new ArgumentException(string.Format("{0} is not defined on type {1}", attrType.Name, tableType.FullName));
            }
            if (string.IsNullOrEmpty(tableAttribute.Name))
            {
                return tableType.Name;
            }
            else
            {
                return tableAttribute.Name.Split('.').Last();
            }
        }

        /// <summary>
        /// Fills a LINQ table object from a SQL query
        /// Used mainly to handle the case of new columns that do not exist in all setups, so we can dynamically fill the objects without defining a LINQ discriminator column
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataContext"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static IEnumerable<T> Fill<T>(this DataContext dataContext, string cmd)
            where T : new()
        {
            using (SqlDataAdapter adpt = new SqlDataAdapter(cmd, dataContext.Connection as SqlConnection))
            {
                var props = typeof(T).GetProperties()
                    .Select(p => new { Prop = p, Attr = Attribute.GetCustomAttribute(p, typeof(ColumnAttribute)) as ColumnAttribute })
                    .Where(p => p.Attr != null)
                    .ToDictionary(p => string.IsNullOrEmpty(p.Attr.Name) ? p.Prop.Name : p.Attr.Name);

                DataTable t = new DataTable();
                adpt.Fill(t);

                var ret = new List<T>();
                var rows = new DataRow[t.Rows.Count];
                t.Rows.CopyTo(rows, 0);

                ret.AddRange(rows.Select(r =>
                {
                    var o = new T();
                    foreach (DataColumn c in t.Columns)
                    {
                        var p = props[c.ColumnName].Prop;
                        var val = r[c];
                        if (val != DBNull.Value)
                        {
                            if (p.PropertyType.Equals(typeof(char)) || p.PropertyType.Equals(typeof(char?)))
                            {
                                p.SetValue(o, val.ToString()[0], null);
                            }
                            else
                            {
                                p.SetValue(o, val, null);
                            }
                        }
                    }
                    return o;
                }));
                return ret;
            }
        }

        /// <summary>
        /// Limits the input to one entity per primary key instance
        /// Used to avoid PK collisions in Linq2Sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T>(IEnumerable<T> arr)
        {
            var t = typeof(T);
            var props = t.GetProperties()
                .Select(p => new { Prop = p, Attr = Attribute.GetCustomAttribute(p, typeof(ColumnAttribute)) })
                .Where(p => p.Attr != null);

            Func<T, string> pkGetter = o =>
                {
                    var str = string.Join("|",
                        Enumerable
                        .Range(0, props.Count())
                        .Select(i => "{" + i + "}")
                        .ToArray()
                        );
                    var vals = props.Select(p => p.Prop.GetValue(o, null)).ToArray();
                    return string.Format(str, vals);
                };

            return arr.GroupBy(o => pkGetter(o))
                .Select(g => g.First())
                .ToArray();
        }

    }
}
