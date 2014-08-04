using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBR.Tests
{
    public class DbrTestBase : CprBroker.Tests.PartInterface.TestBase
    {
        public DatabaseInfo DbrDatabase;
        public override void CreateDatabases()
        {
            base.CreateDatabases();
            DbrDatabase = CreateDatabase("DBR_", Properties.Resources.CreateDbr, new KeyValuePair<string, string>[] { });
        }

    }
}
