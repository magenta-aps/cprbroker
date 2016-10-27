using CprBroker.DBR;
using CprBroker.Providers.CPRDirect;
using CprBroker.Providers.DPR;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CprBroker.Tests.DBR.Comparison.Person
{
    [TestFixture]
    [Ignore]
    public class _PersonConversion : PersonComparisonTest<object>
    {
        public void ConvertObject(string pnr)
        {
            using (var fakeDprDataContext = new DPRDataContext(Properties.Settings.Default.ImitatedDprConnectionString))
            {
                DatabaseLoadCache.Root.Reset(fakeDprDataContext);

                CprConverter.DeletePersonRecords(pnr, fakeDprDataContext);
                fakeDprDataContext.SubmitChanges();
            }
            using (var fakeDprDataContext = new DPRDataContext(Properties.Settings.Default.ImitatedDprConnectionString))
            {
                var person = ExtractManager.GetPerson(pnr);
                CprConverter.AppendPerson(person, fakeDprDataContext);
                fakeDprDataContext.SubmitChanges();
            }
            KeysHolder._ConvertedPersons[pnr] = true;
        }

        [Test]
        [TestCaseSource("LoadKeys")]
        public void T0_Convert(string key)
        {
            ConvertObject(key);
        }

        public override IQueryable<object> Get(DPRDataContext dataContext, string key)
        {
            return new object[] { }.AsQueryable();
        }

    }
}
