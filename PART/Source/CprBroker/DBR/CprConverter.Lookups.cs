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
 * Dennis Amdi Skov Isaksen
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
using System.IO;
using System.Data.SqlClient;
using System.Data.Linq.Mapping;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using CprBroker.DBR.Extensions;

namespace CprBroker.DBR
{
    public partial class CprConverter
    {
        public static Dictionary<string, string> SplitFile(string path, Dictionary<string, Type> objectMap)
        {
            var ret = new Dictionary<string, string>();
            var streams = new Dictionary<string, StreamWriter>();

            using (var source = new System.IO.StreamReader(path, Encoding.GetEncoding(1252)))
            {
                while (!source.EndOfStream)
                {
                    int batchSize = 100;
                    var wrappers = CprBroker.Providers.CPRDirect.CompositeWrapper.Parse(source, objectMap, batchSize);

                    foreach (var w in wrappers)
                    {
                        var code = w.Contents.Substring(0, 3);
                        StreamWriter target;
                        if (streams.ContainsKey(code))
                        {
                            target = streams[code];
                        }
                        else
                        {
                            var fileName = string.Format("{0}-{1}-{2}.txt", path, code, w.GetType().Name);
                            target = new StreamWriter(fileName, false, Encoding.GetEncoding(1252));
                            streams[code] = target;
                            ret[code] = fileName;
                        }
                        target.Write(w.Contents);
                    }
                }
            }
            foreach (var kvp in streams)
            {
                kvp.Value.Close();
            }
            return ret;
        }

        public static int ImportLookups(string fileName, int batchSize, Encoding encoding, string dprConnectionString, Dictionary<string, Type> objectMap)
        {
            var ret = 0;

            var fileMap = SplitFile(fileName, objectMap);

            var extensionType = typeof(CprConverterExtensions);
            foreach (var kvp in fileMap)
            {
                // Init type parameters
                var typeKey = kvp.Key;
                var typeFile = kvp.Value;
                var sourceType = objectMap[kvp.Key];
                var method = extensionType.GetMethods().Where(m =>
                    {
                        var pars = m.GetParameters();
                        return pars.Length == 1 && pars.Where(p => p.ParameterType.Equals(sourceType)).Count() == 1;
                    }).SingleOrDefault();
                if (method != null)
                {
                    var targetType = method.ReturnType;
                    Converter<object, object> converter = (o) => method.Invoke(o, new object[] { o });

                    // Call batch conversion and insertion
                    ret += ImportLookup(typeFile, targetType, batchSize, encoding, dprConnectionString, converter, objectMap);
                }
            }
            return ret;
        }

        public static int ImportLookup(string fileName, Type targetType, int batchSize, Encoding encoding, string dprConnectionString,
            Converter<object, object> func, Dictionary<string, Type> objectMap)
        {
            int totalReadLinesCount = 0;
            var tableName = Utilities.DataLinq.GetTableName(targetType);

            var loadedKeys = new Dictionary<string, bool>();
            var keyProps = targetType.GetProperties()
                .Select(p => new { Property = p, Attribute = p.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() as ColumnAttribute })
                .Where(p => p.Attribute != null && p.Attribute.IsPrimaryKey)
                .Select(p => p.Property)
                .ToArray();

            using (var file = new StreamReader(fileName, encoding))
            {
                using (var conn = new SqlConnection(dprConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(string.Format("truncate table [{0}];", tableName), conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Start reading the file
                    while (!file.EndOfStream)
                    {
                        var wrappers = CompositeWrapper.Parse(file, objectMap, batchSize);
                        var batchReadLinesCount = wrappers.Count;

                        var dprObjects = wrappers.Select(w => func(w)).ToArray();
                        if (keyProps.Length > 0)
                        {
                            var filtered = dprObjects
                                .GroupBy(o =>
                                {
                                    var values = keyProps.Select(p => p.GetValue(o, null).ToString()).ToArray();
                                    return string.Join("_", values);
                                });
                            filtered = filtered
                                .Where(g => !loadedKeys.ContainsKey(g.Key))
                                .ToArray();
                            dprObjects = filtered.Select(g => g.First()).ToArray();
                            foreach (var g in filtered)
                            {
                                loadedKeys[g.Key] = true;
                            }   
                        }

                        conn.BulkInsertAll(targetType, dprObjects);
                        totalReadLinesCount += batchReadLinesCount;
                    }
                }
            }
            return totalReadLinesCount;
        }
    }
}
