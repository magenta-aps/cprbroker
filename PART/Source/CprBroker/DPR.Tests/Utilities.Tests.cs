using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CprBroker.Schemas;
using CprBroker.Schemas.Part;
using NUnit.Framework;
using CprBroker.Providers.DPR;

namespace CprBroker.Tests.DPR.UtilitiesTests
{
    [TestFixture]
    public class GetParentPnr : BaseTests
    {
        [Test]
        public void GetParentPnr_ValidValues_OK(
            [ValueSource("RandomCprNumbers5")]decimal parentPnr)
        {
            var result = CprBroker.Providers.DPR.Utilities.ToParentPnr(parentPnr.ToDecimalString());
            Assert.AreEqual(parentPnr, result);
        }

        [Test]
        public void GetParentPnr_ValidValues9Digits_OK(
            [ValueSource("Random9DigitCprNumbers5")]decimal parentPnr)
        {
            var result = CprBroker.Providers.DPR.Utilities.ToParentPnr(parentPnr.ToDecimalString());
            Assert.AreEqual(parentPnr, result);
        }

        [Test]
        public void GetParentPnr_Zero_Null(
            [Values("000000-0000")]string parentPnrString)
        {
            var result = CprBroker.Providers.DPR.Utilities.ToParentPnr(parentPnrString);
            Assert.Null(result);
        }

        [Test]
        public void GetParentPnr_InValidValues_Null(
            [ValueSource("RandomDecimals5")]decimal parentPnr)
        {
            var result = CprBroker.Providers.DPR.Utilities.ToParentPnr(parentPnr.ToDecimalString());
            Assert.Null(result);
        }

        [Test]
        public void GetParentPnr_Date_Null(
            [ValueSource("RandomDecimalDates5")]decimal parentDecimalBirthdate,
            [Values("DD MM YYYY", "DD MM YY", "MM YYYY", "MM YY", "DD MM")] string dateFormat)
        {
            DateTime parentBirthdate = CprBroker.Providers.DPR.Utilities.DateFromDecimal(parentDecimalBirthdate).Value;
            var parentStringDate = parentBirthdate.ToString(dateFormat);
            var result = CprBroker.Providers.DPR.Utilities.ToParentPnr(parentStringDate);
            Assert.Null(result);
        }
    }
}
