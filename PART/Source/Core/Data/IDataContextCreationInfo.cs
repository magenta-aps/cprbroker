using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Data
{
    public interface IDataContextCreationInfo
    {
        string[] DDL { get; }
        KeyValuePair<string, string>[] Lookups { get; }
        Action<SqlConnection>[] CustomInitializers { get; }
    }

    public static class IDataContextCreationInfoExtensions
    {
        public static string JoinDdl(this IDataContextCreationInfo dataContextInfo, params string[] ddlArray)
        {
            return string.Join(
                Environment.NewLine + "GO" + Environment.NewLine,
                ddlArray);
        }
    }
}
