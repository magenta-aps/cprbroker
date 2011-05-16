//------------------------------------------------------------------------------
// Gentofte Kommune application framework 2011.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using GKApp2010.RTE;

namespace GKApp2010
{
    // ================================================================================
    public class GKACoreService : IGKACoreService
    {
        // -----------------------------------------------------------------------------
        //[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public string GetServerRuntimeInfo(string context, ref string aux)
        {
            return Info.GetRuntimeInfo();
        }

        // -----------------------------------------------------------------------------
        //[OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
        public string GetServerRuntimeContext(string context, ref string aux)
        {
            return Info.GetRuntimeContext();
        }
    }
}
