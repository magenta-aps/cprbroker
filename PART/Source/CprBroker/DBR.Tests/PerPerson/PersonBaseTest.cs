using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DBR.Tests.PerPerson
{
    public class PersonBaseTest : DbrTestBase
    {
        public string[] CprNumbers;


        public PersonBaseTest()
        {
            var all = CprBroker.Providers.CPRDirect.IndividualResponseType.ParseBatch(CprBroker.Tests.CPRDirect.Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
            CprNumbers = all.Select(p => p.PersonInformation.PNR).OrderBy(p => p).ToArray();
        }

    }
}
