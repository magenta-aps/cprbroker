using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;
using CprBroker.Schemas.Part;

namespace CprBroker.Tests.CPRDirect.Objects
{
    namespace EgenskabIntervalTests
    {
        [TestFixture]
        public class ToFoedestedNavn
        {
            [Test]
            public void ToFoedestedNavn_NameWithNoDateOrHistory_Null()
            {
                var pnr = Utilities.RandomCprNumberString();
                var res = new EgenskabInterval()
                {
                    //BasicInformation = new PersonInformationType() { PNR = pnr, Birthdate = CprBroker.Utilities.Strings.PersonNumberToDate(pnr) },
                    //HistoricalNames = new INameSource[0],
                    Data = new List<ITimedType>(new ITimedType[] { new CurrentNameInformationType() { PNR = pnr, FirstName_s = "1", LastName = "2" } })
                };
                var ret = res.ToFoedestedNavn();
                Assert.IsNullOrEmpty(ret);
            }

            [Test]
            public void ToFoedestedNavn_NameWithDate_Null(
                [Values(0, 1, 2, 10, 14)]int dayOffset)
            {
                string pnr = Utilities.RandomCprNumberString();
                DateTime birthDate = PartInterface.Strings.PersonNumberToDate(pnr).Value;
                DateTime nameDate = birthDate.AddDays(dayOffset);
                var res = new EgenskabInterval()
                {
                    //BasicInformation = new PersonInformationType() { PNR = pnr, Birthdate = birthDate },
                    //HistoricalNames = new INameSource[0],
                    Data = new List<ITimedType>(new ITimedType[] { new CurrentNameInformationType() { PNR = pnr, FirstName_s = "1", LastName = "2", NameStartDate = nameDate } })
                };

                var ret = res.ToFoedestedNavn();
                Assert.IsNullOrEmpty(ret);
            }

            [Test]
            public void ToFoedestedNavn_NameWithFarDate_Null(
                [Values(15, 20, 30)]int dayOffset)
            {
                string pnr = Utilities.RandomCprNumberString();
                DateTime birthDate = PartInterface.Strings.PersonNumberToDate(pnr).Value;
                DateTime nameDate = birthDate.AddDays(dayOffset);
                var res = new EgenskabInterval()
                {
                    //BasicInformation = new PersonInformationType() { PNR = pnr, Birthdate = birthDate },
                    //HistoricalNames = new INameSource[0],
                    Data = new List<ITimedType>(new ITimedType[] { new CurrentNameInformationType() { PNR = pnr, FirstName_s = "1", LastName = "2", NameStartDate = nameDate } })
                };

                var ret = res.ToFoedestedNavn();
                Assert.IsNullOrEmpty(ret);
            }
        }
    }
}
