using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace ClearWrittenAddressTests
    {
        [TestFixture]
        public class Misc
        {
            [Test]
            public void CheckAddresses()
            {
                System.Diagnostics.Debugger.Launch();
                var result = IndividualResponseType.ParseBatch(Properties.Resources.U12170_P_opgavenr_110901_ADRNVN_FE);
                var groupings = result.GroupBy(
                    person => new
                    {
                        //HasClearAddress = person.ClearWrittenAddress != null,
                        ValidClearAddress = !person.ClearWrittenAddress.IsEmpty,    // { related to each other }
                        CurrentAddress = person.CurrentAddressInformation != null,  // { related to each other }
                        ContactAddress = person.ContactAddress != null,
                        ForeignAddress = person.CurrentDepartureData != null,
                        ValidForeignAddress = person.CurrentDepartureData != null && !person.CurrentDepartureData.IsEmpty,
                        Status = person.PersonInformation.Status
                    })
                    .Where(person => !person.Key.ValidClearAddress && !person.Key.ValidForeignAddress)
                    .OrderBy(g => g.Key.Status)
                    .Select(g => new { Key = g.Key, Value = g.ToArray() })
                    .ToArray();


                object o = "";
            }
        }
    }
}
