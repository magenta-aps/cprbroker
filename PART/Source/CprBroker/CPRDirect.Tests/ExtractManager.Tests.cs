using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CPRDirect;
using NUnit.Framework;
using System.IO;

namespace CprBroker.Tests.CPRDirect
{
    namespace ExtractManagerTests
    {
        [TestFixture]
        public class ImportText
        {
            [Test]
            public void ImportText_Normal_OK()
            {
                ExtractManager.ImportText(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE_FixedLength);
            }
        }
    }
}
