//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace GKApp2010.Common
{
    // ================================================================================
    public class LogHelper
    {
        // -----------------------------------------------------------------------------
        public static void LogToFile(string message)
        {
            //string logFilename = GKApp2010.Config.Default.GetLogfilename();
            
            LogEntry logEntry = new LogEntry();
            logEntry.Message = message;
            Logger.Write(logEntry);
        }
    }


}
