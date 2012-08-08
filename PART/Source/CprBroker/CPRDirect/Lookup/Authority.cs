using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;

namespace CprBroker.Providers.CPRDirect
{
    /// <summary>
    /// Database object for authority
    /// </summary>
    public partial class Authority
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
                ImportText(text, conn);
            }
        }

        public static void ImportText(string text, SqlConnection conn)
        {
            var authorities = LineWrapper.ParseBatch(text)
                    .Select(line => line.ToWrapper(Constants.DataObjectMap_P02680))
                    .Where(w => w != null)
                    .Select(w => (w as AuthorityType).ToAuthority());

            using (var trans = conn.BeginTransaction())
            {
                conn.DeleteAll<Authority>(trans);
                conn.BulkInsertAll<Authority>(authorities, trans);
                trans.Commit();
            }
        }

        public static string GetNameByCode(string code)
        {
            using (var dataContext = new LookupDataContext())
            {
                return dataContext
                    .Authorities
                    .Where(auth => auth.AuthorityCode == code)
                    .Select(auth => auth.FullName)
                    .FirstOrDefault();
            }
        }

    }
}
