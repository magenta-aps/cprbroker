using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;
using CPRBroker.Engine.Util;

namespace ImpersonationAction
{
    [RunInstaller(true)]
    public class MyInstaller:Installer
    {
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);            
            string token = System.Security.Principal.WindowsIdentity.GetCurrent().Token.ToString();
            Context.Parameters["userToken222"] = token;
            stateSaver["userToken222"] = token;
            string fileName = this.GetAssemblyFolderPath() + "\\a.txt";
            System.IO.File.WriteAllText(fileName, token);
        }
    }
}
