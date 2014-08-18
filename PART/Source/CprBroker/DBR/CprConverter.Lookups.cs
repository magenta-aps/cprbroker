using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CprBroker.DBR
{
    public partial class CprConverter
    {
        public static Dictionary<string, string> SplitFile()
        {
            var ret = new Dictionary<string, string>();
            var streams = new Dictionary<string, StreamWriter>();

            var path = @"C:\Magenta Workspace\broker\PART\Doc\Data Providers\CPR Direct\Lookups\vejregister_hele_landet_pr_140801\A370715.txt";


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
    }
}
