using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using CprBroker.Installers;

namespace InstallerLib
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult TestDatabaseConnection(Session session)
        {
            return DatabaseCustomAction.TestConnectionString(session);

            return ActionResult.Success;
        }
    }
}
