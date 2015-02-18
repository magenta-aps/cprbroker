using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.DBR.PerPerson
{
    public class PersonBaseTest 
    {
        public static string[] CprNumbers;
        public static Dictionary<string, IndividualResponseType> Persons;


        static PersonBaseTest()
        {
            var all = CprBroker.Providers.CPRDirect.IndividualResponseType.ParseBatch(CprBroker.Tests.CPRDirect.Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
            CprNumbers = all.Select(p => p.PersonInformation.PNR).OrderBy(p => p).ToArray();
            Persons = all.ToDictionary(p => p.PersonInformation.PNR);
        }

    }
}
