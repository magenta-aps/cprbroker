//------------------------------------------------------------------------------
// DPR Updates Code Library
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using GKApp2010.DB;
using GKApp2010.Threads;

namespace DPRUpdateLib
{
    // ================================================================================
    public class DPRUpdatedStagingBatch
    {
        IsStopRequestedFunc _shouldIStop = null;
        DataSet dsUpdatedCPRs = null;

        // -----------------------------------------------------------------------------
        public DPRUpdatedStagingBatch(IsStopRequestedFunc shouldIStop)
        {
            _shouldIStop = shouldIStop;

            GetBatchFromDB();
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
                    int pnrColIdx = dt.Columns.IndexOf("PNR");

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

                    DataRow[] rowsForThisPerson = dt.Select("PNR=" + pnr.ToString());

                    int idColIdx = dt.Columns.IndexOf("Id");

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
                    delStmt = "DELETE FROM [DPR].[dbo].[T_DPRUpdateStaging] WHERE [Id] IN (" + delStmt + ")";

                    CRUDContext dbCtx = new CRUDContext("DPRUpdates", delStmt);

                    int rowCnt = dbCtx.ExecuteNonQuery();
                }
            }
        }

        // -----------------------------------------------------------------------------
        private void GetBatchFromDB()
        {
            string sqlStmt = "SELECT [Id], [PNR], [DPRTable], [CreateTS] FROM [DPR].[dbo].[T_DPRUpdateStaging]";

            CRUDContext dbCtx = new CRUDContext("DPRUpdates", sqlStmt);
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
