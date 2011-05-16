//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

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
