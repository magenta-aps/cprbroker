using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace IndividualResponse.Attributes
    {
        [TestFixture]
        public class ToBirthDate
        {
            [Test]
            [ExpectedException]
            public void ToBirthDate_NullInformation_Exception()
            {
                var info = new IndividualResponseType();
                info.ToBirthDate();
            }

            [Test]
            public void ToBirthDate_Birthdate_OK()
            {
                var info = new IndividualResponseType() { PersonInformation = new PersonInformationType() { Birthdate = DateTime.Today, BirthdateUncertainty = ' ' } };
                var ret = info.ToBirthDate();
                Assert.AreEqual(DateTime.Today, ret);
            }

            [Test]
            public void ToBirthDate_NoBirthdate_FromPnr()
            {
                var info = new IndividualResponseType() 
                { 
                    PersonInformation = new PersonInformationType() 
                    { 
                      PNR = DateTime.Today.ToString("ddMMyy4111")
                    } 
                };
                var ret = info.ToBirthDate();
                Assert.AreEqual(DateTime.Today, ret);
            }
        }
    }
}