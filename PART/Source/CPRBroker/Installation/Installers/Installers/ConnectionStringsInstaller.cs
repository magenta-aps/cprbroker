using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Configuration.Install;

namespace CprBroker.Installers
{
    public class ConnectionStringsInstaller : Installer
    {
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            foreach (var configFile in _Files)
            {
                string configFileName = configFile.FileName;
                foreach (var connectionString in configFile.ConnectionStrings)
                {
                    Engine.Util.Installation.SetConnectionStringInConfigFile(configFileName, connectionString.Name, connectionString.Value);
                }
                Engine.Util.Installation.SetApplicationSettingInConfigFile(configFileName, typeof(CprBroker.Config.Properties.Settings), "EncryptConnectionStrings", "True");
            }
        }

        private static List<File> _Files = new List<File>();

        public static void RegisterConnectionString(string fileName, string name, string value)
        {
            var file = _Files.FirstOrDefault(f => f.FileName == fileName);
            if (file == null)
            {
                file = new File() { FileName = fileName };
                _Files.Add(file);
            }

            var connectionString = file.ConnectionStrings.FirstOrDefault(cs => cs.Name == name);
            if (connectionString == null)
            {
                connectionString = new ConnectionString() { Name = name };
                file.ConnectionStrings.Add(connectionString);
            }
            connectionString.Value = value;
        }

        public class ConnectionString
        {
            public string Name;
            public string Value;
        }

        public class File
        {
            public string FileName;
            public List<ConnectionString> ConnectionStrings = new List<ConnectionString>();
        }
    }
}
