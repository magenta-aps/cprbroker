using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Installers.CprBrokerInstallers
{
    public class CprBrokerWebSiteInstaller : CprBroker.Installers.WebSiteInstaller
    {
        protected override string DefaultWebsiteName
        {
            get
            {
                return "CprBroker";
            }
        }
    }
}
