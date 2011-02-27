using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backend
{
    public abstract class Messages
    {
        public static readonly string BackEndServiceRunsAsNetworkService = "The Back end service is running under the NetworkService account. Please make sure this account has sufficient privileges to read data and execute stored procedures on SQL server";
    }
}
