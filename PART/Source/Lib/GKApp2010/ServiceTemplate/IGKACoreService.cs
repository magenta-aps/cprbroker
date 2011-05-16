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

namespace GKApp2010
{
    // ================================================================================
    [ServiceContract(Namespace = "http://gkaservices.gentofte.dk/CoreOp/01")]
    public interface IGKACoreService
    {
        // -----------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="aux"></param>
        /// <returns></returns>
        [OperationContract]
        string GetServerRuntimeInfo(string context, ref string aux);

        // -----------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="aux"></param>
        /// <returns></returns>
        [OperationContract]
        string GetServerRuntimeContext(string context, ref string aux);
    }
}
