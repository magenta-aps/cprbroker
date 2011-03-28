using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace CprBrokerWixInstallers
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult CreateDatabase(Session session)
        {
            session.Log("Begin Create database");
            System.Diagnostics.Debugger.Break();            
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult DeleteDatabase(Session session)
        {
            return ActionResult.Success;
        }
    }
}
