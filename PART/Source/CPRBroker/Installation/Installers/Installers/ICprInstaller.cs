using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CprBroker.Installers
{
    public interface ICprInstaller
    {
        void GetInstallInfoFromUser();
        void GetUnInstallInfoFromUser();
    }
}
