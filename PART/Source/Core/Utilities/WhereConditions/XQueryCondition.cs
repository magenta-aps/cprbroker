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
using System.Xml;

namespace CprBroker.Utilities.WhereConditions
{

    public class XQueryCondition : WhereCondition, System.Xml.IXmlNamespaceResolver
    {
        public Dictionary<string, string> Namespaces { get; set; }

        public string Path { get; set; }

        public static List<WhereCondition> CreateXQueryElements(XmlElement element, string columnName)
        {
            return CreateXQueryElements(element, columnName, new Dictionary<string, string>(), "");
        }

        private static List<WhereCondition> CreateXQueryElements(XmlElement element, string columnName, Dictionary<string, string> nsMgr, string basePath)
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
            // TODO: only use [last()] on the elements that have effect periods
            string path = string.Format("{0}/{1}:{2}", basePath, nsMgr[element.NamespaceURI], element.Name);

            var arr = new List<WhereCondition>();

            if (element.ChildNodes.Count == 1 && element.ChildNodes[0].NodeType == XmlNodeType.Text)
            {
                var invalidValues = new string[] { string.Empty, bool.FalseString, "0" };
                if (invalidValues.Where(s => s.Equals(element.InnerText, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() == null)
                {
                    path = string.Format("{0}", path);
                    arr.Add(
                        new XQueryCondition()
                        {
                            ColumnName = columnName,
                            Namespaces = nsMgr,
                            Path = path,
                            Values = new string[] { element.InnerText }
                        }
                    );
                }
                else
                {
                    // Do nothing
                }
            }
            else
            {
                foreach (var child in element.ChildNodes)
                {
                    if (child is XmlElement)
                    {
                        arr.AddRange(CreateXQueryElements(child as XmlElement, columnName, nsMgr, path));
                    }
                }
            }
            return arr;
        }

        public string ToSql(System.Data.SqlClient.SqlCommand sqlCommand)
        {
            string namespaces = string.Join(Environment.NewLine, Namespaces.Select(kvp => string.Format("declare namespace {0}=\"{1}\";", kvp.Value, kvp.Key)).ToArray());

            string parameterName = "@" + Strings.NewRandomString(7);
            sqlCommand.Parameters.Add(parameterName, System.Data.SqlDbType.VarChar).Value = Values[0];

            return ToString(parameterName);
        }

        public override string ToString(string valueExpression)
        {
            string namespaces = string.Join(Environment.NewLine, Namespaces.Select(kvp => string.Format("declare namespace {0}=\"{1}\";", kvp.Value, kvp.Key)).ToArray());
            return string.Format("{0}.value('{1}{2}({3})[{4}]','varchar(max)') = {5}",
                           ColumnName,
                           namespaces,
                           Environment.NewLine,
                           Path,
                           CprBroker.Config.Properties.Settings.Default.CprDirectReturnsNewestFirst ? "1" : "last()",
                           valueExpression
                           );
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
