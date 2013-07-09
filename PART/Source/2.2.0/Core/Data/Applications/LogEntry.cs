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
using System.Data.Linq;
using System.Diagnostics;
using CprBroker.Utilities;
using System.Linq.Expressions;

namespace CprBroker.Data.Applications
{
    public partial class LogEntry
    {
        public static System.Linq.Expressions.Expression<Func<LogEntry, bool>> CreatePredicate(ApplicationDataContext dataContext, DateTime fromDate, DateTime? toDate, TraceEventType? type, string appName)
        {
            var pred = CprBroker.Utilities.PredicateBuilder.True<LogEntry>();

            if (toDate.HasValue && toDate.Value < fromDate)
            {
                var tmp = fromDate;
                fromDate = toDate.Value;
                toDate = tmp;
            }
            // By date
            pred = pred.And(le => le.LogDate >= fromDate);
            if (toDate.HasValue)
            {
                pred = pred.And(le => le.LogDate < toDate.Value.AddDays(1));
            }

            // Byte type
            if (type.HasValue)
            {
                pred = pred.And(le => le.LogTypeId == (int)type.Value);
            }

            // By app
            if (!string.IsNullOrEmpty(appName))
            {
                var appId = dataContext.Applications.Where(app => app.Name == appName).Select(app => app.ApplicationId).FirstOrDefault();
                pred = pred.And(le => le.ApplicationId == appId);
            }
            return pred;
        }

        public static int CountRows(DateTime fromDate, DateTime? toDate, TraceEventType? type, string appName)
        {
            using (ApplicationDataContext dataContext = new ApplicationDataContext())
            {
                var pred = CreatePredicate(dataContext, fromDate, toDate, type, appName);
                return dataContext.LogEntries
                    .Where(pred)
                    .Select(le => le.LogDate)
                    .Count();
            }
        }

        /// <summary>
        /// Reads the LogEntry objects as requested
        /// Makes dummy calls to Application and LogType properties to allow accessing them without the source data context object
        /// </summary>
        /// <param name="startRow">Zero based start row number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>The found LogEntry objects</returns>
        public static List<LogEntry> LoadByPage(DateTime fromDate, DateTime? toDate, TraceEventType? type, string appName, int startRow, int pageSize)
        {
            using (ApplicationDataContext dataContext = new ApplicationDataContext())
            {
                DataLoadOptions options = new DataLoadOptions();
                var pred = CreatePredicate(dataContext, fromDate, toDate, type, appName);

                // Read records
                var ret = dataContext.LogEntries
                    .Where(pred)
                    .OrderByDescending(le => le.LogDate)
                    .Skip(startRow)
                    .Take(pageSize)
                    .ToList();

                foreach (var logEntry in ret)
                {
                    object o = logEntry.Application;
                    object s = logEntry.LogType;
                }
                return ret;
            }
        }
    }
}
