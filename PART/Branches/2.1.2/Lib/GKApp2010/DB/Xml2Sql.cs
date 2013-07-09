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

using System.Collections;
using System.Xml;

namespace GKApp2010.DB
{
    // ================================================================================
    public class Xml2Sql
    {
        // --------------------------------------------------------------------------------
        public static string EnumeratedTag2Sql(XmlNode root, string tagName, string columnName, bool wrapInQuotes)
        {
            string s = "";
            string enumSql = "";

            XmlNodeList nodelist = root.SelectNodes("//" + tagName);
            foreach (XmlNode node in nodelist)
            {
                s += node.InnerText + ",";
            }

            char[] ca = { ',' };
            string[] sa = s.Split(ca, StringSplitOptions.RemoveEmptyEntries);

            // Build a set to filter out any duplicates
            SortedSet<string> sortedSet = new SortedSet<string>();

            // Remove any whitespace and populate set
            for (int i = 0; i < sa.Length; i++)
            {
                s = sa[i].Trim();
                if (s.Length > 0)
                {
                    sortedSet.Add(s);
                }
            }

            // Check if one of the items is -1 ... and iff so, return "select everything" statement
            if (sortedSet.Contains("-1") && !wrapInQuotes)
            {
                enumSql = "1=1 ";
            }
            else
            {
                if (sortedSet.Count > 0)
                {
                    foreach (string s2 in sortedSet)
                    {
                        enumSql += wrapInQuotes ? "'" + s2 + "'," : s2 + ",";
                    }

                    // Remove trailing comma
                    enumSql = enumSql.TrimEnd(ca);

                    // Build SQL IN clause
                    enumSql = columnName + " IN (" + enumSql + ")";

                    enumSql += " ";
                }
            }

            return enumSql;
        }

        // --------------------------------------------------------------------------------
        public static string BuildANDCombinedWhereClause(List<string> sqlANDList)
        {
            string AND = "AND ";
            string whereClause = "";

            foreach (string s in sqlANDList)
            {
                if (s.Length > 0)
                {
                    whereClause += s + AND;
                }
            }

            // Remove any trailing AND statement
            whereClause = whereClause.EndsWith(AND) ? whereClause.Substring(0, whereClause.Length - AND.Length) : whereClause;

            // If all EnumList, OUType, ... tag list is empty (ie only <XMLSelect></XMLSelect> specified), create "select everything" where clause
            whereClause = whereClause.Length == 0 ? "1=1" : whereClause;

            return whereClause;
        }
    }
}
