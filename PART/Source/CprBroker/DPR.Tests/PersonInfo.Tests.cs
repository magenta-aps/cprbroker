using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.DPR;
using CprBroker.Utilities;
using CprBroker.Utilities.ConsoleApps;

namespace CprBroker.Tests.DPR
{
    [TestFixture]
    class PersonInfoTests : ConsoleEnvironment
    {
        public PersonInfoTests(string[] args)
            : base(args)
        { }

        public static void Main(string[] args)
        {
            // OtherConnectionString = "Data Source=10.20.1.20;Database=DPR;User ID=sa;Password=Dlph10t"
            var test = new PersonInfoTests(args);
            test.Run();
        }

        [Test]
        [TestCaseSource("AllCprNumbers")]
        public void GetPersonInfo_Normal_EqualsPersonInfoExpression(decimal pnr)
        {
            using (var dataContext = new DPRDataContext(OtherConnectionString))
            {
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

                var simplifiedPersonInfo = PersonInfo.GetPersonInfo(dataContext, pnr);
                Assert.NotNull(simplifiedPersonInfo, "simplifiedPersonInfo");
                var simplifiedPersonRegistration = simplifiedPersonInfo.ToRegisteringType1(effectDate, func, dataContext);
                Assert.NotNull(simplifiedPersonRegistration, "simplifiedPersonRegistration");
                var simplifiedXml = CprBroker.Utilities.Strings.SerializeObject(simplifiedPersonRegistration);
                WriteObject(pnr.ToDecimalString(), simplifiedXml);

                var expressionPersonInfo = PersonInfo.PersonInfoExpression.Compile()(dataContext).Where(pi => pi.PersonTotal.PNR == pnr).FirstOrDefault();
                if (expressionPersonInfo != null)
                {
                    var expressionPersonRegistration = expressionPersonInfo.ToRegisteringType1(effectDate, func, dataContext);
                    Assert.NotNull(expressionPersonRegistration, "expressionPersonRegistration");
                    var expressionXml = CprBroker.Utilities.Strings.SerializeObject(expressionPersonRegistration);
                    WriteObject(pnr.ToDecimalString() + "-expr", expressionXml);
                    Assert.AreEqual(expressionXml, simplifiedXml);
                }

            }
        }

        public override void ProcessPerson(string pnr)
        {
            decimal decimalPnr = decimal.Parse(pnr);
            GetPersonInfo_Normal_EqualsPersonInfoExpression(decimalPnr);
        }

        public override string[] LoadCprNumbers()
        {
            using (var dataContext = new DPRDataContext(OtherConnectionString))
            {
                return dataContext.PersonTotals.OrderBy(pt => pt.PNR).Select(pt => pt.PNR.ToString()).ToArray();
            }
        }
    }
}
