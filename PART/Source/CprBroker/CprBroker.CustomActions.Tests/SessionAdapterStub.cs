using CprBroker.Installers;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.InstallerActions
{
    public class SessionAdapterStub : SessionAdapter
    {
        public SessionAdapterStub()
        {
            var cprInfo = new WebsiteInstallationInfo() { FeatureName = "CPR", WebsiteName = "CPRBROKER" };
            var eventInfo = new WebsiteInstallationInfo() { FeatureName = "EVENT", WebsiteName = "EVENTBROKER" };
            this[PropertyNames.InstallDir] = AppDomain.CurrentDomain.BaseDirectory;

            WebInstallationInfo.AddFeatureDetails(this, cprInfo);
            WebInstallationInfo.AddFeatureDetails(this, eventInfo);

        }

        InstallRunMode _Mode = InstallRunMode.Admin;
        public override bool GetMode(InstallRunMode mode)
        {
            return mode == _Mode;
        }

        Dictionary<string, string> _This = new Dictionary<string, string>();
        public override string this[string property]
        {
            get
            {
                return _This.ContainsKey(property) ? _This[property] : null;
            }

            set
            {
                _This[property] = value;
            }
        }
    }
}
