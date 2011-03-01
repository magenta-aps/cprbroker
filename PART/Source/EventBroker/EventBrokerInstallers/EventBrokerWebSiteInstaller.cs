using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Installers.EventBrokerInstallers
{
    /// <summary>
    /// Installs the Event Broker website
    /// </summary>
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
