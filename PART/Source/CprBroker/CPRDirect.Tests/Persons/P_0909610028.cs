using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect.Persons
{
    [TestFixture]
    public class P_0909610028 : Person
    {
        [Test]
        public void ToWrapper()
        {
            //var txt = System.IO.File.ReadAllText(@"C:\Magenta Workspace\PART\Doc\Data Providers\CPR Direct\Test data\U12170-P opgavenr 110901 ADRNVN FE");
            //var txt = System.IO.File.ReadAllText(@"C:\Magenta Workspace\PART\Source\CprBroker\CPRDirect.Tests\Resources\Test\PNR_0909610028.txt", Constants.DefaultEncoding);
            var txt = Properties.Resources.PNR_0909610028;
            var lines = LineWrapper.ParseBatch(txt);

            var line = lines.Where(l => l.Code == "017" && l.PNR == this.GetPNR()).First();
            System.Diagnostics.Debugger.Launch();
            var w = line.ToWrapper(Constants.DataObjectMap);
            var ss = "";
        }

        
    }
}
