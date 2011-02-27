using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NUnitTester
{
    [NUnit.Framework.TestFixture]
    public class TestAdmin
    {
        [NUnit.Framework.Test]
        public void RequestAppRegisteration()
        {
            CPRAdministrationWS.CPRAdministrationWS adminWS = new NUnitTester.CPRAdministrationWS.CPRAdministrationWS();
            CPRAdministrationWS.ApplicationRegistrationStructureType codeType = adminWS.RequestAppRegisteration("", "APPLICATION1");
            Assert.IsNotNull(codeType);
        }

        [NUnit.Framework.Test]
        public void ApproveAppRegisteration()
        {
            CPRAdministrationWS.CPRAdministrationWS adminWS = new NUnitTester.CPRAdministrationWS.CPRAdministrationWS();
            bool codeType = adminWS.ApproveAppRegisteration("1DE09BE5-8382-4DC1-A89B-8F86FC67F97A");
            Assert.IsTrue(codeType);
        }

        [NUnit.Framework.Test]
        public void ListAppRegisteration()
        {
            CPRAdministrationWS.CPRAdministrationWS adminWS = new NUnitTester.CPRAdministrationWS.CPRAdministrationWS();
            CPRAdministrationWS.ApplicationStructureTypeBusinessApplications[] list = adminWS.ListAppRegisteration("", "1DE09BE5-8382-4DC1-A89B-8F86FC67F97A");
            Assert.Greater(list.Count(), 0);
        }

        [NUnit.Framework.Test]
        public void UnregisterApp()
        {
            CPRAdministrationWS.CPRAdministrationWS adminWS = new NUnitTester.CPRAdministrationWS.CPRAdministrationWS();
            bool codeType = adminWS.UnregisterApp("", "1DE09BE5-8382-4DC1-A89B-8F86FC67F97A");
            Assert.IsTrue(codeType);
        }

    }
}
