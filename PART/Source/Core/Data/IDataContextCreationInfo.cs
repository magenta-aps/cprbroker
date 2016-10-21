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
        string DDL { get; }
        KeyValuePair<string, string>[] Lookups { get; }
        Action<SqlConnection> CustomInitializer { get; }
    }
}
