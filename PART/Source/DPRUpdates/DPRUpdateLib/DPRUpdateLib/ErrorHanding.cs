//------------------------------------------------------------------------------
// DPR Updates Code Library
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GKApp2010.Core;

namespace DPRUpdateLib
{
    public class ErrorHanding
    {
    }

    // ================================================================================
    public class DPRUpdBrokerRequestException : GKAException
    {
        // -----------------------------------------------------------------------------
        public DPRUpdBrokerRequestException()
        {
        }

        // -----------------------------------------------------------------------------
        public DPRUpdBrokerRequestException(string message)
            : base(message)
        {
        }

        // -----------------------------------------------------------------------------
        public DPRUpdBrokerRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
