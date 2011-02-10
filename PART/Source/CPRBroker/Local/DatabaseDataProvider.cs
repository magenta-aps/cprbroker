using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using CprBroker.Engine;
using CprBroker.DAL;
using CprBroker.Schemas;
using CprBroker.Schemas.Util;
using CprBroker.Engine.Local;

namespace CprBroker.Providers.Local
{
    /// <summary>
    /// Handles implementation of data provider using the system's local database
    /// </summary>
    public partial class DatabaseDataProvider : IDataProvider
    {
        #region IDataProvider Members

        bool IDataProvider.IsAlive()
        {
            return true;
        }

        Version IDataProvider.Version
        {
            get { return new Version(Versioning.Major, Versioning.Minor); }
        }

        #endregion
    }
}
