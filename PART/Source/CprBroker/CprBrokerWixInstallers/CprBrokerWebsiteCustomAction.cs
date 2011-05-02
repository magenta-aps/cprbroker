using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using CprBroker.Installers.Installers;
using System.DirectoryServices;

namespace CprBrokerWixInstallers
{
    public class CprBrokerWebsiteCustomAction
    {
        [CustomAction]
        public static ActionResult PopulateWebSites(Session session)
        {
            return WebsiteCustomAction.PopulateWebSites(session);
        }

        [CustomAction]
        public static ActionResult CreateCprBrokerWebsite(Session session)
        {
            session["CCCC"] = "CCCC";
            //return ActionResult.Success;
            return WebsiteCustomAction.DeployWebsite(session);
        }

    }
}
