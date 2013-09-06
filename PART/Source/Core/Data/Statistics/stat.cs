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
using System.Reflection;
using System.Data;
using System.Data.Linq;

namespace CprBroker.Data.Statistics
{
    /// <summary>
    /// Represents rows of the sys.stats catalog view
    /// </summary>
    public partial class stat
    {

        /// <summary>
        /// Counts the number of rows in a database table
        /// A faster alternative to COUNT(*) based on index statistics for the table's primary key
        /// Statistics are recomputed if the age of last statistics is larger than <paramref name="maxPeriod"/>
        /// </summary>        
        /// <typeparam name="TTable">Table object type</typeparam>
        /// <param name="connectionString">Connection string</param>
        /// <param name="maxPeriod">Maximum age of statistics</param>
        /// <returns>The number of rows, or null if no statistics are defined on the database table</returns>
        public static long? CountRowsByStatistics<TTable>(string connectionString, TimeSpan maxPeriod)
        {
            var tableName = Utilities.DataLinq.GetTableName<TTable>();
            return CountRowsByStatistics(tableName, connectionString, maxPeriod);
        }

        /// <summary>
        /// Counts the number of rows in a database table
        /// A faster alternative to COUNT(*) based on index statistics for the table's primary key
        /// Statistics are recomputed if the age of last statistics is larger than <paramref name="maxPeriod"/>
        /// </summary>
        /// <param name="tableName">Name of database table</param>
        /// <param name="connectionString">Connection string</param>
        /// <param name="maxPeriod">Maximum age of statistics</param>
        /// <returns>The number of rows, or null if no statistics are defined on the database table</returns>
        public static long? CountRowsByStatistics(string tableName, string connectionString, TimeSpan maxPeriod)
        {
            using (var dataContext = new StatisticsDataContext(connectionString))
            {
                DateTime? lastUpdated = DateTime.MinValue;

                var statObj = GetPrimaryKeyStat(dataContext, tableName);

                if (statObj != null)
                {
                    lastUpdated = statObj.GetStatsDate(dataContext);

                    if (!lastUpdated.HasValue || lastUpdated + maxPeriod < DateTime.Now)
                    {
                        statObj.UpdateStatistics(dataContext);
                    }
                    var ds = statObj.GetStatistics(connectionString);
                    return (long)ds.Tables[0].Rows[0]["Rows"];

                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Loads the stats object belonging to the table's primary key
        /// </summary>
        /// <param name="dataContext">Database context used to run</param>
        /// <param name="tableName">Name of database table</param>
        /// <returns>The found object</returns>
        public static stat GetPrimaryKeyStat(StatisticsDataContext dataContext, string tableName)
        {
            return (from s in dataContext.stats
                    where
                        s.@object.name == tableName
                        && s.auto_created.HasValue
                        && s.auto_created.Value == false
                    orderby s.stats_id
                    select s
            )
            .FirstOrDefault();
        }

        /// <summary>
        /// Calls STATS_DATE function for the current object
        /// </summary>
        /// <param name="dataContext">Database context used to execute the database call</param>
        /// <returns>The last date at which statistics have been calculated for this object</returns>
        public DateTime? GetStatsDate(DataContext dataContext)
        {
#if Mono
            return dataContext.ExecuteQuery<object>(
                "select STATS_DATE({0}, {1})", this.object_id, this.stats_id
            )
            .Select(o => o as DateTime?)
            .FirstOrDefault();
#else
            return dataContext.ExecuteQuery<DateTime?>(
                "select STATS_DATE({0}, {1})", this.object_id, this.stats_id
            ).FirstOrDefault();
#endif

        }

        /// <summary>
        /// Calls 'update statistics' for the current object
        /// </summary>
        /// <param name="dataContext">Database context used to execute the database call</param>
        public void UpdateStatistics(DataContext dataContext)
        {
            dataContext.ExecuteCommand(
                string.Format("update statistics [{0}] [{1}]", this.@object.name, this.name)
            );
        }

        /// <summary>
        /// Calls DBCC SHOW_STATISTICS for the current object
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>A DataSet object containing the result</returns>
        public DataSet GetStatistics(string connectionString)
        {
            return Utilities.Sql.FillDataSet(
                string.Format("dbcc show_statistics ([{0}],[{1}])", this.@object.name, this.name),
                connectionString
            );
        }

    }
}
