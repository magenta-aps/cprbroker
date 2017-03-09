using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Queues
{
    public partial class QueueDataContext : IDataContextCreationInfo
    {
        public QueueDataContext()
            : this(CprBroker.Config.Properties.Settings.Default._CprBrokerConnectionString)
        { }

        #region IDataContextCreationInfo members
        public string[] DDL
        {
            get
            {
                return new string[] {
                    Properties.Resources.Semaphore_Sql,
                    Properties.Resources.Queue_Sql,
                    Properties.Resources.QueueItem_Sql,
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

        public Action<SqlConnection>[] CustomInitializers
        {
            get
            {
                return new Action<SqlConnection>[] { };
            }
        }
        #endregion
    }
}
