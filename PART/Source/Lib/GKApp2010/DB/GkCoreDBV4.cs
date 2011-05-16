//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

using System.Data;

namespace GKApp2010.DB
{
    // ================================================================================
    public class GkCoreDBV4
    {
        // -----------------------------------------------------------------------------
        public static void LogAuditEntry(string context, string procName, int eventID, string auditText)
        {
            //CREATE PROCEDURE spGK_CORE_LogAuditEntryExt
            //    @context            VARCHAR(1020),
            //    @procname           VARCHAR(120),
            //    @eventID            INTEGER,
            //    @logText            VARCHAR(2000),
            //    @aux                VARCHAR(1020) OUTPUT

            string aux = "";

            // Init params
            StoredProcedureCallContext.ClearStandardParams(ref context, ref aux);

            StoredProcedureCallContext spContext = new StoredProcedureCallContext("GkCoreDBV4", "spGK_CORE_LogAuditEntryExt");

            spContext.AddInParameter("procname", DbType.String, procName.Trim());
            spContext.AddInParameter("eventID", DbType.Int32, eventID);
            spContext.AddInParameter("logText", DbType.String, auditText.Trim());

            spContext.ExecuteNonQueryWithReturnValue();

        }
    }
}
