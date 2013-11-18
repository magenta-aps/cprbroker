using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Utilities;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Providers.DPR;

namespace BatchClient
{
    class TestDPRConversion : ConsoleEnvironment
    {
        public override string[] LoadCprNumbers()
        {
            using (var dataContext = new DPRDataContext(OtherConnectionString))
            {
                return dataContext.PersonTotals.Select(t => t.PNR).ToArray().Select(pnr=>pnr.ToPnrDecimalString()).ToArray();
            }
        }

        Dictionary<string, Guid> uuids = new Dictionary<string, Guid>();

        Guid cpr2uuidFunc(string s)
        {
            if (!uuids.ContainsKey(s)) uuids[s] = Guid.NewGuid();
            return uuids[s];
        }

        public override void ProcessPerson(string pnr)
        {
            using (var dataContext = new DPRDataContext(OtherConnectionString))
            {
                {
                    var db = PersonInfo.GetPersonInfo(dataContext, decimal.Parse(pnr));
                    var ret = db.ToRegisteringType1(cpr2uuidFunc, dataContext);
                }
            }
        }

    }
}
