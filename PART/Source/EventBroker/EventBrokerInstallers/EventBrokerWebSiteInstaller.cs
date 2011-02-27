using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Installers.EventBrokerInstallers
{
    public class EventBrokerWebSiteInstaller : WebSiteInstaller
    {
        protected override string DefaultWebsiteName
        {
            get
            {
                return "EventBroker";
            }
        }
    }
}
