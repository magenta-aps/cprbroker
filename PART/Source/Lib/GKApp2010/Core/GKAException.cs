//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

using System;

namespace GKApp2010.Core
{
    // ================================================================================
    public class GKAException : Exception
    {
        // -----------------------------------------------------------------------------
        public GKAException()
        {
        }

        // -----------------------------------------------------------------------------
        public GKAException(string message)
            : base(message)
        {
        }

        // -----------------------------------------------------------------------------
        public GKAException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    // ================================================================================
    public class GKAConfigException : GKAException
    {
        // -----------------------------------------------------------------------------
        public GKAConfigException()
        {
        }

        // -----------------------------------------------------------------------------
        public GKAConfigException(string message)
            : base(message)
        {
        }

        // -----------------------------------------------------------------------------
        public GKAConfigException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    // ================================================================================
    public class GKAInvalidFormatException : GKAException
    {
        // -----------------------------------------------------------------------------
        public GKAInvalidFormatException()
        {
        }

        // -----------------------------------------------------------------------------
        public GKAInvalidFormatException(string message)
            : base(message)
        {
        }

        // -----------------------------------------------------------------------------
        public GKAInvalidFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    // ================================================================================
    public class GKAApplicationException : GKAException
    {
        bool _terminateAppl = false;

        // -----------------------------------------------------------------------------
        public GKAApplicationException()
        {
        }

        // -----------------------------------------------------------------------------
        public GKAApplicationException(string message)
            : base(message)
        {
        }

        // -----------------------------------------------------------------------------
        public GKAApplicationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        // -----------------------------------------------------------------------------
        public bool TerminateApplication
        {
            set { _terminateAppl = value; }
            get { return _terminateAppl; }
        }
    }

    // ================================================================================
    public static class ExceptionMessageBuilder
    {
        // -----------------------------------------------------------------------------
        public static string Build(string srcMessage, Exception ex)
        {
            string message = "";
            int nestLevel = 0;

            if (srcMessage == null) srcMessage = "";
            srcMessage = srcMessage.Trim();

            if (srcMessage.Length == 0)
            {
                message = "Exception was thrown. Message=[]";
            }
            else
            {
                message = srcMessage + " Message=";
            }

            while (ex != null)
            {
                message += "[(" + (nestLevel++).ToString() + "):" + ex.Message + "]\n";
                ex = ex.InnerException;
            }

            return message;
        }
    }
}
