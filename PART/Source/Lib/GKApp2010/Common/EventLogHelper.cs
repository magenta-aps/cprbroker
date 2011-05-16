//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

using System.Diagnostics;

using GKApp2010.RTE;

namespace GKApp2010.Common
{
    // ================================================================================
    public static class EventLogHelper
    {
        // -----------------------------------------------------------------------------
        public static void WriteEntry(string message)
        {
            EventLog.WriteEntry(GetEventSource(), message);
        }

        // -----------------------------------------------------------------------------
        public static void WriteWarningEntry(string message)
        {
            EventLog.WriteEntry(GetEventSource(), message, EventLogEntryType.Warning);
        }

        // -----------------------------------------------------------------------------
        public static void WriteErrorEntry(string message)
        {
            EventLog.WriteEntry(GetEventSource(), message, EventLogEntryType.Error);
        }

        // -----------------------------------------------------------------------------
        private static string GetEventSource()
        {
            return Info.GetApplicationName();
        }
    }

}
