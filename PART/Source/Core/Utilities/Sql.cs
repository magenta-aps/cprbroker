using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Utilities
{
    /// <summary>
    /// Contains utility methods that support SQL server access
    /// </summary>
    public static class Sql
    {
        /// <summary>
        /// Converts the value of the DateTime to null if is is out of the possible values in SQL Server
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? GetSqlDateTime(DateTime? value)
        {
            if (value == null)
                return null;
            if (value <= Constants.MinSqlDate)
                return null;
            return value;
        }
    }
}
