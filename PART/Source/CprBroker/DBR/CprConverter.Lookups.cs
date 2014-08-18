using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using CprBroker.Providers.DPR;
using CprBroker.Providers.CPRDirect;
using CprBroker.DBR.Extensions;

namespace CprBroker.DBR
{
    public partial class CprConverter
    {
        public static Dictionary<string, string> SplitFile(string path)
        {
            var ret = new Dictionary<string, string>();
            var streams = new Dictionary<string, StreamWriter>();

            using (var source = new System.IO.StreamReader(path, Encoding.GetEncoding(1252)))
            {
                while (!source.EndOfStream)
                {
                    int batchSize = 100;
                    var wrappers = CprBroker.Providers.CPRDirect.CompositeWrapper.Parse(source, CprBroker.Providers.CPRDirect.Constants.DataObjectMap_P05780, batchSize);

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

        public int ImportLookups(string fileName, Encoding encoding, string dprConnectionString)
        {
            var ret = 0;
            var objectMap = CprBroker.Providers.CPRDirect.Constants.DataObjectMap_P05780;
            int batchSize = 1000;

            var fileMap = SplitFile(fileName);
            var reverseObjectMap = objectMap
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

            ret += ImportLookup<StreetType, Street>(fileMap[reverseObjectMap[typeof(StreetType)]], dprConnectionString, CprConverterExtensions.ToDprStreet, encoding, batchSize, objectMap);
            return ret;
        }

        public int ImportLookup<TCpr, TDpr>(string fileName, string dprConnectionString, Converter<TCpr, TDpr> func,
            Encoding encoding, int batchSize, Dictionary<string, Type> objectMap)
            where TCpr : class
            where TDpr : class
        {
            int totalReadLinesCount = 0;

            using (var file = new StreamReader(fileName, encoding))
            {
                using (var conn = new SqlConnection(dprConnectionString))
                {
                    conn.Open();
                    // Start reading the file
                    while (!file.EndOfStream)
                    {
                        var wrappers = CompositeWrapper.Parse(file, objectMap, batchSize);
                        var batchReadLinesCount = wrappers.Count;

                        var dprObjects = wrappers.Select(w => func(w as TCpr)).ToArray();

                        conn.BulkInsertAll<TDpr>(dprObjects);
                        totalReadLinesCount += batchReadLinesCount;
                    }
                }
            }
            return totalReadLinesCount;
        }
    }
}
