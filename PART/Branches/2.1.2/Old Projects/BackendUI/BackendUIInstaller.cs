using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CprBroker.Engine.Util;

namespace CPRBroker.BackendUI
{
    [RunInstaller(true)]
    public partial class BackendUIInstaller : System.Configuration.Install.Installer
    {
        public BackendUIInstaller()
        {
            InitializeComponent();
        }

        public BackendUIInstaller(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
            this.SetConnectionStringInConfigFile(this.GetAssemblyConfigFilePath(), this.GetConnectionStringFromWebConfig());
        }

    }
}
