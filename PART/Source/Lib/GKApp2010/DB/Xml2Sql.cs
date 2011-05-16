//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

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
