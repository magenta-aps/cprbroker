using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.IO;

namespace CprBroker.Tests.CustomActions
{
    namespace CprBrokerCustomActionTests
    {
        [TestFixture]
        public class AddMvcElements
        {
            [Test]
            public void AddMvcElements_Run_CorrectTargetContents()
            {
                var createdPath = "created.xml";
                File.WriteAllText(createdPath, Properties.Resources.OldWebConfig);

                CprBrokerWixInstallers.CprBrokerCustomActions.AddMvcElements(createdPath);

                var created = File.ReadAllText(createdPath);
                Assert.AreEqual(Properties.Resources.NewWebConfig, created);
            }
        }
    }
}
