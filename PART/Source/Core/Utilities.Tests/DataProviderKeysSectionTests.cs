using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CprBroker.Utilities.Config;
using System.IO;
using System.Configuration;

namespace CprBroker.Tests.Utilities
{
    namespace DataProviderKeysSectionTests
    {
        [TestFixture]
        public class RegisterNewKeys
        {
            [Test]
            public void RegisterNewKeys_Normal_Passes()
            {
                var folder = CprBroker.Utilities.Strings.NewUniquePath(@".\Temp\", "");
                Directory.CreateDirectory(folder);
                var path = CprBroker.Utilities.Strings.NewUniquePath(folder, "config");
                File.WriteAllText(path, Properties.Resources.App_config);

                var map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = path;
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

                DataProviderKeysSection.RegisterNewKeys(configuration);
            }
        }
    }
}
