using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CprBrokerWixInstallers;
using CprBroker.Installers;
using Microsoft.Deployment.WindowsInstaller;
using NUnit.Framework;

namespace CprBroker.Tests.InstallerActions
{
    namespace CprBrokerCustomActionTests
    {
        public class PatchWebsite_2_2_7
        {
            [Test]
            public void PatchWebsite_2_2_7_Passes()
            {
                var session = new SessionAdapterStub();
                CprBrokerCustomActions.PatchWebsite_2_2_7(session);
            }
        }
    }
}
