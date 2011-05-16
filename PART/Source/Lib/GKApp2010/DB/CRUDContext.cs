//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

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
