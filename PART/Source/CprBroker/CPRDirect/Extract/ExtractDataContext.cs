using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities.Config;
using CprBroker.Data;
using System.Data.SqlClient;

namespace CprBroker.Providers.CPRDirect
{
    partial class ExtractDataContext : IDataContextCreationInfo
    {
        public ExtractDataContext()
            : this(ConfigManager.Current.Settings.CprBrokerConnectionString)
        { }

        #region IDataContextCreationInfo
        public string[] DDL
        {
            get
            {
                return new string[]{
                    Properties.Resources.Extract_Sql,
                    Properties.Resources.ExtractPersonStaging_Sql,
                    Properties.Resources.ExtractItem_Sql,
                    Properties.Resources.ExtractError_Sql,
                };
            }
        }

        public KeyValuePair<string, string>[] Lookups
        {
            get
            {
                return new KeyValuePair<string, string>[] { };
            }
        }

        public Action<SqlConnection> CustomInitializer
        {
            get
            {
                return null;
            }
        }
        #endregion
    }
}
