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
            session.Log("DDDDDDDDDDDD" + session.CustomActionData.Keys.Count.ToString());
            session.Log(string.Join(",",session.CustomActionData.Keys.ToArray()));
            session.Message(InstallMessage.Warning, new Record(session.CustomActionData.Count.ToString()));
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
