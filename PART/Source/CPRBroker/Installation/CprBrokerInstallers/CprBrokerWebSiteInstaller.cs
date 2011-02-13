using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Installers.CprBrokerInstallers
{
    [System.ComponentModel.RunInstaller(true)]
    public class CprBrokerWebSiteInstaller : Installers.WebSiteInstaller
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
