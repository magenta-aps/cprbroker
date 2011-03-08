using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities;

namespace CprBroker.Engine.Ping
{
    public class PingDataProvider:IPingDataProvider
    {

        #region IPingDataProvider Members

        public bool Ping()
        {
            return true;
        }

        #endregion

        #region IDataProvider Members

        public bool IsAlive()
        {
            return true;
        }

        public Version Version
        {
            get { return new Version(Constants.Versioning.Major, Constants.Versioning.Minor); }
        }

        #endregion
    }
}
