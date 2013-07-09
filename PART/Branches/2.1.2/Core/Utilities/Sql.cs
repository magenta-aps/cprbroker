/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS"basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The CPR Broker concept was initally developed by
 * Gentofte Kommune / Municipality of Gentofte, Denmark.
 * Contributor(s):
 * Steen Deth
 *
 *
 * The Initial Code for CPR Broker and related components is made in
 * cooperation between Magenta, Gentofte Kommune and IT- og Telestyrelsen /
 * Danish National IT and Telecom Agency
 *
 * Contributor(s):
 * Beemen Beshara
 * Niels Elgaard Larsen
 * Leif Lodahl
 * Steen Deth
 *
 * The code is currently governed by IT- og Telestyrelsen / Danish National
 * IT and Telecom Agency
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

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

        /// <summary>
        /// Fills a DataSet object with the result of a SQL query
        /// </summary>
        /// <param name="command">SQL string to execute</param>
        /// <param name="connectionString">SQL connection string</param>
        /// <returns>A DataSet object containing the returned data</returns>
        public static DataSet FillDataSet(string command, string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var adpt = new SqlDataAdapter(command, conn))
                {
                    DataSet ret = new DataSet();
                    adpt.Fill(ret);
                    return ret;
                }
            }
        }
    }
}
