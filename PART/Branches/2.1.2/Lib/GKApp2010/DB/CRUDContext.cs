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
using System.Configuration;
using System.Data;
using System.Data.Common;

using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

using System.Text.RegularExpressions;

namespace GKApp2010.DB
{
    // ================================================================================
    public class CRUDContext
    {
        SqlDatabase db = null;
        DbCommand cmd = null;

        const string _useExplicitConnStringPrefix = "EXPLICIT-CONNSTRING:";

        #region constructors
        // -----------------------------------------------------------------------------
        public CRUDContext(string connStrID, string sqlStmt)
        {
            CommonInit(connStrID, sqlStmt, "", "");
        }

        // -----------------------------------------------------------------------------
        public CRUDContext(string connStrID, string sqlStmt, string context)
        {
            CommonInit(connStrID, sqlStmt, context, "");
        }

        // -----------------------------------------------------------------------------
        public CRUDContext(string connStrID, string sqlStmt, string context, string aux)
        {
            CommonInit(connStrID, sqlStmt, context, aux);
        }
        #endregion

        #region public methods
        // -----------------------------------------------------------------------------
        public DataSet ExecuteDataSet()
        {
            DataSet ds = db.ExecuteDataSet(cmd);
            return ds;
        }

        // -----------------------------------------------------------------------------
        public int ExecuteNonQuery()
        {
            return db.ExecuteNonQuery(cmd);
        }
        #endregion

        // -----------------------------------------------------------------------------
        public static string ExplicitConnStringPrefix
        {
            get { return _useExplicitConnStringPrefix; }
        }

        #region private methods
        // -----------------------------------------------------------------------------
        private void CommonInit(string connStrID, string sqlStmt, string context, string aux)
        {
            try
            {
                // If connStrID starts with a special prefix to indicate that 'connStrID' contains an explicit
                // connection string instead of only a config keyname (which is default) ...
                if (Regex.IsMatch(connStrID, "^" + _useExplicitConnStringPrefix, RegexOptions.IgnoreCase))
                {
                    // ... well, then extract the connection string
                    connStrID = connStrID.Substring(_useExplicitConnStringPrefix.Length);
                }
                else
                {
                    // ... otherwise read it from config, using connStrID as config keyname/index
                    connStrID = GKApp2010.Config.Default.GetConnectionString(connStrID);
                }

                db = new SqlDatabase(connStrID);
                cmd = db.GetSqlStringCommand(sqlStmt);

            }
            catch (Exception ex)
            {
                if (connStrID == null) connStrID = "NULL";

                string msg = "Exception inside CommonInit(), connStr=(" + connStrID + "), msg=(" + ex.Message + ")";

                throw new Exception(msg, ex);
            }
        }
        #endregion

    }
}
