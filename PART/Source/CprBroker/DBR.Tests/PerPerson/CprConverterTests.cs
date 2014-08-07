using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.DBR;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.DBR.PerPerson
{
    [TestFixture]
    public class CprConverterTests : PersonBaseTest
    {
        [Test]
        public void AppendPerson_IndividualResponse_Passes([ValueSource(typeof(PersonBaseTest), "CprNumbers")]string pnr)
        {
            var pers = Persons[pnr];
            using (var dataContext = new CprBroker.Providers.DPR.DPRDataContext(""))
            {
                CprConverter.AppendPerson(pers, dataContext);
            }
        }
    }
}
