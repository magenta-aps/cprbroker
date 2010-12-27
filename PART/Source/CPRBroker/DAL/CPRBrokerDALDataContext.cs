using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.DAL
{
    public partial class CPRBrokerDALDataContext
    {
        private static object _Lock = new object();

        public CPRBrokerDALDataContext()
            : base(Config.Properties.Settings.Default.CPRConnectionString)
        {

        }

        public override void SubmitChanges(System.Data.Linq.ConflictMode failureMode)
        {
            try
            {
                base.SubmitChanges(failureMode);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (ex.Message.Contains("deadlocked on lock resources"))
                {
                    System.Threading.Thread.Sleep(Config.Properties.Settings.Default.DataContextDeadLockWaitMilliseconds);
                    base.SubmitChanges(failureMode);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
