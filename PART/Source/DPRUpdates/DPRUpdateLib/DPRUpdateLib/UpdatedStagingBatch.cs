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
using System.Data;
using System.Data.SqlClient;
using System.Text;

using GKApp2010.DB;
using GKApp2010.Threads;

namespace UpdateLib
{
    // ================================================================================
    public class UpdatedStagingBatch
    {
        UpdateDetectionVariables _UpdateDetectionVariables;
        IsStopRequestedFunc _shouldIStop = null;
        DataSet dsUpdatedCPRs = null;

        // -----------------------------------------------------------------------------
        public UpdatedStagingBatch(UpdateDetectionVariables updateDetectionVariables, IsStopRequestedFunc shouldIStop)
        {
            _UpdateDetectionVariables = updateDetectionVariables;
            _shouldIStop = shouldIStop;

            GetBatchFromDB();
        }

        public string DatabaseName
        {
            get
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(GKApp2010.Config.Default.GetConnectionString(_UpdateDetectionVariables.ConnectionStringName));
                return builder.InitialCatalog;
            }
        }

        // -----------------------------------------------------------------------------
        public List<string> GetUpdatedPersonsList()
        {
            List<string> updatedPersons = new List<string>();

            updatedPersons.Clear();

            //int cnt = 0;
            if (dsUpdatedCPRs != null)
            {
                DataTable dt = dsUpdatedCPRs.Tables[0];
                if (dt != null)
                {
                    int pnrColIdx = dt.Columns.IndexOf(_UpdateDetectionVariables.PnrColumnName);

                    decimal prevPnr = 0m;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (ShouldIStop())
                            break;

                        decimal currPnr = (decimal)row[pnrColIdx];

                        if (prevPnr != currPnr)
                        {
                            updatedPersons.Add(GetCprNoAsStringFromDecimal(currPnr));
                            prevPnr = currPnr;
                        }

                        //cnt++;
                    }
                }
            }

            //Console.WriteLine("Noof recs=(" + cnt.ToString() + ")");
            return updatedPersons;
        }

        // -----------------------------------------------------------------------------
        public void DeletePersonFromStaging(string cprNo)
        {
            if (dsUpdatedCPRs != null)
            {
                DataTable dt = dsUpdatedCPRs.Tables[0];
                if (dt != null)
                {
                    decimal pnr = GetCprNoAsDecimalFromString(cprNo);

                    DataRow[] rowsForThisPerson = dt.Select(_UpdateDetectionVariables.PnrColumnName + "=" + pnr.ToString());

                    int idColIdx = dt.Columns.IndexOf(_UpdateDetectionVariables.IdColumnName);

                    StringBuilder sb = new StringBuilder();
                    foreach (DataRow row in rowsForThisPerson)
                    {
                        if (ShouldIStop())
                            return;

                        sb.Append(row[idColIdx].ToString() + ",");
                    }

                    string delStmt = sb.ToString();

                    // Remove trailing delimiter
                    delStmt = delStmt.Substring(0, delStmt.Length - 1);

                    // Build delete statement to delete this persons rows from batch ()
                    delStmt = "DELETE FROM [" + DatabaseName + "].[" + _UpdateDetectionVariables.SchemaName + "].[" + _UpdateDetectionVariables.StagingTableName + "] WHERE [" + _UpdateDetectionVariables.IdColumnName + "] IN (" + delStmt + ")";

                    CRUDContext dbCtx = new CRUDContext(_UpdateDetectionVariables.ConnectionStringName, delStmt);

                    int rowCnt = dbCtx.ExecuteNonQuery();
                }
            }
        }

        // -----------------------------------------------------------------------------
        private void GetBatchFromDB()
        {
            string sqlStmt = "SELECT [" + _UpdateDetectionVariables.IdColumnName + "], [" + _UpdateDetectionVariables.PnrColumnName + "], [" + _UpdateDetectionVariables.TableColumnName + "], [" + _UpdateDetectionVariables.TimestampColumnName + "] FROM [" + DatabaseName + "].[" + _UpdateDetectionVariables.SchemaName + "].[" + _UpdateDetectionVariables.StagingTableName + "]";

            CRUDContext dbCtx = new CRUDContext(_UpdateDetectionVariables.ConnectionStringName, sqlStmt);
            dsUpdatedCPRs = dbCtx.ExecuteDataSet();
        }

        // -----------------------------------------------------------------------------
        private string GetCprNoAsStringFromDecimal(decimal cprAsDecimal)
        {
            string cprno = "";

            cprno = cprAsDecimal.ToString();
            if (cprno.Length == 9)
            {
                cprno = "0" + cprno;
            }

            return cprno;
        }

        // -----------------------------------------------------------------------------
        private decimal GetCprNoAsDecimalFromString(string cprAsString)
        {
            decimal cprno = 0;

            cprno = Convert.ToDecimal(cprAsString);

            return cprno;
        }

        // -----------------------------------------------------------------------------
        private bool ShouldIStop()
        {
            return (_shouldIStop != null ? _shouldIStop() : false);
        }
    }
}
