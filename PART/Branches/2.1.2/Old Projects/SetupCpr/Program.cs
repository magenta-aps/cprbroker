using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPRBroker.SetupCpr
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            //Application.Run(new  DatabaseForm());
            System.Collections.Hashtable parameters = new System.Collections.Hashtable();
            
            SetupCpr.WebSiteInstaller inst=new WebSiteInstaller();
            inst.Context = new System.Configuration.Install.InstallContext();
            inst.Context.Parameters["assemblypath"] = System.Reflection.Assembly.GetExecutingAssembly().Location;
            inst.Install(parameters);
        }
    }
}
