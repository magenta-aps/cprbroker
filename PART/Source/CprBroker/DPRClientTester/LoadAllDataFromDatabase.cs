using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using CprBroker.Utilities;
using CprBroker.Utilities.ConsoleApps;
using CprBroker.Providers.DPR;

namespace DPRClientTester
{
    class LoadAllDataFromDatabase : ConsoleEnvironment
    {
        public static void Main(string[] args)
        {
            var test = new LoadAllDataFromDatabase(args);
            test.Run();
        }

        public LoadAllDataFromDatabase(string[] args)
            : base(args)
        { }

        public override string[] LoadCprNumbers()
        {
            using (DPRDataContext dataContext = new DPRDataContext(OtherConnectionString))
            {
                return dataContext.PersonTotals.Select(t => t.PNR).ToArray().Select(p => p.ToPnrDecimalString()).ToArray();
            }
        }

        public override void ProcessPerson(string pnr)
        {
            var decimalPnr = decimal.Parse(pnr);
            using (DPRDataContext dataContext = new DPRDataContext(OtherConnectionString))
            {
                var expressionPersonInfo = PersonInfo.PersonInfoExpression.Compile()(dataContext).Where(pi => pi.PersonTotal.PNR == decimalPnr).FirstOrDefault();
                DateTime effectDate = DateTime.Now;
                // UUID mapping
                var map = new Dictionary<string, Guid>();
                Func<string, Guid> func = (string cpr) =>
                {
                    if (!map.ContainsKey(cpr))
                    {
                        map[cpr] = Guid.NewGuid();
                    }
                    return map[cpr];
                };
                var prov = new DprDatabaseDataProvider() { ConfigurationProperties = new Dictionary<string, string>() };
                prov.AlwaysReturnCprBorgerType = true;
                var xmlObj = expressionPersonInfo.ToRegisteringType1(effectDate, func, dataContext, prov);
                WriteObject(pnr, xmlObj);

            }
        }
    }
}
