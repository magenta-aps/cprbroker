//------------------------------------------------------------------------------
// Gentofte Kommune DPR update notification service.
// Copyright (c) Gentofte Kommune 2011.
// This source is subject to the Mozilla Public License 1.1 terms. See http://www.opensource.org/licenses/mozilla1.1.
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;

using GKApp2010.RTE;

namespace DPRUpdatesNotification
{
    // ================================================================================
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        // -----------------------------------------------------------------------------
        public ProjectInstaller()
        {
            InitializeComponent();

            string s = "";

            s += "Collect changes made to persons in the DPR database and notify target systems such as CPRBroker of these changes. ";
            s += "In this version of the service, collection occurs at predefined intervals (polling the DB). Poll interval is defined in configuration. ";
            s += "Version=[" + Info.GetRuntimeVersion() + "]. ";
            s += "20110329/SDE";

            serviceInstaller1.Description = s;

        }
    }
}
