using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;

namespace CprBroker.Providers.CPRDirect
{
    public static class ExtractManager
    {
        public static void ImportFile(string path)
        {
            var text = File.ReadAllText(path, Constants.DefaultEncoding);
            ImportText(text);
        }

        public static void ImportText(string text)
        {
            using (var conn = new SqlConnection(CprBroker.Config.Properties.Settings.Default.CprBrokerConnectionString))
            {
                conn.Open();

                var extract = new Extract(text, Constants.DataObjectMap);
                conn.BulkInsertAll<Extract>(new Extract[] { extract });
                conn.BulkInsertAll<ExtractItem>(extract.ExtractItems);
            }
        }

        public static IndividualResponseType GetPerson(string pnr)
        {
            using (var dataContext = new ExtractDataContext())
            {
                return Extract.GetPerson(pnr, dataContext.ExtractItems, Constants.DataObjectMap);
            }

        }
    }
}
