﻿/* ***** BEGIN LICENSE BLOCK *****
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
using System.Xml;

namespace CprBroker.Utilities
{
    public class XQueryElement : System.Xml.IXmlNamespaceResolver
    {
        public Dictionary<string, string> Namespaces { get; set; }
        public string Path { get; set; }
        public string Value { get; set; }

        public static XQueryElement[] CreateXQueryElements(XmlElement element)
        {
            return CreateXQueryElements(element, new Dictionary<string, string>(), "");
        }

        private static XQueryElement[] CreateXQueryElements(XmlElement element, Dictionary<string, string> nsMgr, string basePath)
        {
            nsMgr = new Dictionary<string, string>(nsMgr);

            // Put namespace
            if (!string.IsNullOrEmpty(element.NamespaceURI))
            {
                if (!nsMgr.ContainsKey(element.NamespaceURI))
                {
                    nsMgr[element.NamespaceURI] = "ns" + nsMgr.Count;
                }
            }

            string path = string.Format("{0}/{1}:{2}", basePath, nsMgr[element.NamespaceURI], element.Name);

            if (element.ChildNodes.Count == 1 && element.ChildNodes[0].NodeType == XmlNodeType.Text)
            {
                var invalidValues = new string[] { string.Empty, bool.FalseString, "0" };
                if (invalidValues.Where(s => s.Equals(element.InnerText, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() == null)
                {
                    path = string.Format("{0}", path);
                    return new XQueryElement[]
                    {
                        new XQueryElement()
                        {
                            Namespaces = nsMgr,
                            Path = path,
                            Value = element.InnerText
                        }
                    };
                }
                else
                {
                    return new XQueryElement[0];
                }
            }
            else
            {
                var arr = new List<XQueryElement>();

                foreach (var child in element.ChildNodes)
                {
                    if (child is XmlElement)
                    {
                        arr.AddRange(CreateXQueryElements(child as XmlElement, nsMgr, path));
                    }
                }
                return arr.ToArray();
            }
        }

        public string ToSql(string columnName, System.Data.SqlClient.SqlCommand sqlCommand)
        {
            string namespaces = string.Join(Environment.NewLine, Namespaces.Select(kvp => string.Format("declare namespace {0}=\"{1}\";", kvp.Value, kvp.Key)).ToArray());
            string parameterName = "@" + Strings.NewRandomString(7);
            sqlCommand.Parameters.Add(parameterName, System.Data.SqlDbType.VarChar).Value = Value;

            return string.Format("{0}.value('{1}{2}({3})[1]','varchar(max)') = {4}",
                columnName,
                namespaces,
                Environment.NewLine,
                Path,
                parameterName
                );
        }

        public static IEnumerable<T> GetMatchingObjects<T>(System.Data.Linq.DataContext dataContex, IEnumerable<XQueryElement> elements, string tableName, string columnName, string[] columnNames)
        {
            var where = new List<string>();

            foreach (var elem in elements)
            {
                string namespaces = string.Join(Environment.NewLine, elem.Namespaces.Select(kvp => string.Format("declare namespace {0}=\"{1}\";", kvp.Value, kvp.Key)).ToArray());

                var myWhere = string.Format("{0}.value('{1}{2}({3})[1]','varchar(max)') = {4}",
                    columnName,
                    namespaces,
                    Environment.NewLine,
                    elem.Path,
                    string.Format("{{{0}}}", where.Count)
                    );
                where.Add(myWhere);
            }

            string sql = string.Format("SELECT {0} FROM {1} WHERE {2}",
                string.Join(",", columnNames),
                tableName,
                string.Join(" AND ", where.ToArray())
                );
            return dataContex.ExecuteQuery<T>(sql, elements.AsQueryable().Select(elem => elem.Value).ToArray());
        }

        public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
        {
            var ret = new Dictionary<string, string>(Namespaces.Count);
            foreach (var kvp in this.Namespaces)
            {
                ret[kvp.Value] = kvp.Key;
            }
            return ret;
        }

        public string LookupNamespace(string prefix)
        {
            return Namespaces.Where(kvp => kvp.Value == prefix).Select(kvp => kvp.Key).FirstOrDefault();
        }

        public string LookupPrefix(string namespaceName)
        {
            return Namespaces[namespaceName];
        }
    }
}
