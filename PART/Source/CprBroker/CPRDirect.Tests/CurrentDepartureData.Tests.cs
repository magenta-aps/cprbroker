using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CprBroker.Schemas.Part;
using CprBroker.Providers.CPRDirect;

namespace CprBroker.Tests.CPRDirect
{
    namespace CurrentDepartureDataTests
    {
        [TestFixture]
        public class IsEmpty
        {
            [Test]
            public void IsEmpty_Nothing_True()
            {
                var db = new CurrentDepartureDataType();
                var ret = db.IsEmpty;
                Assert.True(ret);
            }

            [Test]
            public void IsEmpty_Dates_True()
            {
                var db = new CurrentDepartureDataType() { ExitDate = DateTime.Today, PNR = "1234567890" };
                var ret = db.IsEmpty;
                Assert.True(ret);
            }

            [Test]
            public void IsEmpty_Lines_False()
            {
                var db = new CurrentDepartureDataType() { ForeignAddress1 = "DDD" };
                var ret = db.IsEmpty;
                Assert.False(ret);
            }
        }

        [TestFixture]
        public class ToVirkningTypeArray
        {
            [Test]
            public void ToVirkningTypeArray_Empty_Empty()
            {
                var db = new CurrentDepartureDataType();
                var ret = db.ToVirkningTypeArray();
                var v = VirkningType.Compose(ret);
                Assert.True(VirkningType.IsDoubleOpen(v));
            }

            [Test]
            public void ToVirkningTypeArray_Date_StartNotEmpty()
            {
                var db = new CurrentDepartureDataType() { ExitDate = DateTime.Today };
                var ret = db.ToVirkningTypeArray();
                var v = VirkningType.Compose(ret);
                Assert.True(v.FraTidspunkt.ToDateTime().HasValue);
            }

            [Test]
            public void ToVirkningTypeArray_Date_EndIsEmpty()
            {
                var db = new CurrentDepartureDataType() { ExitDate = DateTime.Today };
                var ret = db.ToVirkningTypeArray();
                var v = VirkningType.Compose(ret);
                Assert.False(v.TilTidspunkt.ToDateTime().HasValue);
            }
        }

        [TestFixture]
        public class ToCountryIdentificationCodeType
        {
            [Test]
            public void ToCountryIdentificationCodeType_Code_imk(
                [Values(1, 3, 56)]int countryCode)
            {
                var db = new CurrentDepartureDataType() { ExitCountryCode = countryCode };
                var ret = db.ToCountryIdentificationCode();
                Assert.AreEqual(_CountryIdentificationSchemeType.imk, ret.scheme);
            }

            [Test]
            public void ToCountryIdentificationCodeType_Code_CorrectCode(
                [Values(1, 3, 56)]int countryCode)
            {
                var db = new CurrentDepartureDataType() { ExitCountryCode = countryCode };
                var ret = db.ToCountryIdentificationCode();
                Assert.AreEqual(countryCode, decimal.Parse(ret.Value));
            }
        }

        [TestFixture]
        public class ToAdresseType
        {
            [Test]
            public void ToAdresseType_Empty_VerdenAdress()
            {
                var db = new CurrentDepartureDataType();
                var ret = db.ToAdresseType();
                Assert.IsInstanceOf<VerdenAdresseType>(ret.Item);
            }
        }

        [TestFixture]
        public class ToVerdenAdresseType
        {
            [Test]
            public void ToVerdenAdresseType_Empty_ForeignNotNull()
            {
                var db = new CurrentDepartureDataType() { ExitCountryCode = 1 };
                var ret = db.ToVerdenAdresseType();
                Assert.NotNull(ret.ForeignAddressStructure);
            }

            [Test]
            public void ToVerdenAdresseType_EmptyAndValues_CorrectUkendt(
                [Values(null, "line1")]string line1)
            {
                var db = new CurrentDepartureDataType() { ForeignAddress1 = line1, ExitCountryCode = 1 };
                var ret = db.ToVerdenAdresseType();
                Assert.AreEqual(db.IsEmpty, ret.UkendtAdresseIndikator);
            }
        }
    }
}
