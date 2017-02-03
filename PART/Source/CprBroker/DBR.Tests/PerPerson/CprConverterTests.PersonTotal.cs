using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.DBR;
using CprBroker.Schemas.Part;
using CprBroker.Tests.CPRDirect;
using CprBroker.DBR.Extensions;

namespace CprBroker.Tests.DBR.PerPerson
{
    namespace PersonTotalConverterTests
    {
        [TestFixture]
        public class ToPreviousAddressString : DbrTestBase
        {
            [Test]
            public void ToPreviousAddressString_IndividualResponse_Passes([ValueSource(typeof(CPRDirect.Utilities), nameof(CPRDirect.Utilities.PNRs))]string pnr)
            {
                var pers = CPRDirect.Utilities.PersonIndividualResponses[pnr];

                using (var dataContext = new CprBroker.Providers.DPR.DPRDataContext(DbrDatabase.ConnectionString))
                {
                    var ret = pers.ToPreviousAddressString(dataContext);
                }
            }

            [Test]
            public void ToPreviousAddressString_IndividualResponse_NotEmpty([ValueSource(typeof(CPRDirect.Utilities), nameof(CPRDirect.Utilities.PNRs))]string pnr)
            {
                var pers = CPRDirect.Utilities.PersonIndividualResponses[pnr];

                using (var dataContext = new CprBroker.Providers.DPR.DPRDataContext(DbrDatabase.ConnectionString))
                {
                    var ret = pers.ToPreviousAddressString(dataContext);
                    if (string.IsNullOrEmpty(ret))
                    {
                        if (pers.PersonInformation.Status == 90)
                            Assert.IsNotNullOrEmpty(ret); // dead must have a previous address
                        else
                        {
                            var lst = new List<IHasCorrectionMarker>();
                            lst.AddRange(pers.HistoricalAddress);
                            lst.AddRange(pers.HistoricalDeparture);
                            lst.AddRange(pers.HistoricalDisappearance);
                            if (lst.Where(a => a.IsOk()).Count() > 0)
                            {
                                Assert.IsNotNullOrEmpty(ret);
                            }
                        }
                    }
                }
            }
        }
    }
}
