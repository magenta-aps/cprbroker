using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Configuration.Install;

namespace CprBroker.Installers
{
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {


        }

        public ProjectInstaller(IContainer container)
        {
            container.Add(this);
        }
        
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            return;
            foreach (var inst in Installers)
            {
                if (inst is ICprInstaller)
                {
                    (inst as Installer).Context = Context;
                    var cprInstaller = inst as ICprInstaller;
                    cprInstaller.GetInstallInfoFromUser(stateSaver);
                }
            }
            foreach (var inst in Installers)
            {
                (inst as Installer).Install(stateSaver);
            }
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            foreach (var inst in Installers)
            {
                if (inst is ICprInstaller)
                {
                    var cprInstaller = inst as ICprInstaller;
                    cprInstaller.GetUnInstallInfoFromUser(savedState);
                }
            }
        }
    }
}
