//------------------------------------------------------------------------------
// Gentofte Kommune DPR update notification service.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace DPRUpdatesNotification
{
    // ================================================================================
    static class Program
    {
        // -----------------------------------------------------------------------------
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new DPRUpdatesNotificationService() 
			};

            ServiceBase.Run(ServicesToRun);
        }
    }
}
