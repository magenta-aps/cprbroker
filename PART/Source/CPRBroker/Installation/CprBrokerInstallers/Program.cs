using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CprBroker.Installers.CprBrokerInstallers
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*CprBrokerInstaller inst = new CprBrokerInstaller();
            inst.Context = new System.Configuration.Install.InstallContext();
            System.Collections.Hashtable stateSaver = new System.Collections.Hashtable();
            inst.Context.Parameters["assemblypath"] = AppDomain.CurrentDomain.BaseDirectory + "CprBroker.Installers.CprBrokerInstallers.exe";
            inst.Install(stateSaver);*/
        }
    }
}
