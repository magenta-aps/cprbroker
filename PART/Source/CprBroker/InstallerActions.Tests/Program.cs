using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.InstallerActions
{
    class Program
    {
        public static void Main()
        {
            new CprBrokerCustomActionTests.PatchWebsite_2_2_7().PatchWebsite_2_2_7_Passes();
        }
    }
}
