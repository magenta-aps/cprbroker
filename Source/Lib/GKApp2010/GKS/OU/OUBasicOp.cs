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

using System.Data;

using System.Text.RegularExpressions;

using GKApp2010.DB;

namespace GKApp2010.GKS.OU
{
    // ================================================================================
    public class OUBasicOp
    {
        // --------------------------------------------------------------------------------
        public static int GetList(string context, string xmlSelect, ref DataSet ds, ref string aux)
        {
            return GetList(context, xmlSelect, ref ds, ref aux, false);
        }

        // --------------------------------------------------------------------------------
        public static int GetList(string context, string xmlSelect, ref DataSet ds, ref string aux, bool extendedResult)
        {
            string whereClause = "1=1";
            List<string> sqlANDList = new List<string>();

            xmlSelect = (xmlSelect == null ? "" : xmlSelect);
            xmlSelect = xmlSelect.Trim();

            // Prevent some kinds of injection attacks. TODO Upgrade with better anti SQL injection attack prevention, ie escaping, etc. 20110427/SDE
            //if (Regex.IsMatch(xmlSelect, @"[;%\?\[\]]*"))
            //{
            //    throw new ArgumentOutOfRangeException("The 'xmlSelect' argument contains one or more illegal characters. 'xmlSelect' value=(" + xmlSelect + "). ");
            //}

            //Match m = Regex.Match(xmlSelect, @"[;%\?\[\]]*");

            if (xmlSelect.Length > 0)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlSelect);

                XmlNode root = doc.SelectSingleNode("XMLSelect");
                if (root == null)
                {
                    throw new ArgumentOutOfRangeException("The 'xmlSelect' argument is missig mandatory <XMLSelect></XMLSelect> root tag. 'xmlSelect' value=(" + xmlSelect + "). ");
                }
                else
                {
                    sqlANDList.Add(Xml2Sql.EnumeratedTag2Sql(root, "EnumList", "OUId", false));
                    sqlANDList.Add(Xml2Sql.EnumeratedTag2Sql(root, "OUType", "OUTypeId", false));
                }

                whereClause = Xml2Sql.BuildANDCombinedWhereClause(sqlANDList);
            }

            // Wrap whereClause in paranteses
            whereClause = "( " + whereClause + " )";

            return OUBasicOpDAL.SearchOUWhere(context, whereClause, ref ds, ref aux, extendedResult);
        }
    }

    // ================================================================================
    internal class OUBasicOpDAL
    {
        // --------------------------------------------------------------------------------
        public static int SearchOUWhere(string context, string whereClause, ref DataSet ds, ref string aux, bool extendedResult)
        {
            //CREATE PROCEDURE spGKv2_OU_BasicOp_SearchWhere
            //    @context            VARCHAR(1020),
            //    @whereClause        VARCHAR(1020),
            //    @extendedResult     INTEGER,
            //    @aux                VARCHAR(1020)       OUTPUT

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);
            StoredProcedureCallContext spContext = new StoredProcedureCallContext("GKSDB", "spGKv2_OU_BasicOp_SearchWhere");

            spContext.AddInParameter("whereClause", DbType.String, whereClause);
            spContext.AddInParameter("extendedResult", DbType.Int32, (extendedResult ? 1 : 0));

            ds = spContext.ExecuteDataSet();

            aux = spContext.Aux;
            return spContext.ReturnValue;
        }
    }
}
