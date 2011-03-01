using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Data.Events
{
    /// <summary>
    /// Represents the data context for DataChangeEvent table
    /// Everytime a new record is added to PersonRegistration, a new record is added to this table to tell the system that a change has occured
    /// </summary>
    public partial class DataChangeEventDataContext
    {
        public DataChangeEventDataContext()
            : base(Config.Properties.Settings.Default.CprBrokerConnectionString)
        {
            OnCreated();
        }
    }
}
