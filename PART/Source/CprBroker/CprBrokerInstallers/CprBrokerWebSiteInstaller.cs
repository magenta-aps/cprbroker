using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Installers.CprBrokerInstallers
{
    /// <summary>
    /// Installs Cpr broker website
    /// </summary>
    public class CprBrokerWebSiteInstaller : CprBroker.Installers.WebSiteInstaller
    {
        protected override string DefaultWebsiteName
        {
            get
            {
                return "CprBroker";
            }
        }

        protected override string EventLogSourceName
        {
            get
            {
                return "Cpr Broker";
            }
        }
    }
}
