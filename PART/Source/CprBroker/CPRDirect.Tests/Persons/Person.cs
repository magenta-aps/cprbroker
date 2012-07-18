using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect.Persons
{
    public abstract class Person
    {
        public string GetPNR()
        {
            return this.GetType().Name.Substring(2, 10);
        }

        public string GetData()
        {
            var t = typeof(Properties.Resources);
            var ret = t.InvokeMember(
                string.Format("PNR_3112970079",GetPNR()),
                 System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                 null,
                 null,
                 new object[0]
                 );
            return ret.ToString();
        }

        public IndividualResponseType GetPerson()
        {
            var all = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
            return all.Where(p => p.PersonInformation.PNR == GetPNR()).First();
        }
    }
}
