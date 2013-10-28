using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using CprBroker.Utilities;

namespace CprBrokerWixInstallers
{
    public static class EventBrokerCustomActions
    {
        [CustomAction]
        public static ActionResult CreateEventBrokerWebsite(Session session)
        {
            System.Windows.Forms.MessageBox.Show("Installing event broker website");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RollbackEventBrokerWebsite(Session session)
        {
            System.Windows.Forms.MessageBox.Show("Rollingback event broker website");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveEventBrokerWebsite(Session session)
        {
            System.Windows.Forms.MessageBox.Show("Removing event broker website");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult DeployEventBrokerDatabase(Session session)
        {
            System.Windows.Forms.MessageBox.Show("Installing event broker database");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RollbackEventBrokerDatabase(Session session)
        {
            System.Windows.Forms.MessageBox.Show("Rolling back event broker database");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveEventBrokerDatabase(Session session)
        {
            System.Windows.Forms.MessageBox.Show("Removing event broker database");
            return ActionResult.Success;
        }
    }
}
