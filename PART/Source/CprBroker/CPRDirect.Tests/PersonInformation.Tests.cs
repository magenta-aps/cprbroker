using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace PersonInformationTests
    {
        [TestFixture]
        public class ToBirthdate
        {
            [Test]
            public void ToBirthdate_Empty_Null()
            {
                var inf = new PersonInformationType();
                var ret = inf.ToBirthdate();
                Assert.Null(ret);
            }

            [Test]
            public void ToBirthdate_UncertainValue_Null(
                [Values('s', '2', '4', 'S')] char uncertainty)
            {
                var inf = new PersonInformationType() { Birthdate = DateTime.Today, BirthdateUncertainty = uncertainty };
                var ret = inf.ToBirthdate();
                Assert.Null(ret);
            }

            [Test]
            public void ToBirthdate_Value_OK()
            {
                var inf = new PersonInformationType() { Birthdate = DateTime.Today };
                var ret = inf.ToBirthdate();
                Assert.AreEqual(DateTime.Today, ret.Value);
            }
        }

        [TestFixture]
        public class ToStatusDate
        {
            [Test]
            public void ToStatusDate_Empty_Null()
            {
                var inf = new PersonInformationType();
                var ret = inf.ToStatusDate();
                Assert.Null(ret);
            }

            [Test]
            public void ToStatusDate_UncertainValue_Null(
                [Values('s', '2', '4', 'S')] char uncertainty)
            {
                var inf = new PersonInformationType() { StatusStartDate = DateTime.Today, StatusDateUncertainty = uncertainty };
                var ret = inf.ToStatusDate();
                Assert.Null(ret);
            }

            [Test]
            public void ToStatusDate_Value_OK()
            {
                var inf = new PersonInformationType() { StatusStartDate = DateTime.Today };
                var ret = inf.ToStatusDate();
                Assert.AreEqual(DateTime.Today, ret.Value);
            }
        }

        [TestFixture]
        public class ToPnr
        {
            [Test]
            public void ToPnr_Normal_OK(
                [Values("1234567890", "123456789")]string pnr)
            {
                var inf = new PersonInformationType() { PNR = pnr };
                var ret = inf.ToPnr();
                Assert.IsNotNullOrEmpty(ret);
            }

            [Test]
            public void ToPnr_Invalid_Emypty(
                [Values("12345678", "0", "224123", "12345", "123")]string pnr)
            {
                var inf = new PersonInformationType() { PNR = pnr };
                var ret = inf.ToPnr();
                Assert.IsNullOrEmpty(ret);
            }
        }

    }
}
