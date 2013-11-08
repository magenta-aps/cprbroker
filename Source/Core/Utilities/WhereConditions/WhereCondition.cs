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

namespace CprBroker.Utilities.WhereConditions
{
    public class WhereCondition
    {
        public string ColumnName { get; set; }
        public string[] Values { get; set; }

        public virtual string ToString(string valueExpression)
        {
            return string.Format("{0} = {1}", ColumnName, valueExpression);
        }

        public static IEnumerable<T> GetMatchingObjects<T>(System.Data.Linq.DataContext dataContext, IEnumerable<WhereCondition> elements, string tableName, string[] columnNames)
#if Mono
            where T : class, new()
#endif
        {
            string sql = string.Format("SELECT {0} FROM {1} WHERE {2}",
                            string.Join(",", columnNames),
                            tableName,
                            elements.ToWhereString()
                            );

            var parameterValues = elements.AsQueryable().SelectMany(elem => elem.Values).ToArray();
            return dataContext.ExecuteQuery<T>(sql, parameterValues);
        }


        public static IEnumerable<T> GetMatchingObjects<T>(System.Data.Linq.DataContext dataContext, IEnumerable<WhereCondition> elements, string tableName, bool distinct, string[] columnNames, int startIndex, int maxCount, string sort)            
#if Mono
            where T : class, new()
#endif
        {
            string tempName = Strings.NewRandomString(10);
            string columnNamesStr = string.Join(",", columnNames);

            string sql = string.Format(""
                + " WITH t{0} AS "
                + " ("
                + "   SELECT TOP {1} *,"
                + "   ROW_NUMBER() OVER (ORDER BY {2}) AS RowNumber{3} "
                + "   FROM "
                + "   ("
                + "     SELECT {4} {5}"
                + "     FROM {6}"
                + "     WHERE {7}"
                + "   ) AS s{8}"
                + " ) "
                + " SELECT {9} "
                + " FROM t{10} "
                + " WHERE RowNumber{11} BETWEEN {12} AND {13}",

                tempName,
                startIndex + maxCount,
                sort,
                tempName,
                distinct ? "DISTINCT" : "",
                columnNamesStr,
                tableName,
                elements.ToWhereString(),
                tempName,
                columnNamesStr,
                tempName,
                tempName,
                startIndex + 1,
                startIndex + maxCount
                );

            var parameterValues = elements.AsQueryable().SelectMany(elem => elem.Values).ToArray();
            return dataContext.ExecuteQuery<T>(sql, parameterValues);
        }
    }


}
